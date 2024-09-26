using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NicknameSetter : MonoBehaviourPunCallbacks
{
    public Text nicknameText; // NickName ������Ʈ�� Text ������Ʈ�� ������ ����

    void Start()
    {
        // ���� �г��� ���� �� ����
        string randomNickname = "Player" + Random.Range(1000, 9999);
        PhotonNetwork.NickName = randomNickname;

        // �г����� ��� �÷��̾�� ����ȭ
        photonView.RPC("SetNickname", RpcTarget.All, randomNickname);
    }

    [PunRPC]
    void SetNickname(string nickname)
    {
        // UI�� �г��� ǥ��
        if (nicknameText != null)
        {
            nicknameText.text = nickname;
        }
        else
        {
            Debug.LogError("NickName Text ������Ʈ�� ������� �ʾҽ��ϴ�!");
        }
    }
}
