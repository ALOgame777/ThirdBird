using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Photon2Manager : MonoBehaviourPunCallbacks
{
    public void Awake()
    {
        // Set the rate of sending data
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;


        // ������ ����Ǿ� �ְ�, ���� �÷��̾ ���� �������� �ʾ����� ����
        if (PhotonNetwork.IsConnected && PhotonNetwork.LocalPlayer.TagObject == null)
        {
            // ���� �÷��̾ ��Ʈ��ũ �󿡼� ����ȭ�Ͽ� ����
            GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

            // �ߺ� ���� ������ ���� ���� �÷��̾��� TagObject�� �÷��̾� ��ü ����
            PhotonNetwork.LocalPlayer.TagObject = player;

            Debug.Log("���� �÷��̾ �����Ǿ����ϴ�.");
        }
    }
}
