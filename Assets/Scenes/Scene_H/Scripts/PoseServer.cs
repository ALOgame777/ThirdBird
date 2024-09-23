using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;

// 받은 JSON 데이터를 담을 클래스
//[Serializable]
//public class Coordinates
//{
//    public float x;
//    public float y;
//    public float z;
//}

//[Serializable]
//public class Angles
//{
//    public float LEFT_ELBOW;
//    public float RIGHT_ELBOW;
//}

//[Serializable]
public class Motions
{
    public bool clapping;
    public bool swinging_sword;
    public bool right_hand_slam;
    public bool throwing;

    public Motions(bool clapping, bool swinging_sword, bool right_hand_slam, bool throwing)
    {
        this.clapping = clapping;
        this.swinging_sword = swinging_sword;
        this.right_hand_slam = right_hand_slam;
        this.throwing = throwing;
    }

}

[Serializable]
public class MotionData
{
    // "motions" 필드를 Dictionary<string, bool>로 정의
    public Dictionary<string, bool> motions;
}


// Json 배열
[System.Serializable]
public struct UserDataList
{
    public List<MotionData> motionDatas;
}

public class PoseServer : MonoBehaviour
{

    public bool isClapping;
    public bool isSwinging;
    public bool isSlam;
    public bool isThrowing;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    public MotionData motionDatas;

    void Start()
    {
        // 서버 연결
        ConnectToServer("127.0.0.1", 65534);


    }

    void ConnectToServer(string serverIP, int port)
    {
        try
        {
            client = new TcpClient(serverIP, port);
            stream = client.GetStream();
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            Debug.Log("서버에 연결되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError("서버 연결 실패: " + e.Message);
        }
    }

    void ReceiveData()
    {
        try
        {
            while (true)
            {
                if (stream != null && stream.CanRead)
                {
                    byte[] bytes = new byte[1024];
                    int length = stream.Read(bytes, 0, bytes.Length);
                    string data = Encoding.UTF8.GetString(bytes, 0, length);

                    // 받은 데이터 처리 (JSON 파싱)
                    HandleReceivedData(data);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("데이터 수신 오류: " + e.Message);
        }
    }

    void HandleReceivedData(string data)
    {
        try
        {
            print(data);

            // JSON 데이터를 클래스에 맞게 파싱
            MotionData motionData = JsonConvert.DeserializeObject<MotionData>(data);

            // 딕셔너리에 데이터 넣기
            Dictionary<string, bool> motions = motionData.motions;

            // 구조체 값 할당 및 동작에 따른 처리
            if (motions.ContainsKey("clapping") && motions["clapping"])
            {
                Debug.Log("박수치기 감지");
                isClapping = true;
            }
            else
            {
                isClapping = false;
            }

            if (motions.ContainsKey("swinging_sword") && motions["swinging_sword"])
            {
                Debug.Log("칼 휘두르기 감지");
                isSwinging = true;
            }
            else
            {
                isSwinging = false;
            }

            if (motions.ContainsKey("right_hand_slam") && motions["right_hand_slam"])
            {
                Debug.Log("오른손으로 바닥 내려치기 감지");
                isSlam = true;
            }
            else
            {
                isSlam = false;
            }

            if (motions.ContainsKey("throwing") && motions["throwing"])
            {
                Debug.Log("물건 던지기 감지");
                isThrowing = true;
            }
            else
            {
                isThrowing = false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("JSON 파싱 오류: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        // 연결 종료
        if (receiveThread != null) receiveThread.Abort();
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }


}
