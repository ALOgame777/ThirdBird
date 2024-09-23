using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


// LandMark 저장 할 구조
[Serializable]
public class LandMarkFormat
{
    public float x;
    public float y;
    public float z;
    public float visibility;
}

[Serializable]
public class LandMarkData
{
    public List<LandMarkFormat> data;
}

public class UDPServer : MonoBehaviour
{
    // Port 수업: 5005 
    // 내 컴퓨터IP: 192.168.0.12
    public int serverPort = 65534;

    // UDPServer 컨트롤러
    UdpClient udpServer;
    // EndPoint
    IPEndPoint remoteEndPoint;

    // LandmarkData
    public LandMarkData landMark;
    void Start()
    {
        StartUDPServer();
    }

    void Update()
    {

    }

    // UDP 서버 시작
    void StartUDPServer()
    {
        udpServer = new UdpClient(serverPort);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, serverPort);

        print("서버 시작! 클라이언트에서 들어오는 응답 기다리는 중..");

        // 응답이 들어오면 실행되는 함수 등록
        udpServer.BeginReceive(ReciveData, null);
    }
    void ReciveData(IAsyncResult result)
    {
        // 응답 온 데이터를 byte 배열로 받자.
        byte[] receiveByte = udpServer.EndReceive(result, ref remoteEndPoint);
        // byte 배열 데이터를 string으로 변경 (UTF-8)
        string receiveMessage = Encoding.UTF8.GetString(receiveByte);
        print(receiveMessage);

        // receiveMessage가 배열의 Json으로 들어와서 key값을 만들어줘야 JsonUtility.FromJson으로 사용이 가능
        // (무조건은 아님, receiveMessage를 확인 후!)
        receiveMessage = "{ \"data\" : " + receiveMessage + " }";
        // jsonStringData ----> LandMarkData 변환
        landMark = JsonUtility.FromJson<LandMarkData>(receiveMessage);


        // 다음 응답이 들어오면 실행되는 함수 등록(계속 반복하도록)
        udpServer.BeginReceive(ReciveData, null);
    }

    private void OnDestroy()
    {
        // 서버 종료
        udpServer.Close();
        print("UDP서버 종료");
    }
}
