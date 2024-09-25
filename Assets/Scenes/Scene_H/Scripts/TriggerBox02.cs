using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBox02 : MonoBehaviour
{
    public GameObject guideUI;
    public GameObject eventUI;

    
    void Start()
    {

        guideUI.SetActive(false);
        eventUI.SetActive(false);
      
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // 태그가 "Player"라면 guideUI를 활성화
        if (other.CompareTag("Player"))
        {
            guideUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        guideUI.SetActive(false);

    }

    public void YesClickEvent01()
    {
        guideUI.SetActive(false);
        eventUI.SetActive(true);
             
    }
    public void NoClickEvent01()
    {
        guideUI.SetActive(false);
    }

    public void XClick01()
    {
        eventUI.SetActive(false);

    }
}
