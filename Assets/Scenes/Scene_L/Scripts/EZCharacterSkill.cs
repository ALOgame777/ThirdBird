using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EZCharacterSkill : MonoBehaviourPunCallbacks
{
    public float cooldownTime = 5f;
    private float cooldownTimer = 0f;
    private bool isCooldown = false;
    [SerializeField] private Canvas ezCanvas;
    [SerializeField] private Canvas gaCanvas;
    public Image imgCool;
    public GameObject bulletFactory;
    Animator anim;
    public Transform firePos;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            FindAndAssignEzCanvas();
            FindAndAssignImgCool();
        }
  
    }

    void Update()
    {
        if (photonView.IsMine == false) return;
        {
            if (Input.GetKeyDown(KeyCode.R) && !isCooldown)
            {
                UseSkill();
            }

            if (isCooldown)
            {
                ApplyCooldown();
            }
        }
    }

    void FindAndAssignEzCanvas()
    {
        ezCanvas = GameObject.Find("EzCanvas").GetComponent<Canvas>();
        if (ezCanvas != null)
        {
            Debug.Log("EzCanvas found and assigned successfully.");
        }
        else
        {
            Debug.LogError("EzCanvas not found in the scene!");
        }
    }
    void FindAndGaCanvas()
    {
        gaCanvas = GameObject.Find("GaCanvas").GetComponent<Canvas>();
        
    }

    void FindAndAssignImgCool()
    {
        if (ezCanvas != null)
        {
            Transform imgRTransform = ezCanvas.transform.Find("img_r");
            if (imgRTransform != null)
            {
                Transform imgCoolTransform = imgRTransform.Find("img_cool");
                if (imgCoolTransform != null)
                {
                    imgCool = imgCoolTransform.GetComponent<Image>();
                    if (imgCool != null)
                    {
                        Debug.Log("img_cool found and assigned successfully.");
                    }
                    else
                    {
                        Debug.LogError("Image component not found on img_cool!");
                    }
                }
                else
                {
                    Debug.LogError("img_cool not found under img_r!");
                }
            }
            else
            {
                Debug.LogError("img_r not found in EzCanvas!");
            }
        }
    }

    void UseSkill()
    {
        Debug.Log("Skill used!");
        isCooldown = true;
        cooldownTimer = cooldownTime;
        photonView.RPC(nameof(SetTrigger), RpcTarget.All, "EzRkey");
        // 디버그 로그 추가
        Debug.Log("Attempting to instantiate ExplosionSlash at position: " + firePos.position);
        PhotonNetwork.Instantiate("ExplosionSlash", firePos.position , Camera.main.transform.rotation);
        if (imgCool != null)
        {
            
            imgCool.fillAmount = 1;
        }
        
    }

    void ApplyCooldown()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            isCooldown = false;
            cooldownTimer = 0f;
        }

        if (imgCool != null)
        {
            imgCool.fillAmount = cooldownTimer / cooldownTime;
        }
    }

    [PunRPC]
    void SetTrigger(string parameter)
    {
        anim.SetTrigger(parameter);
    }

    [PunRPC]
    void CreateI(Vector3 position , Quaternion rotation)
    {
        Instantiate(bulletFactory, position, rotation);
    }
}