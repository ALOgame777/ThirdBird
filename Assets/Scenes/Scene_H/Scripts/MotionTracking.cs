using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Motion_EX;


public class MotionTracking : MonoBehaviour
{
    // UDPserver
    public UDPServer udpServer;

    [System.Serializable]
    public class LandMarkInfo
    {
        public Vector3 mediaPipePos = new Vector3();  // 월드 3D 공간에서의 관절 위치
        public float score3D;  // 3D 위치 추정의 신뢰도

        // Bones
        public Transform Transform;
        // 처음 생성될 때의 초기 회전 상태
        public Quaternion InitRotation;
        // 역회전
        public Quaternion Inverse;
        // 자식 객체를 초기화
        public LandMarkInfo Child = null;
    }

    [SerializeField]
    // 랜드마크 정보 및 본
    public LandMarkInfo[] landMarkInfos;

    public Vector3 initPosition; // 객체의 초기 중심 위치를 저장하는 변수

    private Quaternion InitGazeRotation;
    private Quaternion gazeInverse;

    // 캐릭터
    public GameObject character; // 캐릭터 오브젝트
    public GameObject nose; // 캐릭터 코(머리 회전과 얼굴 방향 정확성 항샹을 위해)
    public Animator anim; // 캐릭터 애니메이터

