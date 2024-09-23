using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class ConManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Photon 환경설정을 기반으로 마스터 서버에 접속을 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 마스터 서버에 접속이 되면 호출되는 함수
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        print("마스터 서버에 접속");

        // 로비 접속
        JoinLobby();
    }

    public void JoinLobby()
    {
        // 닉네임 설정
        PhotonNetwork.NickName = "이준수";
        // 기본 Lobby 입장
        PhotonNetwork.JoinLobby();
    }

    // 로비에 참여가 성공하면 호출되는 함수
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print("로비 입장 완료");
        joinOrCreateRoom();
    }

    // 방 만드는 버튼을 하거나, 조인을 하거나

    // 방을 만들거나 참여를 하는 기능이 포톤에 있음

    // Room을 참여하자, 만약에 해당 Room이 없으면 Room을 만들겠다.

    public void joinOrCreateRoom()
    {
        // 방 생성 옵션
        RoomOptions roomOptions = new RoomOptions();
        // 방에 들어 올 수 있는 최대 인원 설정
        roomOptions.MaxPlayers = 20;
        // 로비에 방을 보이게 할것이니?
        roomOptions.IsVisible = true;
        // 방에 참여를 할 수 있니?
        roomOptions.IsOpen = true;

        // Room 참여 or 생성
        PhotonNetwork.JoinOrCreateRoom("dddd", roomOptions, TypedLobby.Default);
    }
    private void Awake()
    {
        Screen.SetResolution(768, 768, false);
    }
    // 방 생성 성공 했을 때 호출되는 함수
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("방 생성 완료");
    }

    // 방 입장 서공 했을 때 호출 되는 함수
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("방 입장 완료");

        // 멀티플레이 컨텐츠 즐길 수 있는 상태
        // GameScene으로 이동!
        PhotonNetwork.LoadLevel("SecondBird");

    }
}
