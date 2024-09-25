using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonChatMgr : MonoBehaviour, IChatClientListener
{
    public GameObject chatViewGo;

    // 채팅을 총괄하는 객체
    ChatClient chatClient;
    // 채팅 입력 UI
    public InputField inputChat;

    // 채팅 채널
    string currChannel = "메타";

    // 스크롤뷰의 Content
    public RectTransform trContent;

    // 챗 아이템 팩토리
    public GameObject chatfactory;

    void Start()
    {
        chatViewGo.SetActive(false);

        // 채팅내용을 작성하고 엔터를 쳤을 때 호출되는 함수 등록
        inputChat.onSubmit.AddListener(OnSubmit);
        Connect();
    }

    void Update()
    {
        // 채팅서버에서 오는 응답을 수신하기 위해서 계속 호출해야함
        if (chatClient != null)
        {
            chatClient.Service();
        }
        // 채널에서 나가자
        if (Input.GetKeyDown(KeyCode.F5))
        {
            string[] channels = { currChannel };
            chatClient.Unsubscribe(channels);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            // 현재 활성화 상태를 반대로 설정 (true이면 false, false이면 true)
            chatViewGo.SetActive(!chatViewGo.activeSelf);
        }
    }

    void Connect()
    {
        // 포톤 설정을 가져오자
        AppSettings photonSettings = PhotonNetwork.PhotonServerSettings.AppSettings;

        // 위 설정을 가지고 ChatAppSettings를 세팅(복제)
        ChatAppSettings chatAppSettings = new ChatAppSettings();
        chatAppSettings.AppIdChat = photonSettings.AppIdChat;
        chatAppSettings.AppVersion = photonSettings.AppVersion;
        chatAppSettings.FixedRegion = photonSettings.FixedRegion;
        chatAppSettings.NetworkLogging = photonSettings.NetworkLogging;
        chatAppSettings.Protocol = photonSettings.Protocol;
        chatAppSettings.EnableProtocolFallback = photonSettings.EnableProtocolFallback;
        chatAppSettings.Server = photonSettings.Server;
        chatAppSettings.Port = (ushort)photonSettings.Port;
        chatAppSettings.ProxyServer = photonSettings.ProxyServer;

        // ChatClient 만들자
        chatClient = new ChatClient(this);
        // 닉네임
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName);
        // 연결시도
        chatClient.ConnectUsingSettings(chatAppSettings);
    }
    void OnSubmit(string s)
    {
        // 만약에 s의 길이가 0이면 함수를 나가자
        if (s.Length == 0) return;
        // 채팅을 보내자
        chatClient.PublishMessage(currChannel, s);
        // 채팅 입력란 초기화
        inputChat.text = "";

    }

    void CreateChatItem(string sender, object message, Color color)
    {
        // Content의 자식으로 챗아이템 생성
        GameObject go = Instantiate(chatfactory, trContent);
        ChatItem chatItem = go.GetComponent<ChatItem>();
        chatItem.SetText(sender + " : " + message);

        // TMP_Text 컴포넌트 가져오기
        Text text = go.GetComponent<Text>();
        // 텍스트 색 바꾸기
        text.color = color;
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnDisconnected()
    {
    }

    // 채팅 서버에 접속이 성공하면 호출되는 함수
    public void OnConnected()
    {
        print("채팅 서버 접속 성공");
        // "전체" 채널에 들어가자(구독Subscribed)
        chatClient.Subscribe(currChannel);
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    // 특정 채널에 다른나람(나)이 메세지르 보내ㅗㄱ\고 나한테 응답이 올대 호ㅜㄹ돠눈 험슈]
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            print(senders[i] + " : " + messages[i]);

            CreateChatItem(senders[i], messages[i], Color.black);
        }
    }

    // 누군가 나한테 개인메세지를 보냈을 때
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        CreateChatItem(sender, message, Color.green);
    }

    // 채팅채널에 접속 성공했을 때 들어오는 함수
    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            print(channels[i] + " 채널에 접속 성공 했습니다.");
        }
    }


    // 채팅 채널에서 나갔을 때 들어오는 함수
    public void OnUnsubscribed(string[] channels)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            print(channels[i] + " 채널에서 나갔습니다.");
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }

}