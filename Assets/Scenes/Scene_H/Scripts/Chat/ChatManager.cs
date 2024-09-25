using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ChatManager : MonoBehaviourPun
{
    // Inpu Field
    public InputField inputChat;

    // ChatItem Prefab
    public GameObject chatItemFactory;
    public RectTransform trContent;

    // ChatView의 Transtorm
    public RectTransform trChatView;

    // 채팅이 추가되기 전의 Content의 H(높이) 값을 가지고 있는 변수
    float prevContentH;

    // 닉네임 색상
    Color nameColor;

    void Start()
    {
        nameColor = Random.ColorHSV();

        inputChat.onSubmit.AddListener(OnSubmit);

        #region
        // inputChat의 내용이 변경될 때 호출되는 함수 등록
        //inputChat.onValueChanged.AddListener(OnValueChanged);
        // inputChat 엔터를 쳤을 때 호출되는 함수 등록
        // inputChat 포커싱을 잃을 때 호출되는 함수 등록
        //inputChat.onEndEdit.AddListener(OnEndEdit);
        #endregion
    }
    void Update()
    {
        // 만약에 왼쪽 컨트롤 키를 누르면
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            // 커서를 활성화
            Cursor.lockState = CursorLockMode.None;

        }
        // 만약에 마우스 왼쪽 버튼을 눌렀으면
        if (Input.GetMouseButtonDown(0))
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                // 만약에 UI가 클릭이 되지 않았다면
                if (EventSystem.current.IsPointerOverGameObject() == false)
                {
                    // 커서를 비활성화
                    Cursor.lockState = CursorLockMode.Locked;

                }
            }

        }


    }
    void OnSubmit(string s)
    {
        // 채팅 내용을 NickName : 채팅 내용으로 한다.
        // "<color=#ffffff> 원하는 내용 </color>"
        string nick = "<color=#" + ColorUtility.ToHtmlStringRGB(nameColor) + ">" + PhotonNetwork.NickName + "</color>";
        string chat = nick + ": " + s;

        // 만약에 s의 길이가 0 이면 함수를 나가자
        if (s.Length == 0) return;

        // AddChat RPC 함수 호출
        //AddChat(chat);
       photonView.RPC(nameof(AddChat), RpcTarget.All, chat);
        // 강제로 InputChat 을 활성화 하자
        inputChat.ActivateInputField();

    }

    // 채팅 추가 함수
    [PunRPC]
    void AddChat(string chat)
    {
        // 새로운 채팅이 추가되기 전 Content의 H 값을 저장
        prevContentH = trContent.sizeDelta.y;

        // ChatItem 하나 만들고 부모를 ChatView의 Content 로 하자
        GameObject go = Instantiate(chatItemFactory, trContent);
        // ChatItem 컴포넌트 가져오자
        ChatItem chatItem = go.GetComponent<ChatItem>();

        // 가져온 컴포넌트의 SetText 함수 실행
        chatItem.SetText(chat);
        // 가져온 컴포넌트의 onAutoScroll 변수에 AutoScrollBottom을 세팅하자
        chatItem.onAutoScroll = AutoScrollBottom;

        // inpuChat에 있는 내용을 초기화
        inputChat.text = "";


    }

    // 채팅 추가 되었을 때 맨 밑으로 Content위치를 옮기는 함수
    public void AutoScrollBottom()
    {

        //trChatView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;

        // ChatView의 H 보다 Content의 H 값이 크다면(스크롤이 가능한 상태라면)
        if (trContent.sizeDelta.y > trChatView.sizeDelta.y)
        {
            // 이전 바닥에 닿아있었다면,
            if (prevContentH - trChatView.sizeDelta.y <= trContent.anchoredPosition.y)
            {
                // Content의 y 값을 재설정한다.
                trContent.anchoredPosition = new Vector2(0, trContent.sizeDelta.y - trChatView.sizeDelta.y);
            }
        }
    }

}
