using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; // ���� ���� ���ӽ����̽� �߰�

public class PlayerNameTag : MonoBehaviour
{
    public Text nameText;  // �÷��̾� �̸��� ǥ���� Text UI ������Ʈ

    void Start()
    {
        // nameText�� �Ҵ�Ǿ����� Ȯ��
        if (nameText == null)
        {
            Debug.LogError("nameText is not assigned in the Inspector!");
            return;
        }

        // Photon ��Ʈ��ũ�� �غ�Ǿ����� Ȯ���ϰ�, �÷��̾� �̸� ����
        if (PhotonNetwork.IsConnected && PhotonNetwork.NickName != null)
        {
            string playerName = PhotonNetwork.NickName;
            nameText.text = playerName;
        }
        else
        {
            Debug.LogError("Photon Network is not connected or NickName is null");
        }
    }
}
