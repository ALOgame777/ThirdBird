using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Motion_EX : MonoBehaviour
{
    // UDPserver
    public UDPServer udpServer;

    [System.Serializable]
    public class JointPoint
    {
        public Vector3 Pos3D = new Vector3();  // 월드 3D 공간에서의 관절 위치
        public float score3D;  // 3D 위치 추정의 신뢰도

        // Bones
        public Transform Transform;
        // 객체가 처음 생성될 때의 회전 상태를 저장할 변수
        public Quaternion InitRotation;
        // InitRotation의 역 회전, ex) 두 회전 간의 상대적인 회전이나, 특정 회전을 반대로 적용
        public Quaternion Inverse;
        // 초기에는 자식 객체를 null로 하여 아무것도 참조하지 않도록 함(나중에 관절의 계층 구조을 새로 할당해서 사용)
        public JointPoint Child = null;
    }

    [SerializeField]
    // Joint position and bone
    private JointPoint[] jointPoints;

    private Vector3 initPosition; // 객체의 초기 중심 위치를 저장하는 변수

    private Quaternion InitGazeRotation;
    private Quaternion gazeInverse;

    // 캐릭터
    public GameObject UnityChan; // 캐릭터 오브젝트
    public GameObject nose; // 캐릭터 코(머리 회전과 얼굴 방향 정확성 항샹을 위해)
    public Animator anim; // 캐릭터 애니메이터

    void Start()
    {
        // PositionIndex 수 만큼의 배열 자리를 확보한다.
        jointPoints = new JointPoint[PositionIndex.Count.Int()];

        // PositionIndex의 값 -1 만큼 반복하여, jointPoints의 i번째에 new JointPoint()를 할당한다.
        for (var i = 0; i < PositionIndex.Count.Int(); i++) jointPoints[i] = new JointPoint();

        // 애님에 캐릭터의 애니메이터를 할당한다.
        anim = UnityChan.GetComponent<Animator>();
        Init();
    }


    //  jointPoints 배열의 각 요소에 적절한 본의 트랜스폼을 할당, 
    void Init()
    {
        // 오른쪽 팔
        jointPoints[PositionIndex.right_shoulder.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        jointPoints[PositionIndex.right_elbow.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        jointPoints[PositionIndex.right_wrist.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightHand);
        jointPoints[PositionIndex.right_thumb.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
        jointPoints[PositionIndex.right_index.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        // 왼쪽 팔
        jointPoints[PositionIndex.left_shoulder.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        jointPoints[PositionIndex.left_elbow.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        jointPoints[PositionIndex.left_wrist.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        jointPoints[PositionIndex.left_thumb.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
        jointPoints[PositionIndex.left_index.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);

        // 얼굴
        jointPoints[PositionIndex.left_ear.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        jointPoints[PositionIndex.left_eye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftEye);
        jointPoints[PositionIndex.right_ear.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        jointPoints[PositionIndex.right_eye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightEye);
        jointPoints[PositionIndex.nose.Int()].Transform = nose.transform;

        //// 오른쪽 다리
        //jointPoints[PositionIndex.right_hip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        //jointPoints[PositionIndex.right_knee.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        //jointPoints[PositionIndex.right_ankle.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        //jointPoints[PositionIndex.right_foot_index.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightToes);

        //// 왼쪽 다리
        //jointPoints[PositionIndex.left_hip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        //jointPoints[PositionIndex.left_knee.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        //jointPoints[PositionIndex.left_ankle.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        //jointPoints[PositionIndex.left_foot_index.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftToes);

        // 그 외 넥, 헤드, 스파인은 필요할거같은데 음 새로 구해서 넣어야하나
        //jointPoints[PositionIndex.abdomenUpper.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Spine);
        //jointPoints[PositionIndex.hip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Hips);
        //jointPoints[PositionIndex.head.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        //jointPoints[PositionIndex.neck.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Neck);
        //jointPoints[PositionIndex.spine.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Spine);


        // 본들 간의 부모-자식 관계를 설정하여 캐릭터의 본 구조를 정의
        // 오른쪽 팔
        jointPoints[PositionIndex.right_shoulder.Int()].Child = jointPoints[PositionIndex.right_elbow.Int()];
        jointPoints[PositionIndex.right_elbow.Int()].Child = jointPoints[PositionIndex.right_wrist.Int()];

        // 왼쪽 팔
        jointPoints[PositionIndex.left_shoulder.Int()].Child = jointPoints[PositionIndex.left_elbow.Int()];
        jointPoints[PositionIndex.left_elbow.Int()].Child = jointPoints[PositionIndex.left_wrist.Int()];



        //// 오른쪽 다리
        //jointPoints[PositionIndex.right_hip.Int()].Child = jointPoints[PositionIndex.right_knee.Int()];
        //jointPoints[PositionIndex.right_knee.Int()].Child = jointPoints[PositionIndex.right_ankle.Int()];
        //jointPoints[PositionIndex.right_ankle.Int()].Child = jointPoints[PositionIndex.right_foot_index.Int()];

        //// 왼쪽 다리
        //jointPoints[PositionIndex.left_hip.Int()].Child = jointPoints[PositionIndex.left_knee.Int()];
        //jointPoints[PositionIndex.left_knee.Int()].Child = jointPoints[PositionIndex.left_ankle.Int()];
        //jointPoints[PositionIndex.left_ankle.Int()].Child = jointPoints[PositionIndex.left_foot_index.Int()];

        // 그 외
        //jointPoints[PositionIndex.spine.Int()].Child = jointPoints[PositionIndex.neck.Int()];
        //jointPoints[PositionIndex.neck.Int()].Child = jointPoints[PositionIndex.head.Int()];
        //jointPoints[PositionIndex.head.Int()].Child = jointPoints[PositionIndex.nose.Int()];


        // Set Inverse
        //  jointPoints 컬렉션 안에 있는 각각의 jointPoint 객체에 대하여 순회한다.
        // InitRotation 값을 jointPoint.Transform.rotation(초기 회전값)으로 저장
        foreach (var jointPoint in jointPoints)
        {
            // 예외처리
            if (jointPoint.Transform != null)
            {
                jointPoint.InitRotation = jointPoint.Transform.rotation;
            }
            // 예외처리
            if (jointPoint.Child != null)
            {
                // Inverse 값을 함수 GetInverse(jointPoint, jointPoint.Child)의 결과로 설정하여 역변환 값을 저장
                jointPoint.Inverse = GetInverse(jointPoint, jointPoint.Child);
            }
        }

        //// 객체의 초기 중심 위치를 엉덩이 본의 위치로 저장한다.
        //initPosition = jointPoints[PositionIndex.hip.Int()].Transform.position;

        //// 엉덩이 본, 왼쪽허벅지, 오른쪽허벅지로 계산한 삼각형 면의 법선벡터를 forward에 저장한다.
        //var forward = TriangleNormal(jointPoints[PositionIndex.hip.Int()].Transform.position, jointPoints[PositionIndex.left_hip.Int()].Transform.position, jointPoints[PositionIndex.right_hip.Int()].Transform.position);
        //// 엉덩이 본이 forward 방향을 바라보도록 회전시킨 후, 그 회전의 역(반대) 회전을 저장한다.
        //jointPoints[PositionIndex.hip.Int()].Inverse = Quaternion.Inverse(Quaternion.LookRotation(forward));


        // 머리 회전
        //jointPoints[PositionIndex.head.Int()].InitRotation = jointPoints[PositionIndex.head.Int()].Transform.rotation;
        //var gaze = jointPoints[PositionIndex.nose.Int()].Transform.position - jointPoints[PositionIndex.head.Int()].Transform.position;
        //jointPoints[PositionIndex.head.Int()].Inverse = Quaternion.Inverse(Quaternion.LookRotation(gaze));

        //jointPoints[PositionIndex.left_wrist.Int()].InitRotation = jointPoints[PositionIndex.left_wrist.Int()].Transform.rotation;
        //jointPoints[PositionIndex.left_wrist.Int()].Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.left_thumb.Int()].Transform.position - jointPoints[PositionIndex.left_index.Int()].Transform.position));

        //jointPoints[PositionIndex.right_wrist.Int()].InitRotation = jointPoints[PositionIndex.right_wrist.Int()].Transform.rotation;
        //jointPoints[PositionIndex.right_wrist.Int()].Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.right_thumb.Int()].Transform.position - jointPoints[PositionIndex.rMid1.Int()].Transform.position));
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
    private Quaternion GetInverse(JointPoint p1, JointPoint p2)
    {
        // p1 - p2 계산하여, p1의 위치를 기준으로 p2를 바라보는 방향의 회전값을 구한 후,
        // 그 회전값의 역회전 값을 반환한다.
        return Quaternion.Inverse(Quaternion.LookRotation(p1.Transform.position - p2.Transform.position));
    }

    void Update()
    {
        // 모든 LandMarkData를 담을 변수
        LandMarkData landMark = udpServer.landMark;

        // 데이터가 들어있다면
        if (landMark.data.Count == (int)PositionIndex.Count)
        {
            // landMark 의 위치값을 point의 위치 값으로 설정
            for (int i = 0; i < jointPoints.Length; i++)
            {
                Vector3 pos = new Vector3(landMark.data[i].x, -landMark.data[i].y, landMark.data[i].z);
                jointPoints[i].Pos3D = pos;
            }
        }

        StartCoroutine("PoseUpdate");
    }

    // 캐릭터의 본 회전을 담당하는 함수
    IEnumerator PoseUpdate()
    {
        // 센터 이동 및 회전
        // forward 값을 저장한다.
        //var forward = // 벡터 a, b, c의 값으로 삼각형의 면을 만들고, 면이 바라보는 방향(법선 벡터) forward를 반환하는 함수
        //    TriangleNormal(jointPoints[PositionIndex.hip.Int()].Pos3D, jointPoints[PositionIndex.left_hip.Int()].Pos3D, jointPoints[PositionIndex.right_hip.Int()].Pos3D);

        //// 엉덩이 본의 위치값은 
        //jointPoints[PositionIndex.hip.Int()].Transform.position =
        //    // 엉덩이 본의 월드 3D좌표계의 값에 0.01을 곱해 값을 스케일링한 후,
        //    jointPoints[PositionIndex.hip.Int()].Pos3D * 0.01f
        //    // 그 결과에 객체의 초기 위치값의 x, z 좌표를 적용하여 최종 위치를 대입한다.
        //    + new Vector3(initPosition.x, 0f, initPosition.z);

        //// 엉덩이 본의 회전값은 
        //jointPoints[PositionIndex.hip.Int()].Transform.rotation =
        //    // 엉덩이 본이 forward 방향으로, 정면으로 향하도록 설정한다. 
        //    Quaternion.LookRotation(forward)
        //    // 현재 본의 회전값의 역방향을 곱하여 현재 본의 회전 상태를 초기 상태로 되돌린다.
        //    * jointPoints[PositionIndex.hip.Int()].Inverse
        //    // 본의 초기 회전값을 곱하여 최종 회전값을 결정.
        //    * jointPoints[PositionIndex.hip.Int()].InitRotation;
        //// 엉덩이 본이 가진 정면 방향 forward 과 월드좌표계에서의 회전 방향이 다르기 때문에
        //// 엉덩이 본이 정면을 바라보면서 월드좌표계에서 회전을 할 수 있게 하기 위해서 이런 방법을 사용함


        // 각 뼈의 회전 
        // jointPoints 컬렉션 안에 있는 각각의 jointPoint 객체에 대하여 순회한다.
        foreach (var jointPoint in jointPoints)
        {
            // 예외처리
            if (jointPoint.Child != null)
            {
                //// jointPoint의 회전(각도)값은 
                //jointPoint.Transform.rotation =
                //// 엉덩이 본이 jointPoint.Child를 바라보는 방향으로 회전, 이 방향을 forward 벡터로 맞추는 회전
                //Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, forward)
                //// 엉덩이 본의 현재 회전 상태(jointPoint.Inverse)를 제거하여, 기본적으로 설정된 회전 상태를 적용
                //* jointPoint.Inverse
                //// 초기 회전값을 곱하고 최종적으로 계산된 회전값 방향으로 회전한다.
                // * jointPoint.InitRotation;
            }
        }

        //// 머리 회전 구하기
        //// gaze는 nose를 기준으로 head를 바라보는 (월드)Vector3 Pos3D값이다.
        //var gaze = jointPoints[PositionIndex.nose.Int()].Pos3D - jointPoints[PositionIndex.head.Int()].Pos3D;
        //// f는 코, 왼쪽귀, 오른쪽귀로 이루어진 삼각형의 수직벡터다.
        //var f = TriangleNormal(jointPoints[PositionIndex.nose.Int()].Pos3D, jointPoints[PositionIndex.right_ear.Int()].Pos3D, jointPoints[PositionIndex.left_ear.Int()].Pos3D);
        //// head에 머리에 해당하는 jointPoints 값 할당
        //var head = jointPoints[PositionIndex.head.Int()];
        //// 머리의 회전(각도)를
        //// gaze 벡터가 객체의 전면을 향하고, f 벡터가 객체의 상단을 향하도록 회전하고,
        //// 역방향을 곱해 회전값을 초기화하고, 초기 회전값을 곱해 최종적으로 계산된 회전 값 방향으로 회전한다.
        //head.Transform.rotation = Quaternion.LookRotation(gaze, f) * head.Inverse * head.InitRotation;


        // 손목회전 (테스트 코드) 방식은 같은 듯
        var lf = TriangleNormal(jointPoints[PositionIndex.left_wrist.Int()].Pos3D, jointPoints[PositionIndex.left_index.Int()].Pos3D, jointPoints[PositionIndex.left_thumb.Int()].Pos3D);
        var left_wrist = jointPoints[PositionIndex.left_wrist.Int()];
        //left_wrist.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.left_thumb.Int()].Pos3D - jointPoints[PositionIndex.left_index.Int()].Pos3D, lf) * left_wrist.Inverse * left_wrist.InitRotation;
        //var rf = TriangleNormal(jointPoints[PositionIndex.right_wrist.Int()].Pos3D, jointPoints[PositionIndex.right_thumb.Int()].Pos3D, jointPoints[PositionIndex.rMid1.Int()].Pos3D);
        var right_wrist = jointPoints[PositionIndex.right_wrist.Int()];
        //right_wrist.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.right_thumb.Int()].Pos3D - jointPoints[PositionIndex.rMid1.Int()].Pos3D, rf) * right_wrist.Inverse * right_wrist.InitRotation;

        yield return null;
    }
}
