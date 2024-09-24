using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;



public class PhotonLogin : MonoBehaviourPunCallbacks
{
    public InputField usernameInput;
    public Text feedbackText;

    void Start()
    {
        ConnectToPhoton();
    }

    public void ConnectToPhoton()
    {
        if(PhotonNetwork.IsConnected)
        {
            feedbackText.text = "�̹� ������ ����Ǿ����ϴ�.";
            return;
        }

        PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
        PhotonNetwork.ConnectUsingSettings();
        feedbackText.text = "������ ���� ��";
    }

    public void login()
    {
        string username = usernameInput.text;
        if(!string.IsNullOrEmpty(username))
        {
            PhotonNetwork.NickName = username;
            feedbackText.text = "�α��� ����!" + username;
            PhotonNetwork.JoinLobby();
        }
        else
        {
            feedbackText.text = "������� �̸��� �Է��ϼ���.";
        }
    }

    public override void OnConnectedToMaster()
    {
        feedbackText.text = "������ ����Ǿ����ϴ�!";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        feedbackText.text = "���� ���� ����: " + cause.ToString();
    }

    public override void OnJoinedLobby()
    {
        feedbackText.text = "�κ� �����Ͽ����ϴ�";

        // �κ� ������ ��ȯ
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