    void Start()
    {
        // PositionIndex 수 만큼의 배열 자리를 확보한다.
        landMarkInfos = new LandMarkInfo[PositionIndex.Count.Int()];

        // PositionIndex의 값 -1 만큼 반복하여, landMarkInfos의 i번째에 new JointPoint()를 할당한다.
        for (var i = 0; i < PositionIndex.Count.Int(); i++) landMarkInfos[i] = new LandMarkInfo();

        // 애님에 캐릭터의 애니메이터를 할당한다.
        anim = character.GetComponent<Animator>();
        Init();
    }
    //  landMarkInfos 배열의 각 요소에 적절한 본의 트랜스폼을 할당, 
    void Init()
    {
        // 오른쪽 팔
        landMarkInfos[PositionIndex.right_shoulder.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        landMarkInfos[PositionIndex.right_elbow.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        landMarkInfos[PositionIndex.right_wrist.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightHand);
        landMarkInfos[PositionIndex.right_thumb.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
        landMarkInfos[PositionIndex.right_index.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        // 왼쪽 팔
        landMarkInfos[PositionIndex.left_shoulder.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        landMarkInfos[PositionIndex.left_elbow.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        landMarkInfos[PositionIndex.left_wrist.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        landMarkInfos[PositionIndex.left_thumb.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
        landMarkInfos[PositionIndex.left_index.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);

        // 얼굴
        landMarkInfos[PositionIndex.left_ear.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        landMarkInfos[PositionIndex.left_eye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftEye);
        landMarkInfos[PositionIndex.right_ear.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        landMarkInfos[PositionIndex.right_eye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightEye);
        landMarkInfos[PositionIndex.nose.Int()].Transform = nose.transform;

        // 머리랑 목
        landMarkInfos[PositionIndex.head.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        landMarkInfos[PositionIndex.neck.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Neck);


        // 본들 간의 부모-자식 관계를 설정하여 캐릭터의 본 구조를 정의
        // 오른쪽 팔
        landMarkInfos[PositionIndex.right_shoulder.Int()].Child = landMarkInfos[PositionIndex.right_elbow.Int()];
        landMarkInfos[PositionIndex.right_elbow.Int()].Child = landMarkInfos[PositionIndex.right_wrist.Int()];

        // 왼쪽 팔
        landMarkInfos[PositionIndex.left_shoulder.Int()].Child = landMarkInfos[PositionIndex.left_elbow.Int()];
        landMarkInfos[PositionIndex.left_elbow.Int()].Child = landMarkInfos[PositionIndex.left_wrist.Int()];

        // 역회전 세팅
        //  landMarkInfos 컬렉션 안에 있는 각각의 jointPoint 객체에 대하여 순회한다.
        // InitRotation 값을 jointPoint.Transform.rotation(초기 회전값)으로 저장

        foreach (var landMark in landMarkInfos)
        {
            // 예외처리
            if (landMark.Transform != null)
            {
                landMark.InitRotation = landMark.Transform.rotation;
            }
            // 예외처리
            if (landMark.Child != null)
            {
                // Inverse 값을 함수 GetInverse(jointPoint, jointPoint.Child)의 결과로 설정하여 역변환 값을 저장
                landMark.Inverse = GetInverse(landMark, landMark.Child);
            }

        }

        // 머리 회전
        landMarkInfos[PositionIndex.head.Int()].InitRotation = landMarkInfos[PositionIndex.head.Int()].Transform.rotation;
        var gaze = landMarkInfos[PositionIndex.nose.Int()].Transform.position - landMarkInfos[PositionIndex.head.Int()].Transform.position;
        landMarkInfos[PositionIndex.head.Int()].Inverse = Quaternion.Inverse(Quaternion.LookRotation(gaze));

        landMarkInfos[PositionIndex.left_wrist.Int()].InitRotation = landMarkInfos[PositionIndex.left_wrist.Int()].Transform.rotation;
        landMarkInfos[PositionIndex.left_wrist.Int()].Inverse = Quaternion.Inverse(Quaternion.LookRotation(landMarkInfos[PositionIndex.left_thumb.Int()].Transform.position - landMarkInfos[PositionIndex.left_index.Int()].Transform.position));

        landMarkInfos[PositionIndex.right_wrist.Int()].InitRotation = landMarkInfos[PositionIndex.right_wrist.Int()].Transform.rotation;
        landMarkInfos[PositionIndex.right_wrist.Int()].Inverse = Quaternion.Inverse(Quaternion.LookRotation(landMarkInfos[PositionIndex.right_thumb.Int()].Transform.position - landMarkInfos[PositionIndex.right_pinky.Int()].Transform.position));
    }

    // 벡터 a, b, c의 값으로 삼각형의 면을 만들고, 면이 바라보는 방향(법선 벡터) forward를 반환하는 함수
    Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        // 벡터 d1, d2를 구한다.
        Vector3 d1 = a - b;
        Vector3 d2 = a - c;

        // Cross는 벡터의 외적(cross product)을 계산하는 함수
        Vector3 dd = Vector3.Cross(d1, d2);
        // 벡터 dd는 벡터 d1과 d2가 이루는 평면에 수직인 벡터,
        // d1, d2벡터로 그려진 삼각형의 면이 어느 방향을 향하고 있는 지 나타냄
        dd.Normalize(); // 정규화

        return dd; // dd값을 반환
    }

    // Quaternion 자료형의 GetInverse(역회전을 구하는) 함수
    // 매개변수 자료형은 클래스 JointPoint으로 JointPoint의 인스턴스를 모두 참조 할 수 있다.
    private Quaternion GetInverse(LandMarkInfo p1, LandMarkInfo p2)
    {
        // p1 - p2 계산하여, p1의 위치를 기준으로 p2를 바라보는 방향의 회전값을 구한 후,
        // 그 회전값의 역회전 값을 반환한다.
        return Quaternion.Inverse(Quaternion.LookRotation(p1.Transform.position - p2.Transform.position));
    }

    void Update()
    {

        StartCoroutine("PoseUpdate");
    }

    // 캐릭터의 본 회전을 담당하는 함수
    IEnumerator PoseUpdate()
    {
        // 센터 이동 및 회전

        // 벡터 a, b, c의 값으로 삼각형의 면을 만들고, 면이 바라보는 방향(법선 벡터) forward를 반환하는 함수
        var forward =
        TriangleNormal(landMarkInfos[PositionIndex.hip.Int()].mediaPipePos, landMarkInfos[PositionIndex.left_hip.Int()].mediaPipePos, landMarkInfos[PositionIndex.right_hip.Int()].mediaPipePos);

        // 엉덩이 본의 위치값은 
        landMarkInfos[PositionIndex.hip.Int()].Transform.position =
            // 엉덩이 본의 월드 3D좌표계의 값에 0.01을 곱해 값을 스케일링한 후,
            landMarkInfos[PositionIndex.hip.Int()].mediaPipePos * 0.01f
            // 그 결과에 객체의 초기 위치값의 x, z 좌표를 적용하여 최종 위치를 대입한다.
            + new Vector3(initPosition.x, 0f, initPosition.z);

        // 엉덩이 본의 회전값은 
        landMarkInfos[PositionIndex.hip.Int()].Transform.rotation =
            // 엉덩이 본이 forward 방향으로, 정면으로 향하도록 설정한다. 
            Quaternion.LookRotation(forward)
            // 현재 본의 회전값의 역방향을 곱하여 현재 본의 회전 상태를 초기 상태로 되돌린다.
            * landMarkInfos[PositionIndex.hip.Int()].Inverse
            // 본의 초기 회전값을 곱하여 최종 회전값을 결정.
            * landMarkInfos[PositionIndex.hip.Int()].InitRotation;
        // 엉덩이 본이 가진 정면 방향 forward 과 월드좌표계에서의 회전 방향이 다르기 때문에
        // 엉덩이 본이 정면을 바라보면서 월드좌표계에서 회전을 할 수 있게 하기 위해서 이런 방법을 사용함


        // 각 뼈의 회전 
        // landMarkInfos 컬렉션 안에 있는 각각의 jointPoint 객체에 대하여 순회한다.
        foreach (var jointPoint in landMarkInfos)
        {
            // 예외처리
            if (jointPoint.Child != null)
            {
                // jointPoint의 회전(각도)값은 
                jointPoint.Transform.rotation =
                // 엉덩이 본이 jointPoint.Child를 바라보는 방향으로 회전, 이 방향을 forward 벡터로 맞추는 회전
                Quaternion.LookRotation(jointPoint.mediaPipePos - jointPoint.Child.mediaPipePos, forward)
                // 엉덩이 본의 현재 회전 상태(jointPoint.Inverse)를 제거하여, 기본적으로 설정된 회전 상태를 적용
                * jointPoint.Inverse
                // 초기 회전값을 곱하고 최종적으로 계산된 회전값 방향으로 회전한다.
                 * jointPoint.InitRotation;
            }
        }

        // 머리 회전 구하기
        // gaze는 nose를 기준으로 head를 바라보는 (월드)Vector3 mediaPipePos값이다.
        var gaze = landMarkInfos[PositionIndex.nose.Int()].mediaPipePos - landMarkInfos[PositionIndex.head.Int()].mediaPipePos;
        // f는 코, 왼쪽귀, 오른쪽귀로 이루어진 삼각형의 수직벡터다.
        var f = TriangleNormal(landMarkInfos[PositionIndex.nose.Int()].mediaPipePos, landMarkInfos[PositionIndex.right_ear.Int()].mediaPipePos, landMarkInfos[PositionIndex.left_ear.Int()].mediaPipePos);
        // head에 머리에 해당하는 landMarkInfos 값 할당
        var head = landMarkInfos[PositionIndex.head.Int()];
        // 머리의 회전(각도)를
        // gaze 벡터가 객체의 전면을 향하고, f 벡터가 객체의 상단을 향하도록 회전하고,
        // 역방향을 곱해 회전값을 초기화하고, 초기 회전값을 곱해 최종적으로 계산된 회전 값 방향으로 회전한다.
        head.Transform.rotation = Quaternion.LookRotation(gaze, f) * head.Inverse * head.InitRotation;


        // 손목회전 (테스트 코드) 방식은 같은 듯
        var lf = TriangleNormal(landMarkInfos[PositionIndex.left_wrist.Int()].mediaPipePos, landMarkInfos[PositionIndex.left_index.Int()].mediaPipePos, landMarkInfos[PositionIndex.left_thumb.Int()].mediaPipePos);
        var left_wrist = landMarkInfos[PositionIndex.left_wrist.Int()];
        left_wrist.Transform.rotation = Quaternion.LookRotation(landMarkInfos[PositionIndex.left_thumb.Int()].mediaPipePos - landMarkInfos[PositionIndex.left_index.Int()].mediaPipePos, lf) * left_wrist.Inverse * left_wrist.InitRotation;
        var rf = TriangleNormal(landMarkInfos[PositionIndex.right_wrist.Int()].mediaPipePos, landMarkInfos[PositionIndex.right_thumb.Int()].mediaPipePos, landMarkInfos[PositionIndex.right_pinky.Int()].mediaPipePos);
        var right_wrist = landMarkInfos[PositionIndex.right_wrist.Int()];
        right_wrist.Transform.rotation = Quaternion.LookRotation(landMarkInfos[PositionIndex.right_thumb.Int()].mediaPipePos - landMarkInfos[PositionIndex.right_pinky.Int()].mediaPipePos, rf) * right_wrist.Inverse * right_wrist.InitRotation;

        yield return null;
    }
}