//using OpenCvSharp;
//using OpenCvSharp.Dnn;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.Mathematics;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Video;

//public enum PositionIndex : int
//{
//    rShldrBend = 0,
//    rForearmBend,
//    rHand,
//    rThumb2,
//    rMid1,

//    lShldrBend,
//    lForearmBend,
//    lHand,
//    lThumb2,
//    lMid1,

//    lEar,
//    lEye,
//    rEar,
//    rEye,
//    Nose,

//    rThighBend,
//    rShin,
//    rFoot,
//    rToe,

//    lThighBend,
//    lShin,
//    lFoot,
//    lToe,

//    abdomenUpper,

//    // 계산된 좌표
//    hip,
//    head,
//    neck,
//    spine,

//    Count,
//    None,
//}
//// PositionIndex 열거형 값을 int로 변환하기 위한 static 클래스
//public static partial class EnumExtend
//{
//    public static int Int(this PositionIndex i)
//    {
//        return (int)i;
//    }
//}

//public class ThreeDPoseScript : MonoBehaviour
//{
//    public class JointPoint
//    {
//        public Vector2 Pos2D = new Vector2();  // 웹캠, 화면 같은 2D 공간에서의 관절 위치
//        public float score2D;  // 2D 위치 추정의 신뢰도

//        public Vector3 Pos3D = new Vector3();  // 월드 3D 공간에서의 관절 위치
//        public Vector3 Now3D = new Vector3();  // 현재 3D 위치
//        public Vector3 PrevPos3D = new Vector3();  // 이전 3D 위치
//        public float score3D;  // 3D 위치 추정의 신뢰도

//        // Bones
//        public Transform Transform = null;
//        // 객체가 처음 생성될 때의 회전 상태를 저장할 변수
//        public Quaternion InitRotation;
//        // InitRotation의 역 회전, ex) 두 회전 간의 상대적인 회전이나, 특정 회전을 반대로 적용
//        public Quaternion Inverse;
//        // 초기에는 자식 객체를 null로 하여 아무것도 참조하지 않도록 함(나중에 관절의 계층 구조을 새로 할당해서 사용)
//        public JointPoint Child = null;
//    }

//    // Joint position and bone
//    private JointPoint[] jointPoints;

//    private Vector3 initPosition; // 객체의 초기 중심 위치를 저장하는 변수

//    private Quaternion InitGazeRotation;
//    private Quaternion gazeInverse;

//    // 캐릭터
//    public GameObject UnityChan; // 캐릭터 오브젝트
//    public GameObject Nose; // 캐릭터 코(머리 회전과 얼굴 방향 정확성 항샹을 위해)
//    private Animator anim; // 캐릭터 애니메이터

//    // For camera play
//    public bool UseWebCam = true;
//    private WebCamTexture webCamTexture;

//    // For video play
//    private RenderTexture videoTexture;

//    private Texture2D texture;
//    private int videoScreenWidth = 2560;
//    private float videoWidth, videoHeight;
//    private UnityEngine.Rect clipRect;
//    public float clipScale;

//    // Properties for onnx and estimation
//    private Net Onnx;
//    private Mat[] outputs = new Mat[4];

//    public GameObject TextureObject;

//    private const int inputImageSize = 224;
//    private const int JointNum = 24;
//    private const int HeatMapCol = 14;
//    private const int HeatMapCol_Squared = 14 * 14;
//    private const int HeatMapCol_Cube = 14 * 14 * 14;

//    char[] heatMap2Dbuf = new char[JointNum * HeatMapCol_Squared * 4];
//    float[] heatMap2D = new float[JointNum * HeatMapCol_Squared];
//    char[] offset2Dbuf = new char[JointNum * HeatMapCol_Squared * 2 * 4];
//    float[] offset2D = new float[JointNum * HeatMapCol_Squared * 2];

