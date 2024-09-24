using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; // 포톤 관련 네임스페이스 추가

public class PlayerNameTag : MonoBehaviour
{
    public Text nameText;  // 플레이어 이름을 표시할 Text UI 컴포넌트

    void Start()
    {
        // nameText가 할당되었는지 확인
        if (nameText == null)
        {
            Debug.LogError("nameText is not assigned in the Inspector!");
            return;
        }

        // Photon 네트워크가 준비되었는지 확인하고, 플레이어 이름 설정
        if (PhotonNetwork.IsConnected && PhotonNetwork.NickName != null)
        {
            string playerName = PhotonNetwork.NickName;
            nameText.text = playerName;
        }
        else
        {
            Debug.LogError("Photon Network is not connected or NickName is null");
        }
    }
}
