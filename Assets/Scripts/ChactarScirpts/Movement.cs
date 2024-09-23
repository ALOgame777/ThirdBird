using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // ȸ�� �ӵ� (�ʴ� 360�� ����)

    private Transform cameraTransform;

    void Start()
    {
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (movement.magnitude >= 0.1f)
        {
            // ī�޶� ������ �������� �̵� ���� ���
            Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Quaternion cameraRotation = Quaternion.LookRotation(cameraForward);
            movement = cameraRotation * movement;

            // ĳ���� �̵�
            transform.position += movement * moveSpeed * Time.deltaTime;

            // ĳ���Ͱ� �̵��ϴ� �������� ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
