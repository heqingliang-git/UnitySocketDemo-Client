using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SocketProtocol;
using UnityEngine;

public class ClientManager : SingletonMono<ClientManager>
{
    //客户端socket
    private Socket clientSocket;

    #region 发送数据的变量
    //发送消息的队列
    private Queue<byte[]> sendQueue = new Queue<byte[]>();

    //检查队列的委托
    private Action checkSendQueueAction;
    #endregion


    #region 接收数据的变量
    //接收数据的线程
    private Thread receiveThread;

    //接收数据包的缓冲数据流
    private MemoryStream receiveStream = new MemoryStream();

    //接收数据包的字节数组缓冲区
    private byte[] receiveBuffer = new byte[1024];

    //接收到的字节数组队列
    private Queue<byte[]> receiveQueue = new Queue<byte[]>();

    //当前帧最多处理的消息条数
    private int frameReceiveCount = 5;
    #endregion

    private void Start()
    {
        Connect("127.0.0.1", 1011);
    }

    private void Update()
    {
        int receiveCount = frameReceiveCount;
        while (receiveCount > 0)
        {
            receiveCount--;
            lock (receiveQueue)
            {
                if (receiveQueue.Count > 0)
                {
                    byte[] buffer = receiveQueue.Dequeue();

                    //对这个完整的包进行处理
                    MainPack mainPack = MainPack.Parser.ParseFrom(buffer);
                    EventCenter.Instance.Broadcast(mainPack.ActionCode.ToString() + "Response", mainPack);
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void OnDestroy()
    {
        //关闭已连接的socket
        if (clientSocket != null && clientSocket.Connected)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }

    /// <summary>
    /// 连接到socket服务器
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public bool Connect(string ip, int port)
    {
        //若socket已存在且在连接中则直接返回
        if (clientSocket != null && clientSocket.Connected) return false;

        //创建socket
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            //启动连接
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

            Debug.Log("socket连接成功");

            //启动一个接收数据的线程
            receiveThread = new Thread(ReceiveMsg);
            receiveThread.Start();

            //监听发送队列的委托
            checkSendQueueAction = OnCheckSendQueueCallBack;

            return true;
        }
        catch (Exception ex)
        {
            Debug.Log("socket连接失败=" + ex.Message);
            return false;
        }
    }


    #region 发送数据的方法
    /// <summary>
    /// 检查发送消息队列的回调
    /// </summary>
    private void OnCheckSendQueueCallBack()
    {
        lock (sendQueue)
        {
            //若队列中有数据包，则发送数据包
            if (sendQueue.Count > 0)
            {
                Send(sendQueue.Dequeue());
            }
        }
    }

    /// <summary>
    /// 封装数据包
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private byte[] MakeData(byte[] data)
    {
        byte[] retBuffer = null;
        using (MemoryStream stream = new MemoryStream())
        {
            //写入4字节包头
            byte[] arr = BitConverter.GetBytes((uint)data.Length);
            stream.Write(arr, 0, arr.Length);
            //写入包体
            stream.Write(data, 0, data.Length);
            retBuffer = stream.ToArray();
        }
        return retBuffer;
    }

    /// <summary>
    /// 发送消息（把消息加入发送队列）
    /// </summary>
    /// <param name="buffer"></param>
    public void SendMsg(byte[] buffer)
    {
        //得到封装后的数据包
        byte[] sendBuffer = MakeData(buffer);

        lock (sendQueue)
        {
            //把数据包加入发送队列
            sendQueue.Enqueue(sendBuffer);

            //启动委托(执行委托)
            Task.Run(() => checkSendQueueAction.Invoke());
        }
    }

    /// <summary>
    /// 发送数据包
    /// </summary>
    /// <param name="buffer"></param>
    private void Send(byte[] buffer)
    {
        clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, clientSocket);
    }

    /// <summary>
    /// 发送数据包的回调
    /// </summary>
    /// <param name="ar"></param>
    private void SendCallBack(IAsyncResult ar)
    {
        clientSocket.EndSend(ar);

        //继续检查队列
        OnCheckSendQueueCallBack();
    }
    #endregion


    #region 接收数据的方法
    /// <summary>
    /// 接收数据
    /// </summary>
    private void ReceiveMsg()
    {
        //异步接收数据
        clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallBack, clientSocket);
    }

    /// <summary>
    /// 接收数据的回调
    /// </summary>
    /// <param name="ar"></param>
    private void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            int len = clientSocket.EndReceive(ar);

            //收到了数据
            if (len > 0)
            {
                //把接收到的数据写入缓冲数据流的尾部
                receiveStream.Position = receiveStream.Length;

                //把指定长度的字节写入数据流
                receiveStream.Write(receiveBuffer, 0, len);

                //缓存数据流长度大于4，则至少接收到一个不完整的包
                //因为客户端封装数据包的uint长度为4
                if (receiveStream.Length > 4)
                {
                    while (true)
                    {
                        //把数据流指针位置放在0处
                        receiveStream.Position = 0;

                        //包体长度
                        byte[] arr = new byte[2];
                        receiveStream.Read(arr, 0, 2);
                        int curMsgLen = BitConverter.ToUInt16(arr, 0);

                        //总包的长度=包头长度+包体长度
                        int curFullMsgLen = 4 + curMsgLen;

                        //数据流长度>=总包长度，则至少收到一个完整的包
                        if (receiveStream.Length >= curFullMsgLen)
                        {
                            //拆包
                            //包体
                            byte[] buffer = new byte[curMsgLen];

                            //把数据流指针放到包体的位置
                            receiveStream.Position = 4;

                            //把包体数据读到包体数组
                            receiveStream.Read(buffer, 0, curMsgLen);

                            lock (receiveQueue)
                            {
                                receiveQueue.Enqueue(buffer);
                            }

                            //处理剩余字节数组
                            int remainLen = (int)receiveStream.Length - curFullMsgLen;
                            if (remainLen > 0)
                            {
                                //把数据流指针放在第一个包的尾部
                                receiveStream.Position = curFullMsgLen;

                                //剩余字节数组
                                byte[] remainBuffer = new byte[remainLen];

                                //把剩余数据流读到剩余字节数组
                                receiveStream.Read(remainBuffer, 0, remainLen);

                                //清空数据流
                                receiveStream.Position = 0;
                                receiveStream.SetLength(0);

                                //把剩余字节数组重新写入数据流
                                receiveStream.Write(remainBuffer, 0, remainBuffer.Length);

                                remainBuffer = null;
                            }
                            else
                            {
                                //清空数据流
                                receiveStream.Position = 0;
                                receiveStream.SetLength(0);
                                break;
                            }
                        }
                        //还没有收到完整的包
                        else
                        {
                            break;
                        }
                    }
                }

                //进行下一次接收数据包
                ReceiveMsg();
            }
            //没有收到数据，即服务器断开连接
            else
            {
                Debug.Log(string.Format("服务器{0}断开连接", clientSocket.RemoteEndPoint.ToString()));

            }
        }
        catch
        {
            Debug.Log(string.Format("服务器{0}断开连接", clientSocket.RemoteEndPoint.ToString()));
        }
    }
    #endregion



}
