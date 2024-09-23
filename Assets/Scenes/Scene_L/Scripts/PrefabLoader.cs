using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class PrefabLoader : MonoBehaviourPunCallbacks
{
    private GameObject[] playerChildren;
    private Animator[] animators;

    // 추가: 캐릭터와 매칭되는 캔버스 배열
    public Canvas[] characterCanvases;

    // Buttons for switching characters
    private Button button1;
    private Button button2;
    private Button button3;

    private PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            ActivateCharacter(0);  // 첫 번째 캐릭터 기본 활성화
        }
    }

    private void Update()
    {
        AssignPlayerComponents();
        AssignUIButtons();
    }

    private void AssignPlayerComponents()
    {
        // After instantiation, find child objects and their components (Animator, NavMeshAgent)
        GameObject player = this.gameObject; // This is the Photon-instantiated player object

        if (player != null)
        {
            // Player의 자식 객체를 배열에 저장
            playerChildren = new GameObject[player.transform.childCount];
            animators = new Animator[player.transform.childCount];

            for (int i = 0; i < player.transform.childCount; i++)
            {
                playerChildren[i] = player.transform.GetChild(i).gameObject;

                // 자동으로 Animator 할당
                animators[i] = playerChildren[i].GetComponent<Animator>();
            }
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    private void AssignUIButtons()
    {
        // UI 버튼 찾기
        button1 = GameObject.Find("btn_Ezreal").GetComponent<Button>();
        button2 = GameObject.Find("btn_Garen").GetComponent<Button>();
        button3 = GameObject.Find("btn_Minion").GetComponent<Button>();

        // 버튼 클릭 이벤트 등록
        button1.onClick.AddListener(() => ActivateCharacter(0));
        button2.onClick.AddListener(() => ActivateCharacter(1));
        button3.onClick.AddListener(() => ActivateCharacter(2));
    }

    // 캐릭터와 캔버스를 활성화하는 함수
    void ActivateCharacter(int index)
    {
        if (photonView.IsMine)
        {
            // RPC로 모든 클라이언트에 캐릭터 활성화 동기화
            photonView.RPC("RPC_ActivateCharacter", RpcTarget.AllBuffered, index);
        }
    }

    // RPC로 캐릭터와 캔버스를 활성화/비활성화
    [PunRPC]
    void RPC_ActivateCharacter(int index)
    {
        if (playerChildren != null && characterCanvases != null)
        {
            for (int i = 0; i < playerChildren.Length; i++)
            {
                if (i == index)
                {
                    playerChildren[i].SetActive(true);
                    if (animators[i] != null) animators[i].enabled = true;

                    // 해당 인덱스에 해당하는 캔버스 활성화
                    if (characterCanvases[i] != null)
                    {
                        characterCanvases[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    playerChildren[i].SetActive(false);
                    if (animators[i] != null) animators[i].enabled = false;

                    // 다른 캔버스는 비활성화
                    if (characterCanvases[i] != null)
                    {
                        characterCanvases[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    // 활성화된 캐릭터의 Transform을 반환하는 함수
    public Transform GetActiveCharacterTransform()
    {
        if (playerChildren != null)
        {
            for (int i = 0; i < playerChildren.Length; i++)
            {
                if (playerChildren[i].activeSelf)
                {
                    return playerChildren[i].transform;
                }
            }
        }
        return transform; // 활성화된 캐릭터가 없으면 플레이어 오브젝트의 transform을 반환
    }
}
