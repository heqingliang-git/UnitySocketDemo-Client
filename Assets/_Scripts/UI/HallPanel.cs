using SocketProtocol;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HallPanel : BasePanel
{
    [SerializeField]
    private Button findRoomBtn, createRoomBtn;
    [SerializeField]
    private InputField createRoomNameInput, createRoomCapacityInput;
    [SerializeField]
    private Transform roomListContent;
    [SerializeField]
    private GameObject hallPanelRoomPrefab;

    private void Start()
    {
        findRoomBtn.onClick.AddListener(() =>
        {
            RoomRequest.FindRoomRequest();
        });
        createRoomBtn.onClick.AddListener(() =>
        {
            RoomRequest.CreateRoomRequest(createRoomNameInput.text, int.Parse(createRoomCapacityInput.text));
        });
    }

    private void OnEnable()
    {
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.FindRoomResponse, OnUpdateRoomList);
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.CreateRoomResponse, OnEnterRoom);
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.JoinRoomResponse, OnEnterRoom);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.FindRoomResponse, OnUpdateRoomList);
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.CreateRoomResponse, OnEnterRoom);
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.JoinRoomResponse, OnEnterRoom);
    }

    public override void ClearPanel()
    {
        base.ClearPanel();
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnUpdateRoomList(MainPack mainPack)
    {
        if (mainPack.ReturnCode == ReturnCode.Success)
        {
            foreach (Transform child in roomListContent)
            {
                Destroy(child.gameObject);
            }
            foreach (var roomPack in mainPack.RoomPacks)
            {
                HallPanelRoom roomObj = Instantiate(hallPanelRoomPrefab, roomListContent).GetComponent<HallPanelRoom>();
                roomObj.Init(roomPack);
            }
        }
    }

    private void OnEnterRoom(MainPack mainPack)
    {
        if (mainPack.ReturnCode == ReturnCode.Success)
        {
            UIManager.Instance.OpenPanel(UINAME.RoomPanel);
            UIManager.Instance.ClosePanel(UINAME.HallPanel);

            ClientData.Instance.selfRoomPack = mainPack.RoomPacks[0];
            EventCenter.Instance.Broadcast(EVENTNAME.CreateRoomResponse, mainPack);
        }
    }
}
