using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // 회전 속도 (초당 360도 기준)

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
            // 카메라 방향을 기준으로 이동 방향 계산
            Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Quaternion cameraRotation = Quaternion.LookRotation(cameraForward);
            movement = cameraRotation * movement;

            // 캐릭터 이동
            transform.position += movement * moveSpeed * Time.deltaTime;

            // 캐릭터가 이동하는 방향으로 회전
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
