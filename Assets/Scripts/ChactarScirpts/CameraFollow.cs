using UnityEngine;
using Photon.Pun;

public class CameraFollow : MonoBehaviourPunCallbacks
{
    public float mouseSensitivity = 2f;
    public float distanceFromTarget = 3f;
    public float heightOffset = 1.5f;  // 캐릭터 위로 높이는 변수 추가
    public float pitchMin = -45f;
    public float pitchMax = 45f;

    private float pitch = 0f;
    private float yaw = 0f;

    private Transform targetTransform;
    private PrefabLoader prefabLoader;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        FindLocalPlayer();
    }

    void LateUpdate()
    {
        if (targetTransform == null)
        {
            FindLocalPlayer();
            return;
        }

        Transform activeCharacterTransform = GetActiveCharacterTransform();
        if (activeCharacterTransform != null)
        {
            // 마우스 오른쪽 버튼을 누르고 있을 때만 카메라 회전
            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
            }

            // 카메라 위치 및 회전 계산
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 negDistance = new Vector3(0f, 0f, -distanceFromTarget);
            Vector3 position = rotation * negDistance + activeCharacterTransform.position + Vector3.up * heightOffset;  // Y축으로 heightOffset 만큼 추가

            transform.rotation = rotation;
            transform.position = position;

            // 캐릭터를 카메라 방향으로 회전
            activeCharacterTransform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }

    void FindLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                targetTransform = player.transform;
                prefabLoader = player.GetComponent<PrefabLoader>();
                break;
            }
        }
    }

    Transform GetActiveCharacterTransform()
    {
        if (prefabLoader != null)
        {
            return prefabLoader.GetActiveCharacterTransform();
        }
        return targetTransform;
    }

    public override void OnJoinedRoom()
    {
        FindLocalPlayer();
    }
}
