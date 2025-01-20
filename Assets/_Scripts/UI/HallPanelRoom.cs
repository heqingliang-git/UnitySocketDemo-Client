using SocketProtocol;
using UnityEngine;
using UnityEngine.UI;

public class HallPanelRoom : MonoBehaviour
{
    [SerializeField]
    private Text roomNameTxt, roomIdTxt, roomMemberCountTxt, roomCapacityTxt, roomStateTxt;
    [SerializeField]
    private Button joinRoomBtn;

    public void Init(RoomPack roomPack)
    {
        roomNameTxt.text = roomPack.RoomName;
        roomIdTxt.text = roomPack.RoomId.ToString();
        roomMemberCountTxt.text = roomPack.RoomMemberCount.ToString();
        roomCapacityTxt.text = roomPack.RoomMemberCapacity.ToString();
        roomStateTxt.text = roomPack.RoomState.ToString();
        joinRoomBtn.onClick.RemoveAllListeners();
        joinRoomBtn.onClick.AddListener(() =>
        {
            RoomRequest.JoinRoomRequest(roomPack.RoomId);
        });
    }
}