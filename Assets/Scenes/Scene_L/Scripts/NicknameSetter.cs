using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NicknameSetter : MonoBehaviourPunCallbacks
{
    public Text nicknameText; // NickName 오브젝트의 Text 컴포넌트를 연결할 변수

    void Start()
    {
        // 랜덤 닉네임 생성 및 설정
        string randomNickname = "Player" + Random.Range(1000, 9999);
        PhotonNetwork.NickName = randomNickname;

        // 닉네임을 모든 플레이어에게 동기화
        photonView.RPC("SetNickname", RpcTarget.All, randomNickname);
    }

    [PunRPC]
    void SetNickname(string nickname)
    {
        // UI에 닉네임 표시
        if (nicknameText != null)
        {
            nicknameText.text = nickname;
        }
        else
        {
            Debug.LogError("NickName Text 컴포넌트가 연결되지 않았습니다!");
        }
    }
}
