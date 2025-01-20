using System.Collections.Generic;
using SocketProtocol;
using UnityEngine;

public class ClientData : Singleton<ClientData>
{
    public UserPack selfUserPack = null;
    public RoomPack selfRoomPack = null;
    public List<UserPack> roomUserPacks = new();
}