//    char[] heatMap3Dbuf = new char[JointNum * HeatMapCol_Cube * 4];
//    float[] heatMap3D = new float[JointNum * HeatMapCol_Cube];
//    char[] offset3Dbuf = new char[JointNum * HeatMapCol_Cube * 3 * 4];
//    float[] offset3D = new float[JointNum * HeatMapCol_Cube * 3];

//    void Start()
//    {
//        // PositionIndex 수 만큼의 배열 자리를 확보한다.
//        jointPoints = new JointPoint[PositionIndex.Count.Int()];
//        // PositionIndex의 값 -1 만큼 반복하여, jointPoints의 i번째에 new JointPoint()를 할당한다.
//        for (var i = 0; i < PositionIndex.Count.Int(); i++) jointPoints[i] = new JointPoint();

//        // 애님에 캐릭터의 애니메이터를 할당한다.
//        anim = UnityChan.GetComponent<Animator>();

//        // 웹캠 사용 유무에 따른 플레이어 조정
//        if (UseWebCam)
//        {
//            CameraPlayStart();
//        }
//        else
//        {
//            VideoPlayStart();
//        }

//        // 영상 사이즈 조절
//        videoWidth = texture.width;
//        videoHeight = texture.height;
//        float padWidth = (videoWidth < videoHeight) ? 0 : (videoHeight - videoWidth) / 2;
//        float padHeight = (videoWidth < videoHeight) ? (videoWidth - videoHeight) / 2 : 0;
//        if (clipScale == 0f) clipScale = 0.001f;
//        var w = (videoWidth + padWidth * 2f) * clipScale;
//        padWidth += w;
//        padHeight += w;
//        clipRect = new UnityEngine.Rect(-padWidth, -padHeight, videoWidth + padWidth * 2, videoHeight + padHeight * 2);

//        InitONNX();
//        Init();
//    }

//    //  jointPoints 배열의 각 요소에 적절한 본의 트랜스폼을 할당, 
//    void Init()
//    {
//        // Right Arm
//        jointPoints[PositionIndex.rShldrBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
//        jointPoints[PositionIndex.rForearmBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
//        jointPoints[PositionIndex.rHand.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightHand);
//        jointPoints[PositionIndex.rThumb2.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
//        jointPoints[PositionIndex.rMid1.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
//        // Left Arm
//        jointPoints[PositionIndex.lShldrBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
//        jointPoints[PositionIndex.lForearmBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
//        jointPoints[PositionIndex.lHand.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftHand);
//        jointPoints[PositionIndex.lThumb2.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
//        jointPoints[PositionIndex.lMid1.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);

//        // Face
//        jointPoints[PositionIndex.lEar.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
//        jointPoints[PositionIndex.lEye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftEye);
//        jointPoints[PositionIndex.rEar.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
//        jointPoints[PositionIndex.rEye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightEye);
//        jointPoints[PositionIndex.Nose.Int()].Transform = Nose.transform;

//        // Right Leg
//        jointPoints[PositionIndex.rThighBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
//        jointPoints[PositionIndex.rShin.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
//        jointPoints[PositionIndex.rFoot.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightFoot);
//        jointPoints[PositionIndex.rToe.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightToes);

//        // Left Leg
//        jointPoints[PositionIndex.lThighBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
//        jointPoints[PositionIndex.lShin.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
//        jointPoints[PositionIndex.lFoot.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
//        jointPoints[PositionIndex.lToe.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftToes);

//        // etc
//        jointPoints[PositionIndex.abdomenUpper.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Spine);
//        jointPoints[PositionIndex.hip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Hips);
//        jointPoints[PositionIndex.head.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
//        jointPoints[PositionIndex.neck.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Neck);
//        jointPoints[PositionIndex.spine.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Spine);

//        // Child Settings
//        // 본들 간의 부모-자식 관계를 설정하여 캐릭터의 본 구조를 정의
//        // Right Arm
//        jointPoints[PositionIndex.rShldrBend.Int()].Child = jointPoints[PositionIndex.rForearmBend.Int()];
//        jointPoints[PositionIndex.rForearmBend.Int()].Child = jointPoints[PositionIndex.rHand.Int()];

