using System.Linq;
using SocketProtocol;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    [SerializeField]
    private Button leaveRoomBtn, startGameBtn, sendChatBtn;
    [SerializeField]
    private InputField chatInput;
    [SerializeField]
    private Transform memberListContent, chatListContent;
    [SerializeField]
    private GameObject roomPanelMemberPrefab, roomPanelChatMessagePrefab;

    private void Start()
    {
        leaveRoomBtn.onClick.AddListener(() => { RoomRequest.LeaveRoomRequest(); });
        startGameBtn.onClick.AddListener(() => { RoomRequest.StartGameRequest(); });
        sendChatBtn.onClick.AddListener(() => { RoomRequest.RoomChatRequest(chatInput.text); });
    }

    private void OnEnable()
    {
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.CreateRoomResponse, OnUpdateMemberList);
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.JoinRoomResponse, OnUpdateMemberList);
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.LeaveRoomResponse, OnUpdateMemberList);
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.RoomChatResponse, OnUpdateChatList);
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.StartGameResponse, OnStartGame);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.CreateRoomResponse, OnUpdateMemberList);
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.JoinRoomResponse, OnUpdateMemberList);
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.LeaveRoomResponse, OnUpdateMemberList);
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.RoomChatResponse, OnUpdateChatList);
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.StartGameResponse, OnStartGame);
    }

    public override void ClearPanel()
    {
        base.ClearPanel();
        foreach (Transform child in memberListContent)
        {
            Destroy(child.gameObject);
            ClientData.Instance.roomUserPacks.Clear();
        }
        foreach (Transform child in chatListContent)
        {
            Destroy(child.gameObject);
        }
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

    private void OnUpdateChatList(MainPack mainPack)
    {
        if (mainPack.ReturnCode == ReturnCode.Success)
        {
            if (mainPack.UserPack.Count > 0)
            {
                Text message = Instantiate(roomPanelChatMessagePrefab, chatListContent).GetComponent<Text>();
                message.text = mainPack.UserPack[0].NickName + ":" + mainPack.ChatStr;
            }
        }
    }

    private void OnStartGame(MainPack mainPack)
    {
        if (mainPack.ReturnCode == ReturnCode.Success)
        {
            Text message = Instantiate(roomPanelChatMessagePrefab, chatListContent).GetComponent<Text>();
            message.text = "开始游戏";
        }
    }
}