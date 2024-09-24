using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GaCharacterSkill : MonoBehaviourPunCallbacks
{
    private Image img_q;  // Image_1 (Q ��ų)
    private Image img_e;  // Image_2 (E ��ų)
    private Image img_r;  // Image_3 (R ��ų)

    private void Start()
    {
        // ĵ�������� �̹����� �ڵ����� �Ҵ�
        AssignSkillImages();

        // �ڽ��� ĳ���Ϳ� ���ؼ��� ��ų UI �ʱ�ȭ
        if (photonView.IsMine)
        {
            ResetSkillFills();  // ��ų Fill �ʱ�ȭ
        }
    }

    private void AssignSkillImages()
    {
        // GaCanvas ���� �ִ� img_q, img_e, img_Gr ������ Image ������Ʈ�� ã�� �Ҵ�
        img_q = GameObject.Find("img_q/Image_1").GetComponent<Image>();
        img_e = GameObject.Find("img_e/Image_2").GetComponent<Image>();
        img_r = GameObject.Find("img_Gr/Image_3").GetComponent<Image>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Q ��ų�� ����� �� img_q�� fill amount�� 1�� ����
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UseSkill(img_q);
            }

            // E ��ų�� ����� �� img_e�� fill amount�� 1�� ����
            if (Input.GetKeyDown(KeyCode.E))
            {
                UseSkill(img_e);
            }

            // R ��ų�� ����� �� img_r�� fill amount�� 1�� ����
            if (Input.GetKeyDown(KeyCode.R))
            {
                UseSkill(img_r);
            }
        }
    }

    private void UseSkill(Image skillImage)
    {
        // ��ų�� ����� �� FillAmount�� 1�� �����ϰ� ������ ���ҽ�Ű�� ����
        skillImage.fillAmount = 1;
        StartCoroutine(ReduceFillOverTime(skillImage));
    }

    private IEnumerator ReduceFillOverTime(Image skillImage)
    {
        // ��ų ��� �� ���� �ð� ���� fillAmount�� ������ �����ϴ� ����
        while (skillImage.fillAmount > 0)
        {
            skillImage.fillAmount -= Time.deltaTime / 5; // 5�� ���� ����
            yield return null;
        }
    }

    private void ResetSkillFills()
    {
        // �ʱ� ��ų ���� ���� (��� ��ų�� fill amount�� 0���� �ʱ�ȭ)
        img_q.fillAmount = 0;
        img_e.fillAmount = 0;
        img_r.fillAmount = 0;
    }

}