//        // Left Arm
//        jointPoints[PositionIndex.lShldrBend.Int()].Child = jointPoints[PositionIndex.lForearmBend.Int()];
//        jointPoints[PositionIndex.lForearmBend.Int()].Child = jointPoints[PositionIndex.lHand.Int()];

//        // Fase

//        // Right Leg
//        jointPoints[PositionIndex.rThighBend.Int()].Child = jointPoints[PositionIndex.rShin.Int()];
//        jointPoints[PositionIndex.rShin.Int()].Child = jointPoints[PositionIndex.rFoot.Int()];
//        jointPoints[PositionIndex.rFoot.Int()].Child = jointPoints[PositionIndex.rToe.Int()];

//        // Left Leg
//        jointPoints[PositionIndex.lThighBend.Int()].Child = jointPoints[PositionIndex.lShin.Int()];
//        jointPoints[PositionIndex.lShin.Int()].Child = jointPoints[PositionIndex.lFoot.Int()];
//        jointPoints[PositionIndex.lFoot.Int()].Child = jointPoints[PositionIndex.lToe.Int()];

//        // etc
//        jointPoints[PositionIndex.spine.Int()].Child = jointPoints[PositionIndex.neck.Int()];
//        jointPoints[PositionIndex.neck.Int()].Child = jointPoints[PositionIndex.head.Int()];
//        //jointPoints[PositionIndex.head.Int()].Child = jointPoints[PositionIndex.Nose.Int()];


//        // Set Inverse
//        //  jointPoints 컬렉션 안에 있는 각각의 jointPoint 객체에 대하여 순회한다.
//        // InitRotation 값을 jointPoint.Transform.rotation(초기 회전값)으로 저장
//        foreach (var jointPoint in jointPoints)
//        {
//            // 예외처리
//            if (jointPoint.Transform != null)
//            {
//                jointPoint.InitRotation = jointPoint.Transform.rotation;
//            }
//            // 예외처리
//            if (jointPoint.Child != null)
//            {
//                // Inverse 값을 함수 GetInverse(jointPoint, jointPoint.Child)의 결과로 설정하여 역변환 값을 저장
//                jointPoint.Inverse = GetInverse(jointPoint, jointPoint.Child);
//            }
//        }

//        // 객체의 초기 중심 위치를 엉덩이 본의 위치로 저장한다.
//        initPosition = jointPoints[PositionIndex.hip.Int()].Transform.position;

//        // 엉덩이 본, 왼쪽허벅지, 오른쪽허벅지로 계산한 삼각형 면의 법선벡터를 forward에 저장한다.
//        var forward = TriangleNormal(jointPoints[PositionIndex.hip.Int()].Transform.position, jointPoints[PositionIndex.lThighBend.Int()].Transform.position, jointPoints[PositionIndex.rThighBend.Int()].Transform.position);
//        // 엉덩이 본이 forward 방향을 바라보도록 회전시킨 후, 그 회전의 역(반대) 회전을 저장한다.
//        jointPoints[PositionIndex.hip.Int()].Inverse = Quaternion.Inverse(Quaternion.LookRotation(forward));

//        // For Head Rotation
//        jointPoints[PositionIndex.head.Int()].InitRotation = jointPoints[PositionIndex.head.Int()].Transform.rotation;
//        var gaze = jointPoints[PositionIndex.Nose.Int()].Transform.position - jointPoints[PositionIndex.head.Int()].Transform.position;
//        jointPoints[PositionIndex.head.Int()].Inverse = Quaternion.Inverse(Quaternion.LookRotation(gaze));

//        jointPoints[PositionIndex.lHand.Int()].InitRotation = jointPoints[PositionIndex.lHand.Int()].Transform.rotation;
//        jointPoints[PositionIndex.lHand.Int()].Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.lThumb2.Int()].Transform.position - jointPoints[PositionIndex.lMid1.Int()].Transform.position));

