using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class PrefabLoader : MonoBehaviourPunCallbacks
{
    private GameObject[] playerChildren;
    private Animator[] animators;

    // �߰�: ĳ���Ϳ� ��Ī�Ǵ� ĵ���� �迭
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
            ActivateCharacter(0);  // ù ��° ĳ���� �⺻ Ȱ��ȭ
        }
    }

    private void Update()
    {
        AssignPlayerComponents();
        //AssignUIButtons();
    }

    private void AssignPlayerComponents()
    {
        // After instantiation, find child objects and their components (Animator, NavMeshAgent)
        GameObject player = this.gameObject; // This is the Photon-instantiated player object

        if (player != null)
        {
            // Player�� �ڽ� ��ü�� �迭�� ����
            playerChildren = new GameObject[player.transform.childCount];
            animators = new Animator[player.transform.childCount];

            for (int i = 0; i < player.transform.childCount; i++)
            {
                playerChildren[i] = player.transform.GetChild(i).gameObject;

                // �ڵ����� Animator �Ҵ�
                animators[i] = playerChildren[i].GetComponent<Animator>();
            }
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    //private void AssignUIButtons()
    //{
    //    // UI ��ư ã��
    //    button1 = GameObject.Find("btn_Ezreal").GetComponent<Button>();
    //    button2 = GameObject.Find("btn_Garen").GetComponent<Button>();
    //    button3 = GameObject.Find("btn_Minion").GetComponent<Button>();

    //    // ��ư Ŭ�� �̺�Ʈ ���
    //    button1.onClick.AddListener(() => ActivateCharacter(0));
    //    button2.onClick.AddListener(() => ActivateCharacter(1));
    //    button3.onClick.AddListener(() => ActivateCharacter(2));
    //}

    // ĳ���Ϳ� ĵ������ Ȱ��ȭ�ϴ� �Լ�
    void ActivateCharacter(int index)
    {
        if (photonView.IsMine)
        {
            // RPC�� ��� Ŭ���̾�Ʈ�� ĳ���� Ȱ��ȭ ����ȭ
            photonView.RPC("RPC_ActivateCharacter", RpcTarget.AllBuffered, index);
        }
    }

    // RPC�� ĳ���Ϳ� ĵ������ Ȱ��ȭ/��Ȱ��ȭ
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

                    // �ش� �ε����� �ش��ϴ� ĵ���� Ȱ��ȭ
                    if (characterCanvases[i] != null)
                    {
                        characterCanvases[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    playerChildren[i].SetActive(false);
                    if (animators[i] != null) animators[i].enabled = false;

                    // �ٸ� ĵ������ ��Ȱ��ȭ
                    if (characterCanvases[i] != null)
                    {
                        characterCanvases[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    // Ȱ��ȭ�� ĳ������ Transform�� ��ȯ�ϴ� �Լ�
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
        return transform; // Ȱ��ȭ�� ĳ���Ͱ� ������ �÷��̾� ������Ʈ�� transform�� ��ȯ
    }
}
