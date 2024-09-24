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
            // RPC�� ȣ��Ǹ� ��� �÷��̾�鿡�� �̸�Ƽ���� ǥ����
            photonView.RPC("DisplayEmotion", RpcTarget.All, emotionMessage);
        }
    }

    // �̸�Ƽ���� ǥ�� RPC
    [PunRPC]
    public void DisplayEmotion(string emotionMessage)
    {
        if(currentEmotion != null)
        {
            Destroy(currentEmotion);
        }

        // �̸�Ƽ���� �����ǰ� �÷��̾� �Ӹ� ���� ��Ÿ��
        currentEmotion = Instantiate(emotionPrefab, emotionSpwanPoint.position, Quaternion.identity);
        Text emotionText = currentEmotion.GetComponentInChildren<Text>();
        if(emotionText != null)
        {
            emotionText.text = emotionMessage;
        }
        Destroy(currentEmotion, 3f);
    }
}