//        jointPoints[PositionIndex.rHand.Int()].InitRotation = jointPoints[PositionIndex.rHand.Int()].Transform.rotation;
//        jointPoints[PositionIndex.rHand.Int()].Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Transform.position - jointPoints[PositionIndex.rMid1.Int()].Transform.position));
//    }
//    // 벡터 a, b, c의 값으로 삼각형의 면을 만들고, 면이 바라보는 방향(법선 벡터) forward를 반환하는 함수
//    Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
//    {
//        // 벡터 d1, d2를 구한다.
//        Vector3 d1 = a - b;
//        Vector3 d2 = a - c;

//        // Cross는 벡터의 외적(cross product)을 계산하는 함수
//        Vector3 dd = Vector3.Cross(d1, d2);
//        // 벡터 dd는 벡터 d1과 d2가 이루는 평면에 수직인 벡터,
//        // d1, d2벡터로 그려진 삼각형의 면이 어느 방향을 향하고 있는 지 나타냄
//        dd.Normalize(); // 정규화

//        return dd; // dd값을 반환
//    }

//    // Quaternion 자료형의 GetInverse(역회전을 구하는) 함수
//    // 매개변수 자료형은 클래스 JointPoint으로 JointPoint의 인스턴스를 모두 참조 할 수 있다.
//    private Quaternion GetInverse(JointPoint p1, JointPoint p2)
//    {
//        // p1 - p2 계산하여, p1의 위치를 기준으로 p2를 바라보는 방향의 회전값을 구한 후,
//        // 그 회전값의 역회전 값을 반환한다.
//        return Quaternion.Inverse(Quaternion.LookRotation(p1.Transform.position - p2.Transform.position));
//    }


//    private void CameraPlayStart()
//    {
//        WebCamDevice[] devices = WebCamTexture.devices;
//        webCamTexture = new WebCamTexture(devices[0].name);

//        GameObject videoScreen = GameObject.Find("VideoScreen");
//        RawImage screen = videoScreen.GetComponent<RawImage>();
//        var sd = screen.GetComponent<RectTransform>();
//        screen.texture = webCamTexture;

//        webCamTexture.Play();

//        sd.sizeDelta = new Vector2(videoScreenWidth, (int)(videoScreenWidth * webCamTexture.height / webCamTexture.width));

//        texture = new Texture2D(webCamTexture.width, webCamTexture.height);
//    }

//    private void VideoPlayStart()
//    {
//        var obj = GameObject.Find("Video Player");
//        VideoPlayer videoPlayer = obj.GetComponent<VideoPlayer>();

//        videoTexture = new RenderTexture((int)videoPlayer.clip.width, (int)videoPlayer.clip.height, 24);

//        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
//        videoPlayer.targetTexture = videoTexture;

//        GameObject videoScreen = GameObject.Find("VideoScreen");
//        RawImage screen = videoScreen.GetComponent<RawImage>();
//        var sd = screen.GetComponent<RectTransform>();
//        sd.sizeDelta = new Vector2(videoScreenWidth, (int)(videoScreenWidth * videoPlayer.clip.height / videoPlayer.clip.width));
//        screen.texture = videoTexture;

//        videoPlayer.Play();

//        texture = new Texture2D(videoTexture.width, videoTexture.height);
//    }

//    void Update() // 캠처리 및 포즈업데이트코루틴 호출
//    {
//        if (UseWebCam)
//        {
//            if (webCamTexture != null)
//            {
//                Color32[] color32 = webCamTexture.GetPixels32();
//                texture.SetPixels32(color32);
//                texture.Apply();
//            }
//        }
//        else
//        {
//            if (videoTexture != null)
//            {
//                Graphics.SetRenderTarget(videoTexture);
//                texture.ReadPixels(new UnityEngine.Rect(0, 0, videoTexture.width, videoTexture.height), 0, 0);
//                texture.Apply();
//                Graphics.SetRenderTarget(null);
//            }
//        }

