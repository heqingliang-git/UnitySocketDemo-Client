using System.Linq;
using SocketProtocol;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    [SerializeField]
    private Button leaveRoomBtn, startGameBtn;
    [SerializeField]
    private Transform memberListContent;
    [SerializeField]
    private GameObject roomPanelMemberPrefab;

    private void Start()
    {
        leaveRoomBtn.onClick.AddListener(() => { RoomRequest.LeaveRoomRequest(); });
        startGameBtn.onClick.AddListener(() => { });
    }

    private void OnEnable()
    {
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.CreateRoomResponse, OnUpdateMemberList);
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.JoinRoomResponse, OnUpdateMemberList);
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.LeaveRoomResponse, OnUpdateMemberList);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.CreateRoomResponse, OnUpdateMemberList);
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.JoinRoomResponse, OnUpdateMemberList);
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.LeaveRoomResponse, OnUpdateMemberList);
    }

    private void OnUpdateMemberList(MainPack mainPack)
    {
        if (mainPack.ReturnCode == ReturnCode.Success)
        {
            foreach (Transform child in memberListContent)
            {
                Destroy(child.gameObject);
                ClientData.Instance.roomUserPacks.Clear();
            }

            if (mainPack.UserPack.Any(item => item.UserId == ClientData.Instance.selfUserPack.UserId))
            {
                foreach (var userPack in mainPack.UserPack)
                {
                    RoomPanelMember roomPanelMember = Instantiate(roomPanelMemberPrefab, memberListContent).GetComponent<RoomPanelMember>();
                    roomPanelMember.Init(userPack);
                    ClientData.Instance.roomUserPacks.Add(userPack);
                }
            }
            else
            {
                UIManager.Instance.OpenPanel(UINAME.HallPanel);
                UIManager.Instance.ClosePanel(UINAME.RoomPanel);
                ClientData.Instance.selfRoomPack = null;
            }
        }
    }
}