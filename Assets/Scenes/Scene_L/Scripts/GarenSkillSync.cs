using Photon.Pun;
using UnityEngine;

public class GarenSkillSync : MonoBehaviourPunCallbacks
{
    public GameObject img_q;
    public GameObject img_e;
    public GameObject img_Gr;

    // To track whether the keys are pressed
    private bool qKeyPressed = false;
    private bool eKeyPressed = false;
    private bool grKeyPressed = false;

    void Update()
    {
        // Checking key input locally
        if (Input.GetKeyDown(KeyCode.Q))
        {
            photonView.RPC("Qkey", RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            photonView.RPC("EKey", RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            photonView.RPC("GrKey", RpcTarget.All);
        }
    }

    [PunRPC]
    void ToggleQKey()
    {
        qKeyPressed = !qKeyPressed;
        img_q.SetActive(qKeyPressed);
    }

    [PunRPC]
    void ToggleEKey()
    {
        eKeyPressed = !eKeyPressed;
        img_e.SetActive(eKeyPressed);
    }

    [PunRPC]
    void ToggleGrKey()
    {
        grKeyPressed = !grKeyPressed;
        img_Gr.SetActive(grKeyPressed);
    }
}
