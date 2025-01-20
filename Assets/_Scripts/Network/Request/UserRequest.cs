using Google.Protobuf;
using SocketProtocol;
using UnityEngine;

public static class UserRequest
{
    public static void QuickLoginRequest()
    {
        MainPack mainPack = new()
        {
            RequestCode = RequestCode.User,
            ActionCode = ActionCode.QuickLogin
        };
        ClientManager.Instance.SendMsg(mainPack.ToByteArray());
    }
}
