using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class EmotionManager : MonoBehaviour
{
    private PhotonView photonView;
    public GameObject emotionPrefab;
    public Transform emotionSpwanPoint;
    public GameObject currentEmotion;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    
    private void SendEmotion(string emotionMessage)
    {
        if(photonView.IsMine)
        {
            // RPC가 호출되며 모든 플레이어들에게 이모티콘을 표시함
            photonView.RPC("DisplayEmotion", RpcTarget.All, emotionMessage);
        }
    }

    // 이모티콘을 표시 RPC
    [PunRPC]
    public void DisplayEmotion(string emotionMessage)
    {
        if(currentEmotion != null)
        {
            Destroy(currentEmotion);
        }

        // 이모티콘이 생성되고 플레이어 머리 위에 나타남
        currentEmotion = Instantiate(emotionPrefab, emotionSpwanPoint.position, Quaternion.identity);
        Text emotionText = currentEmotion.GetComponentInChildren<Text>();
        if(emotionText != null)
        {
            emotionText.text = emotionMessage;
        }
        Destroy(currentEmotion, 3f);
    }
}
