using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GaCharacterSkill : MonoBehaviourPunCallbacks
{
    private Image img_q;  // Image_1 (Q 스킬)
    private Image img_e;  // Image_2 (E 스킬)
    private Image img_r;  // Image_3 (R 스킬)

    private void Start()
    {
        // 캔버스에서 이미지를 자동으로 할당
        AssignSkillImages();

        // 자신의 캐릭터에 대해서만 스킬 UI 초기화
        if (photonView.IsMine)
        {
            ResetSkillFills();  // 스킬 Fill 초기화
        }
    }

    private void AssignSkillImages()
    {
        // GaCanvas 내에 있는 img_q, img_e, img_Gr 각각의 Image 컴포넌트를 찾아 할당
        img_q = GameObject.Find("img_q/Image_1").GetComponent<Image>();
        img_e = GameObject.Find("img_e/Image_2").GetComponent<Image>();
        img_r = GameObject.Find("img_Gr/Image_3").GetComponent<Image>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Q 스킬을 사용할 때 img_q의 fill amount를 1로 설정
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UseSkill(img_q);
            }

            // E 스킬을 사용할 때 img_e의 fill amount를 1로 설정
            if (Input.GetKeyDown(KeyCode.E))
            {
                UseSkill(img_e);
            }

            // R 스킬을 사용할 때 img_r의 fill amount를 1로 설정
            if (Input.GetKeyDown(KeyCode.R))
            {
                UseSkill(img_r);
            }
        }
    }

    private void UseSkill(Image skillImage)
    {
        // 스킬을 사용할 때 FillAmount를 1로 설정하고 서서히 감소시키기 시작
        skillImage.fillAmount = 1;
        StartCoroutine(ReduceFillOverTime(skillImage));
    }

    private IEnumerator ReduceFillOverTime(Image skillImage)
    {
        // 스킬 사용 후 일정 시간 동안 fillAmount가 서서히 감소하는 로직
        while (skillImage.fillAmount > 0)
        {
            skillImage.fillAmount -= Time.deltaTime / 5; // 5초 동안 감소
            yield return null;
        }
    }

    private void ResetSkillFills()
    {
        // 초기 스킬 상태 설정 (모든 스킬의 fill amount를 0으로 초기화)
        img_q.fillAmount = 0;
        img_e.fillAmount = 0;
        img_r.fillAmount = 0;
    }

}
