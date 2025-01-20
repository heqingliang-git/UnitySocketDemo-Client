using SocketProtocol;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : BasePanel
{
    [SerializeField]
    private Button quickLoginBtn, loginBtn, registBtn;

    private void Start()
    {
        quickLoginBtn.onClick.AddListener(() =>
        {
            UserRequest.QuickLoginRequest();
        });
    }

    private void OnEnable()
    {
        EventCenter.Instance.AddListener<MainPack>(EVENTNAME.QuickLoginResponse, OnQuickLoginResponse);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveListener<MainPack>(EVENTNAME.QuickLoginResponse, OnQuickLoginResponse);
    }

    private void OnQuickLoginResponse(MainPack mainPack)
    {
        if (mainPack.ReturnCode == ReturnCode.Success)
        {
            UIManager.Instance.OpenPanel(UINAME.HallPanel);
            UIManager.Instance.ClosePanel(UINAME.UserPanel);

            if (mainPack.UserPack.Count > 0)
            {
                ClientData.Instance.selfUserPack = mainPack.UserPack[0];
            }
        }
    }
}
