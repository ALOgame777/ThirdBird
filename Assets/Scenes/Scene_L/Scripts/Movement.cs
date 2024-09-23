using Photon.Pun;
using UnityEngine;

public class Movement : MonoBehaviourPunCallbacks, IPunObservable
{
    public float rotateSpeedMovement = 0.1f;
    float rotateVelocity;
    public float moveSpeed = 5f;  // Speed for WASD movement

    // Variables for network synchronization
    Vector3 networkPosition;
    Quaternion networkRotation;
    float smoothing = 10f;

    // Cache whether this is the local player
    bool isLocalPlayer;

    private void Awake()
    {
       
    }

    void Start()
    {
       
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleWASDMovement();  // WASD movement handling
        }
        else
        {
            SmoothMovement();
        }
    }

    void HandleWASDMovement()
    {

        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;

        if (moveDirection != Vector3.zero)
        {
            // 이동 처리
            Vector3 targetPosition = transform.position + transform.TransformDirection(moveDirection.normalized) * moveSpeed * Time.deltaTime;
            transform.position = targetPosition;

            // 이동 방향으로 회전 처리
            RotateTowards(transform.position + moveDirection); // 이동하는 방향으로 캐릭터 회전
        }
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Y축 방향 회전 방지
        if (direction != Vector3.zero) // 방향이 0이 아닐 때만 회전
        {
            Quaternion rotationLookAt = Quaternion.LookRotation(direction);
            float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationLookAt.eulerAngles.y, ref rotateVelocity, rotateSpeedMovement * (Time.deltaTime * 5));
            transform.eulerAngles = new Vector3(0, rotationY, 0); // X, Z 축 회전을 방지하고 Y축만 회전
        }
    }

    // Smooth networked movement for other clients
    void SmoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * smoothing);
        transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * smoothing);  // Use Slerp for smoother rotations
    }

    // Photon synchronization of position and rotation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
