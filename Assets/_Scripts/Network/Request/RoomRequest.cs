using Google.Protobuf;
using SocketProtocol;
using UnityEngine;

public static class RoomRequest
{
    public static void CreateRoomRequest(string roomName, int roomCapacity)
    {
        MainPack mainPack = new()
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.CreateRoom
        };
        mainPack.RoomPacks.Add(new RoomPack
        {
            RoomName = roomName,
            RoomMemberCapacity = roomCapacity,
            RoomState = RoomState.Waiting
        });
        ClientManager.Instance.SendMsg(mainPack.ToByteArray());
    }

    public static void FindRoomRequest()
    {
        MainPack mainPack = new()
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.FindRoom
        };
        ClientManager.Instance.SendMsg(mainPack.ToByteArray());
    }


    public static void JoinRoomRequest(int roomId)
    {
        MainPack mainPack = new()
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.JoinRoom
        };
        mainPack.RoomPacks.Add(new RoomPack
        {
            RoomId = roomId
        });
        ClientManager.Instance.SendMsg(mainPack.ToByteArray());
    }

    public static void LeaveRoomRequest()
    {
        MainPack mainPack = new()
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.LeaveRoom
        };
        ClientManager.Instance.SendMsg(mainPack.ToByteArray());
    }

    public static void RoomChatRequest(string message)
    {
        MainPack mainPack = new()
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.RoomChat,
            ChatStr = message
        };
        ClientManager.Instance.SendMsg(mainPack.ToByteArray());
    }

    public static void StartGameRequest()
    {
        MainPack mainPack = new()
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.StartGame
        };
        ClientManager.Instance.SendMsg(mainPack.ToByteArray());
    }
}
