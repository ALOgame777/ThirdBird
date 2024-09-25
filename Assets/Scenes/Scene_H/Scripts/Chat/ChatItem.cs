using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class ChatItem : MonoBehaviour
{
    // Text
    Text chatText;

    // 매개없는 함수 담을 변수
    public Action onAutoScroll;
    private void Awake()
    {
        chatText = GetComponent<Text>();

    }
    void Start()
    {
    }

    void Update()
    {

    }

    public void SetText(string s)
    {
        // 텍스트 갱신
        chatText.text = s;
        // 사이즈조절 코루틴실행
        StartCoroutine(UpdateSize());
    }

    IEnumerator UpdateSize()
    {
        yield return null;

        // 텍스트의 내용에 맞춰 크기를 조절
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, chatText.preferredHeight);

        yield return null;

        // 만약에 onAutoScroll에 함수가 들어 있다면
        if (onAutoScroll != null)
        {
            onAutoScroll();
        }

        //// ChatView 게임오브젝트 찾자
        //GameObject go = GameObject.Find("ChatView");
        //// 찾은 ChatView 오브젝트에서 ChatManager컴포넌트 가져오자
        //ChatManager cm = go.GetComponent<ChatManager>();
        //// 가져온 컴포넌트에서 AutoScrollBotoom 함수 호출
        //cm.AutoScrollBottom();
    }
}