//        StartCoroutine("PoseUpdate", texture);

//    }

//    // 실시간으로 포즈를 업데이트하는 함수, 매개변수 texture(웹캠화면)
//    IEnumerator PoseUpdate(Texture2D texture)
//    {
//        var img = ResizeTexture(texture);
//        // Predict <- AI모델의 2D이미지를 읽어와서 3D데이터로 변환하는 함수
//        Predict(img);


//        // 이 밑으로는 캐릭터의 본 회전을 담당하는 함수

//        // 센터 이동 및 회전
//        // forward 값을 저장한다.
//        var forward = // 벡터 a, b, c의 값으로 삼각형의 면을 만들고, 면이 바라보는 방향(법선 벡터) forward를 반환하는 함수
//            TriangleNormal(jointPoints[PositionIndex.hip.Int()].Pos3D, jointPoints[PositionIndex.lThighBend.Int()].Pos3D, jointPoints[PositionIndex.rThighBend.Int()].Pos3D);

//        // 엉덩이 본의 위치값은 
//        jointPoints[PositionIndex.hip.Int()].Transform.position =
//            // 엉덩이 본의 월드 3D좌표계의 값에 0.01을 곱해 값을 스케일링한 후,
//            jointPoints[PositionIndex.hip.Int()].Pos3D * 0.01f
//            // 그 결과에 객체의 초기 위치값의 x, z 좌표를 적용하여 최종 위치를 대입한다.
//            + new Vector3(initPosition.x, 0f, initPosition.z);

//        // 엉덩이 본의 회전값은 
//        jointPoints[PositionIndex.hip.Int()].Transform.rotation =
//            // 엉덩이 본이 forward 방향으로, 정면으로 향하도록 설정한다. 
//            Quaternion.LookRotation(forward)
//            // 현재 본의 회전값의 역방향을 곱하여 현재 본의 회전 상태를 초기 상태로 되돌린다.
//            * jointPoints[PositionIndex.hip.Int()].Inverse
//            // 본의 초기 회전값을 곱하여 최종 회전값을 결정.
//            * jointPoints[PositionIndex.hip.Int()].InitRotation;
//        // 엉덩이 본이 가진 정면 방향 forward 과 월드좌표계에서의 회전 방향이 다르기 때문에
//        // 엉덩이 본이 정면을 바라보면서 월드좌표계에서 회전을 할 수 있게 하기 위해서 이런 방법을 사용함


//        // 각 뼈의 회전 
//        // jointPoints 컬렉션 안에 있는 각각의 jointPoint 객체에 대하여 순회한다.
//        foreach (var jointPoint in jointPoints)
//        {
//            // 예외처리
//            if (jointPoint.Child != null)
//            {
//                // jointPoint의 회전(각도)값은 
//                jointPoint.Transform.rotation =
//                // 엉덩이 본이 jointPoint.Child를 바라보는 방향으로 회전, 이 방향을 forward 벡터로 맞추는 회전
//                Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, forward)
//                // 엉덩이 본의 현재 회전 상태(jointPoint.Inverse)를 제거하여, 기본적으로 설정된 회전 상태를 적용
//                * jointPoint.Inverse
//                // 초기 회전값을 곱하고 최종적으로 계산된 회전값 방향으로 회전한다.
//                * jointPoint.InitRotation;
//            }
//        }

