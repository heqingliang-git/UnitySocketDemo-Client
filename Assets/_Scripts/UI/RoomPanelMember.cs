using SocketProtocol;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanelMember : MonoBehaviour
{
    [SerializeField] private Text userIdTxt, nickNameTxt;

    public void Init(UserPack userPack)
    {
        userIdTxt.text = userPack.UserId.ToString();
        nickNameTxt.text = userPack.NickName;
    }
}
