using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamearaMovement : MonoBehaviour
{
    public Transform objectToFollow; // 카메라가 따라갈 대상
    public float followSpeed = 10.0f; // 따라가는 속도
    public float sensitivity = 100.0f; // 마우스 감도
    public float clampAngle = 70.0f; // 각도 제한
    public float smoothness = 10.0f; // 카메라의 부드러움
    //Time.deltaTime에 곱해져 값이 작을수록 부드럽게, 값이 클수록 빠르게(거칠게)이동함


    private float rotX; // 마우스 인풋을 받는 변수
    private float rotY; // 마우스 인풋을 받는 변수

    public Transform realCamera; // 사용할 카메라
    public Vector3 dirNormalrized; // 방향 
    public Vector3 finaDir; // 최종적으로 정해질 방향
    public float minDistance; // 카메라가 비추는 최소거리
    public float maxDistance; // 카메라가 비추는 최대거리
    public float finalDistance; // 최종적으로 결정된 카메라와 대상 간 거리
    // 방해물이 있을 시에 카메라가 이동해서 위치하게 될 거리


    void Start()
    {
        // 마우스 회전값을 초기화 시켜준다.
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;
        // 벡터값 초기화 시켜준다.
        dirNormalrized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;
        // 커서를 플레이 화면에 잠궈준다.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;




    }


    void Update()
    {   // 매 프레임마다 마우스의 값을 받아와 감도와 시간을 곱해준다.
        // Time.deltaTime 마지막 프레임에서 현재 프레임까지의 시간간격
        // 화면상의 회전축 XY와 마우스 XY는 반대임
        rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        // 화면이 넘어가지 않게 각도값에 제한(Clamp)을 걸어준다.
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        // 회전값을 저장할 그릇 rot에 마우스회전값 Quaternion.Euler(rotX, rotY, 0)을 넣는다. 마우스여서 z는 없음(=0)
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }

    void LateUpdate() // Update가 끝난 후 동작하는 생애주기 함수
    {
        // 따라가도록 이동(=position의 값을 바꾼다.)
        // MoveTowards 함수: 두 점 사이를 일정한 속도로 이동시키는 데 사용. 
        // 벡터를 일정 방향으로 일정 거리만큼 이동시키고자 할 때
        // 현재(카메라) 위치에서 -> 따라갈 대상의 위치로 , 속력(=속도*시간)
        transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position, followSpeed * Time.deltaTime);


        // 최종적으로 방향을 정해준다.
        // dirNormalrized는 위에서 카메라 자신의 로컬 좌표로 초기화 시켜뒀었음
        // TransformPoint = 객체의 로컬 좌표계를 월드 좌표계로 변환(포지션 값을 필요로함)
        finaDir = transform.TransformPoint(dirNormalrized * maxDistance);

        // 대상과 카메라 사이에 방해물이 존재하는 지 검사
        // 만일 Raycast(시작점, 끝점)에 방해물이 부딪힌다면
        RaycastHit hit;
        if (Physics.Linecast(transform.position, finaDir, out hit, ~(1 << 11)))
        {
            // 방해물이 있다면, finalDistance(카메라와 대상간 최종거리)는
            // hit.distance(Ray와 충돌한 객체까지의 거리)를 minDistance와 maxDistance 사이의 값으로 제한한다.
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else // 그렇지 않고, 부딪히지 않았다면
        {
            // 최종 거리는 최대 거리다,
            finalDistance = maxDistance;
        }
        // 카메라의 로컬위치는
        // 카메라의 로컬위치와
        // Vector3.Lerp(a, b, t): a와 b 사이를 t 비율만큼 보간하는 함수. 
        // t는 0과 1 사이의 값으로, 0은 a, 1은 b를 나타내며, 그 사이의 값은 두 점 사이의 중간값을 반환
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalrized * finalDistance, Time.deltaTime * smoothness);
    }
}
// 카메라를 따라가게 하는 코드: transform.position = Vector3.MoveTowards(...)는 카메라가 대상 오브젝트를 따라가도록 합니다.
// 카메라의 위치 조정: realCamera.localPosition = Vector3.Lerp(...)는 카메라의 최종 위치를 부드럽게 조정하여 장애물에 의해 최적의 위치를 유지하도록 합니다.