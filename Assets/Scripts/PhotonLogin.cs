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
            feedbackText.text = "이미 서버에 연결되었습니다.";
            return;
        }

        PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
        PhotonNetwork.ConnectUsingSettings();
        feedbackText.text = "서버에 연결 중";
    }

    public void login()
    {
        string username = usernameInput.text;
        if(!string.IsNullOrEmpty(username))
        {
            PhotonNetwork.NickName = username;
            feedbackText.text = "로그인 성공!" + username;
            PhotonNetwork.JoinLobby();
        }
        else
        {
            feedbackText.text = "사용자의 이름을 입력하세요.";
        }
    }

    public override void OnConnectedToMaster()
    {
        feedbackText.text = "서버에 연결되었습니다!";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        feedbackText.text = "서버 연결 실패: " + cause.ToString();
    }

    public override void OnJoinedLobby()
    {
        feedbackText.text = "로비에 입장하였습니다";

        // 로비 씬으로 전환
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
