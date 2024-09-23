using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class ConManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Photon ȯ�漳���� ������� ������ ������ ������ �õ�
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ������ ������ ������ �Ǹ� ȣ��Ǵ� �Լ�
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        print("������ ������ ����");

        // �κ� ����
        JoinLobby();
    }

    public void JoinLobby()
    {
        // �г��� ����
        PhotonNetwork.NickName = "���ؼ�";
        // �⺻ Lobby ����
        PhotonNetwork.JoinLobby();
    }

    // �κ� ������ �����ϸ� ȣ��Ǵ� �Լ�
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print("�κ� ���� �Ϸ�");
        joinOrCreateRoom();
    }

    // �� ����� ��ư�� �ϰų�, ������ �ϰų�

    // ���� ����ų� ������ �ϴ� ����� ���濡 ����

    // Room�� ��������, ���࿡ �ش� Room�� ������ Room�� ����ڴ�.

    public void joinOrCreateRoom()
    {
        // �� ���� �ɼ�
        RoomOptions roomOptions = new RoomOptions();
        // �濡 ��� �� �� �ִ� �ִ� �ο� ����
        roomOptions.MaxPlayers = 20;
        // �κ� ���� ���̰� �Ұ��̴�?
        roomOptions.IsVisible = true;
        // �濡 ������ �� �� �ִ�?
        roomOptions.IsOpen = true;

        // Room ���� or ����
        PhotonNetwork.JoinOrCreateRoom("dddd", roomOptions, TypedLobby.Default);
    }
    private void Awake()
    {
        Screen.SetResolution(768, 768, false);
    }
    // �� ���� ���� ���� �� ȣ��Ǵ� �Լ�
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("�� ���� �Ϸ�");
    }

    // �� ���� ���� ���� �� ȣ�� �Ǵ� �Լ�
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("�� ���� �Ϸ�");

        // ��Ƽ�÷��� ������ ��� �� �ִ� ����
        // GameScene���� �̵�!
        PhotonNetwork.LoadLevel("SecondBird");

    }
}