//        // 머리 회전 구하기
//        // gaze는 Nose를 기준으로 head를 바라보는 (월드)Vector3 Pos3D값이다.
//        var gaze = jointPoints[PositionIndex.Nose.Int()].Pos3D - jointPoints[PositionIndex.head.Int()].Pos3D;
//        // f는 코, 왼쪽귀, 오른쪽귀로 이루어진 삼각형의 수직벡터다.
//        var f = TriangleNormal(jointPoints[PositionIndex.Nose.Int()].Pos3D, jointPoints[PositionIndex.rEar.Int()].Pos3D, jointPoints[PositionIndex.lEar.Int()].Pos3D);
//        // head에 머리에 해당하는 jointPoints 값 할당
//        var head = jointPoints[PositionIndex.head.Int()];
//        // 머리의 회전(각도)를
//        // gaze 벡터가 객체의 전면을 향하고, f 벡터가 객체의 상단을 향하도록 회전하고,
//        // 역방향을 곱해 회전값을 초기화하고, 초기 회전값을 곱해 최종적으로 계산된 회전 값 방향으로 회전한다.
//        head.Transform.rotation = Quaternion.LookRotation(gaze, f) * head.Inverse * head.InitRotation;


//        // 손목회전 (테스트 코드) 방식은 같은 듯
//        var lf = TriangleNormal(jointPoints[PositionIndex.lHand.Int()].Pos3D, jointPoints[PositionIndex.lMid1.Int()].Pos3D, jointPoints[PositionIndex.lThumb2.Int()].Pos3D);
//        var lHand = jointPoints[PositionIndex.lHand.Int()];
//        lHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.lThumb2.Int()].Pos3D - jointPoints[PositionIndex.lMid1.Int()].Pos3D, lf) * lHand.Inverse * lHand.InitRotation;
//        var rf = TriangleNormal(jointPoints[PositionIndex.rHand.Int()].Pos3D, jointPoints[PositionIndex.rThumb2.Int()].Pos3D, jointPoints[PositionIndex.rMid1.Int()].Pos3D);
//        var rHand = jointPoints[PositionIndex.rHand.Int()];
//        rHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Pos3D - jointPoints[PositionIndex.rMid1.Int()].Pos3D, rf) * rHand.Inverse * rHand.InitRotation;

//        yield return null;
//    }

//    public void InitONNX()
//    {
//        Onnx = Net.ReadNetFromONNX(Application.dataPath + @"\MobileNet3D2.onnx");
//        for (var i = 0; i < 4; i++) outputs[i] = new Mat();
//    }

//    /// <summary>
//    /// Predict
//    /// </summary>
//    /// <param name="img"></param>
//    /// 

//    // 뭔가 데이터를 읽어오는 코드... 나는 Onnx을 사용하지 않는다.
//    // 미디어파이프 데이터 읽어와서 대입하는 식으로 바꿔야할듯....?

//    // ONNX 모델에서 2D이미지 관절 읽어와서 인체의 3D 위치를 계산한 후,
//    // 특정 주파수 이하의 신호를 통과시키고, 그 이상의 주파수 신호를 차단하는 필터를 통해
//    // 관절의 위치를 부드럽게 하는 함수 (여기가 AI모델의 2D이미지를 읽어와서 3D데이터로 변환하는 부분)
//    public void Predict(Mat img)
//    {
//        var blob = CvDnn.BlobFromImage(img, 1.0 / 255.0, new OpenCvSharp.Size(inputImageSize, inputImageSize), 0.0, false, false);
//        Onnx.SetInput(blob);
//        Onnx.Forward(outputs, new string[] { "369", "373", "361", "365" });

//        // copy 2D outputs
//        Marshal.Copy(outputs[2].Data, heatMap2Dbuf, 0, heatMap2Dbuf.Length);
//        Buffer.BlockCopy(heatMap2Dbuf, 0, heatMap2D, 0, heatMap2Dbuf.Length);
//        Marshal.Copy(outputs[3].Data, offset2Dbuf, 0, offset2Dbuf.Length);
//        Buffer.BlockCopy(offset2Dbuf, 0, offset2D, 0, offset2Dbuf.Length);
//        for (var j = 0; j < JointNum; j++)
//        {
//            var maxXIndex = 0;
//            var maxYIndex = 0;
//            jointPoints[j].score2D = 0.0f;
//            for (var y = 0; y < HeatMapCol; y++)
//            {
//                for (var x = 0; x < HeatMapCol; x++)
//                {
//                    var l = new List<int>();
//                    var v = heatMap2D[(HeatMapCol_Squared) * j + HeatMapCol * y + x];

