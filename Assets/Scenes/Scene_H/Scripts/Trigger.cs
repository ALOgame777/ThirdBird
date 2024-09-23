using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Trigger : MonoBehaviour
{
    public GameObject btnUi;

    void Start()
    {
        btnUi.SetActive(false);
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        btnUi.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")

            btnUi.SetActive(false);
    }

    public void GoOXGame()
    {
        SceneManager.LoadScene("OX_Stage");
    }
    public void GoMatch()
    {
        SceneManager.LoadScene("Scene_K");

    }

    public void GoHome()
    {
        SceneManager.LoadScene("ProtoScene");

    }

}
