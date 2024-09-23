using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static MotionTracking;

// 미디어 파이프의 포즈 데이터를 캐릭터 본 구조에 할당하자.
public enum PositionIndex : int
{
    nose,
    left_eye_inner,
    left_eye,
    left_eye_outer,
    right_eye_inner,
    right_eye,
    right_eye_outer,
    left_ear,
    right_ear,
    mouth_left,
    mouth_right,
    left_shoulder,
    right_shoulder,
    left_elbow,
    right_elbow,
    left_wrist,
    right_wrist,
    left_pinky,
    right_pinky,
    left_index,
    right_index,
    left_thumb,
    right_thumb,
    left_hip,
    right_hip,
    left_knee,
    right_knee,
    left_ankle,
    right_ankle,
    left_heel,
    right_heel,
    left_foot_index,
    right_foot_index,
    head,
    neck,
    hip,
    Count,
    None,
}
// PositionIndex 열거형 값을 int로 변환하기 위한 static 클래스
public static partial class EnumExtend
{
    public static int Int(this PositionIndex i)
    {
        return (int)i;
    }
}


public class LandMark : MonoBehaviour
{

    LandMarkData landMark;
    // UDPserver
    public UDPServer udpServer;
    public Transform[] tempBones;
    public MotionTracking motionTracking;


    private void Awake()
    {
        udpServer = GetComponent<UDPServer>();
        tempBones = new Transform[(int)PositionIndex.Count];

        for (int i = 0; i < tempBones.Length; i++)
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.name = ((PositionIndex)i).ToString();
            tempBones[i] = go.transform;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {

        // Head 본 대체 위치
        Vector3 left_ear = new Vector3(landMark.data[(int)PositionIndex.left_ear].x, landMark.data[(int)PositionIndex.left_ear].y, landMark.data[(int)PositionIndex.left_ear].z);
        Vector3 right_ear = new Vector3(landMark.data[(int)PositionIndex.right_ear].x, landMark.data[(int)PositionIndex.right_ear].y, landMark.data[(int)PositionIndex.right_ear].z);
        Vector3 head = Vector3.Lerp(left_ear, right_ear, 0.5f);
        tempBones[(int)PositionIndex.head].position = motionTracking.landMarkInfos[(int)PositionIndex.head].mediaPipePos = head;


        // 쇄골 clavicle 위치(Neck 구하려고)
        Vector3 left_shoulder = new Vector3(landMark.data[(int)PositionIndex.left_shoulder].x, landMark.data[(int)PositionIndex.left_shoulder].y, landMark.data[(int)PositionIndex.left_shoulder].z);
        Vector3 right_shoulder = new Vector3(landMark.data[(int)PositionIndex.right_shoulder].x, landMark.data[(int)PositionIndex.right_shoulder].y, landMark.data[(int)PositionIndex.right_shoulder].z);
        Vector3 clavicle = Vector3.Lerp(left_shoulder, right_shoulder, 0.5f);

        // neck 본 대체 위치 (head와 쇄골 사이)
        Vector3 neck = Vector3.Lerp(head, clavicle, 0.5f);
        tempBones[(int)PositionIndex.neck].position = motionTracking.landMarkInfos[(int)PositionIndex.neck].mediaPipePos = neck;

        // hip 본 대체 위치
        Vector3 left_hip = new Vector3(landMark.data[(int)PositionIndex.left_hip].x, landMark.data[(int)PositionIndex.left_hip].y, landMark.data[(int)PositionIndex.left_hip].z);
        Vector3 right_hip = new Vector3(landMark.data[(int)PositionIndex.right_hip].x, landMark.data[(int)PositionIndex.right_hip].y, landMark.data[(int)PositionIndex.right_hip].z);
        Vector3 hip = Vector3.Lerp(left_hip, right_hip, 0.5f);
        tempBones[(int)PositionIndex.hip].position = motionTracking.landMarkInfos[(int)PositionIndex.hip].mediaPipePos = hip;

    }



}
