using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GaCharacterSkill : MonoBehaviourPunCallbacks
{
    public float cooldownTime = 5f;
    private float cooldownTimer = 0f;
    private bool isCooldown = false;
    private Canvas ezCanvas;
    private Canvas gaCanvas;
    private Image imgCool;

    [SerializeField] private string ezCanvasName = "EzCanvas";
    [SerializeField] private string gaCanvasName = "GaCanvas";

    void Start()
    {
        if (photonView.IsMine)
        {
            FindCanvases();
            SetupCanvases();
        }
    }

    void Update()
    {
        if (photonView.IsMine)
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

    void FindCanvases()
    {
        gaCanvas = FindCanvasByTagOrName("GaCanvas", gaCanvasName);
        ezCanvas = FindCanvasByTagOrName("EzCanvas", ezCanvasName);

        Debug.Log($"FindCanvases result: gaCanvas = {gaCanvas}, ezCanvas = {ezCanvas}");
    }

    Canvas FindCanvasByTagOrName(string tag, string name)
    {
        Canvas foundCanvas = null;
        GameObject canvasObj = GameObject.FindGameObjectWithTag(tag);

        if (canvasObj == null)
        {
            canvasObj = GameObject.Find(name);
        }

        if (canvasObj != null)
        {
            foundCanvas = canvasObj.GetComponent<Canvas>();
            if (foundCanvas != null)
            {
                Debug.Log($"{name} found and assigned successfully.");
            }
            else
            {
                Debug.LogWarning($"{name} found, but it doesn't have a Canvas component.");
            }
        }
        else
        {
            Debug.LogWarning($"Could not find Canvas with tag '{tag}' or name '{name}'.");
        }

        return foundCanvas;
    }

    public void SetupCanvases()
    {
        bool isGaren = transform.GetSiblingIndex() == 1;
        Debug.Log($"SetupCanvases: isGaren = {isGaren}, SiblingIndex = {transform.GetSiblingIndex()}");

        if (gaCanvas != null)
        {
            gaCanvas.gameObject.SetActive(isGaren);
            Debug.Log($"GaCanvas SetActive: {isGaren}");
        }


        if (ezCanvas != null)
        {
            ezCanvas.gameObject.SetActive(!isGaren);
            Debug.Log($"EzCanvas SetActive: {!isGaren}");
        }
        else
        {
            Debug.LogWarning("EzCanvas is null in SetupCanvases");
        }

        Canvas activeCanvas = isGaren ? gaCanvas : ezCanvas;
        if (activeCanvas != null)
        {
            FindAndAssignImgCool(activeCanvas);
        }
        else
        {
            Debug.LogWarning($"Active canvas not found. isGaren: {isGaren}");
        }
    }

    void FindAndAssignImgCool(Canvas targetCanvas)
    {
        if (targetCanvas != null)
        {
            imgCool = FindImageInCanvas(targetCanvas, "img_r", "Image_3") ??
                      FindImageInCanvas(targetCanvas, "img_e", "Image_2") ??
                      FindImageInCanvas(targetCanvas, "img_q", "Image_1");

            if (imgCool == null)
            {
                Debug.LogWarning("Couldn't find any skill cooldown images. Cooldown display may not work correctly.");
            }
        }
        else
        {
            Debug.LogWarning("targetCanvas is null in FindAndAssignImgCool");
        }
    }

    Image FindImageInCanvas(Canvas canvas, string parentName, string imageName)
    {
        if (canvas == null)
        {
            Debug.LogWarning($"Canvas is null when searching for {parentName}/{imageName}");
            return null;
        }

        Transform parentTransform = canvas.transform.Find(parentName);
        if (parentTransform != null)
        {
            Transform imageTransform = parentTransform.Find(imageName);
            if (imageTransform != null)
            {
                Image image = imageTransform.GetComponent<Image>();
                if (image != null)
                {
                    Debug.Log($"{imageName} found and assigned successfully.");
                    return image;
                }
                else
                {
                    Debug.LogWarning($"Image component not found on {imageName}.");
                }
            }
            else
            {
                Debug.LogWarning($"{imageName} not found under {parentName}.");
            }
        }
        else
        {
            Debug.LogWarning($"{parentName} not found in Canvas.");
        }
        return null;
    }

    void UseSkill()
    {
        Debug.Log("Skill used!");
        isCooldown = true;
        cooldownTimer = cooldownTime;

        if (imgCool != null)
        {
            imgCool.fillAmount = 1;
        }
        else
        {
            Debug.LogWarning("imgCool is null in UseSkill");
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
}