//                    if (v > jointPoints[j].score2D)
//                    {
//                        jointPoints[j].score2D = v;
//                        maxXIndex = x;
//                        maxYIndex = y;
//                    }
//                }

//            }

//            jointPoints[j].Pos2D.x = (offset2D[HeatMapCol_Squared * j + HeatMapCol * maxYIndex + maxXIndex] + maxXIndex / (float)HeatMapCol) * (float)inputImageSize;
//            jointPoints[j].Pos2D.y = (offset2D[HeatMapCol_Squared * (j + JointNum) + HeatMapCol * maxYIndex + maxXIndex] + maxYIndex / (float)HeatMapCol) * (float)inputImageSize;
//        }

//        // copy 3D outputs
//        Marshal.Copy(outputs[0].Data, heatMap3Dbuf, 0, heatMap3Dbuf.Length);
//        Buffer.BlockCopy(heatMap3Dbuf, 0, heatMap3D, 0, heatMap3Dbuf.Length);
//        Marshal.Copy(outputs[1].Data, offset3Dbuf, 0, offset3Dbuf.Length);
//        Buffer.BlockCopy(offset3Dbuf, 0, offset3D, 0, offset3Dbuf.Length);
//        for (var j = 0; j < JointNum; j++)
//        {
//            var maxXIndex = 0;
//            var maxYIndex = 0;
//            var maxZIndex = 0;
//            jointPoints[j].score3D = 0.0f;
//            for (var z = 0; z < HeatMapCol; z++)
//            {
//                for (var y = 0; y < HeatMapCol; y++)
//                {
//                    for (var x = 0; x < HeatMapCol; x++)
//                    {
//                        float v = heatMap3D[HeatMapCol_Cube * j + HeatMapCol_Squared * z + HeatMapCol * y + x];
//                        if (v > jointPoints[j].score3D)
//                        {
//                            jointPoints[j].score3D = v;
//                            maxXIndex = x;
//                            maxYIndex = y;
//                            maxZIndex = z;
//                        }
//                    }
//                }
//            }

//            jointPoints[j].Now3D.x = (offset3D[HeatMapCol_Cube * j + HeatMapCol_Squared * maxZIndex + HeatMapCol * maxYIndex + maxXIndex] + (float)maxXIndex / (float)HeatMapCol) * (float)inputImageSize;
//            jointPoints[j].Now3D.y = (float)inputImageSize - (offset3D[HeatMapCol_Cube * (j + JointNum) + HeatMapCol_Squared * maxZIndex + HeatMapCol * maxYIndex + maxXIndex] + (float)maxYIndex / (float)HeatMapCol) * (float)inputImageSize;
//            jointPoints[j].Now3D.z = (offset3D[HeatMapCol_Cube * (j + JointNum * 2) + HeatMapCol_Squared * maxZIndex + HeatMapCol * maxYIndex + maxXIndex] + (float)(maxZIndex - 7) / (float)HeatMapCol) * (float)inputImageSize;
//        }

//        // Calculate hip location
//        var lc = (jointPoints[PositionIndex.rThighBend.Int()].Now3D + jointPoints[PositionIndex.lThighBend.Int()].Now3D) / 2f;
//        jointPoints[PositionIndex.hip.Int()].Now3D = (jointPoints[PositionIndex.abdomenUpper.Int()].Now3D + lc) / 2f;
//        // Calculate neck location
//        jointPoints[PositionIndex.neck.Int()].Now3D = (jointPoints[PositionIndex.rShldrBend.Int()].Now3D + jointPoints[PositionIndex.lShldrBend.Int()].Now3D) / 2f;
//        // Calculate head location
//        var cEar = (jointPoints[PositionIndex.rEar.Int()].Now3D + jointPoints[PositionIndex.lEar.Int()].Now3D) / 2f;
//        var hv = cEar - jointPoints[PositionIndex.neck.Int()].Now3D;
//        var nhv = Vector3.Normalize(hv);
//        var nv = jointPoints[PositionIndex.Nose.Int()].Now3D - jointPoints[PositionIndex.neck.Int()].Now3D;
//        jointPoints[PositionIndex.head.Int()].Now3D = jointPoints[PositionIndex.neck.Int()].Now3D + nhv * Vector3.Dot(nhv, nv);
//        // Calculate spine location
//        jointPoints[PositionIndex.spine.Int()].Now3D = jointPoints[PositionIndex.abdomenUpper.Int()].Now3D;

