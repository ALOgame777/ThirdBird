using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterCanvasManager : MonoBehaviourPunCallbacks
{
    public GameObject[] characters;  // 0, 1, 2 인덱스 캐릭터
    private Canvas[] characterCanvases;  // 각 캐릭터에 맞는 Canvas들
    private int activeCharacterIndex = 0; // 기본값 0

    private void Start()
    {
        AssignCanvases();
        if (photonView.IsMine)
        {
            SetLocalActiveCharacter(0);
        }
    }

    private void AssignCanvases()
    {
        characterCanvases = new Canvas[2];
        characterCanvases[0] = GameObject.Find("EzCanvas").GetComponent<Canvas>();
        characterCanvases[1] = GameObject.Find("GaCanvas").GetComponent<Canvas>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetLocalActiveCharacter(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetLocalActiveCharacter(1);
        }
    }

    private void SetLocalActiveCharacter(int index)
    {
        activeCharacterIndex = index;
        UpdateLocalCharacterVisibility();
        photonView.RPC("SyncCharacterVisibility", RpcTarget.Others, index);
    }

    private void UpdateLocalCharacterVisibility()
    {
        for (int i = 0; i < characterCanvases.Length; i++)
        {
            characterCanvases[i].gameObject.SetActive(i == activeCharacterIndex);
        }
        if (activeCharacterIndex == 2)
        {
            foreach (var canvas in characterCanvases)
            {
                canvas.gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == activeCharacterIndex);
        }
    }

    [PunRPC]
    private void SyncCharacterVisibility(int index)
    {
        if (!photonView.IsMine)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].SetActive(i == index);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (photonView.IsMine)
        {
            photonView.RPC("SyncCharacterVisibility", newPlayer, activeCharacterIndex);
        }
    }
}