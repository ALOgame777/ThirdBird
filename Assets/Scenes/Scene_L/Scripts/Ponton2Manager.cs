using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Photon2Manager : MonoBehaviourPunCallbacks
{
    public void Awake()
    {
        // Set the rate of sending data
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;


        // 서버에 연결되어 있고, 로컬 플레이어가 아직 생성되지 않았으면 생성
        if (PhotonNetwork.IsConnected && PhotonNetwork.LocalPlayer.TagObject == null)
        {
            // 로컬 플레이어를 네트워크 상에서 동기화하여 생성
            GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

            // 중복 생성 방지를 위해 로컬 플레이어의 TagObject에 플레이어 객체 저장
            PhotonNetwork.LocalPlayer.TagObject = player;

            Debug.Log("로컬 플레이어가 생성되었습니다.");
        }
    }
}