//        // Low pass filter
//        foreach (var jp in jointPoints)
//        {
//            jp.Pos3D = jp.PrevPos3D * 0.5f + jp.Now3D * 0.5f;
//            jp.PrevPos3D = jp.Pos3D;
//        }
//    }

//    /// <summary>
//    /// Resize Texture and Convrt to Mat
//    /// </summary>
//    /// <param name="src"></param>
//    /// <returns></returns>
//    private Mat ResizeTexture(Texture2D src)
//    {
//        // 이미지나 비디오의 관심 영역을 정확하게 설정하기 위한 변수들
//        // clipRect란 사각형 형태의 클리핑 영역
//        float bbLeft = clipRect.xMin;
//        float bbRight = clipRect.xMax;
//        float bbTop = clipRect.yMin;
//        float bbBottom = clipRect.yMax;
//        float bbWidth = clipRect.width;
//        float bbHeight = clipRect.height;

//        float videoLongSide = (videoWidth > videoHeight) ? videoWidth : videoHeight;
//        float videoShortSide = (videoWidth > videoHeight) ? videoHeight : videoWidth;
//        float aspectWidth = videoWidth / videoShortSide;
//        float aspectHeight = videoHeight / videoShortSide;

//        float left = bbLeft;
//        float right = bbRight;
//        float top = bbTop;
//        float bottom = bbBottom;

//        left /= videoShortSide;
//        right /= videoShortSide;
//        top /= videoShortSide;
//        bottom /= videoShortSide;

//        src.filterMode = FilterMode.Trilinear;
//        src.Apply(true);

//        // 매개변수 너비, 높이, 깊이로 렌더텍스쳐
//        RenderTexture rt = new RenderTexture(224, 224, 32);
//        Graphics.SetRenderTarget(rt);
//        GL.LoadPixelMatrix(left, right, bottom, top);
//        GL.Clear(true, true, new Color(0, 0, 0, 0));
//        Graphics.DrawTexture(new UnityEngine.Rect(0, 0, aspectWidth, aspectHeight), src);

//        UnityEngine.Rect dstRect = new UnityEngine.Rect(0, 0, 224, 224);
//        Texture2D dst = (Texture2D)TextureObject.GetComponent<Renderer>().material.mainTexture;
//        dst.ReadPixels(dstRect, 0, 0, true);
//        Graphics.SetRenderTarget(null);
//        Destroy(rt);

//        dst.Apply();

//        TextureObject.GetComponent<Renderer>().material.mainTexture = dst;

//        // Convrt to Mat
//        Color32[] c = dst.GetPixels32();
//        var m = new Mat(224, 224, MatType.CV_8UC3);
//        var videoSourceImageData = new Vec3b[224 * 224];
//        for (var i = 0; i < 224; i++)
//        {
//            for (var j = 0; j < 224; j++)
//            {
//                var col = c[j + i * 224];
//                var vec3 = new Vec3b
//                {
//                    Item0 = col.b,
//                    Item1 = col.g,
//                    Item2 = col.r
//                };
//                videoSourceImageData[j + i * 224] = vec3;
//            }
//        }
//        m.SetArray(0, 0, videoSourceImageData);

//        return m.Flip(FlipMode.X);
//    }
//}
