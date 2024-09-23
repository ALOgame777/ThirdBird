using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    
    public string ip = "192.168.43.26";
    public int port = 7777;
    private void Start()
    {
        StartUDPClient();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
           SendData("hahahaha");
        }
    }

    public void StartUDPClient()
    {
        udpClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        // Start receiving data asynchronously
        udpClient.BeginReceive(ReceiveData, null);

        // Send a message to the server
        SendData("Hello, server!");
    }

    private void ReceiveData(IAsyncResult result)
    {
        byte[] receivedBytes = udpClient.EndReceive(result, ref remoteEndPoint);
        string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedBytes);

        Debug.Log("Received from server: " + receivedMessage);

        // Continue receiving data asynchronously
        udpClient.BeginReceive(ReceiveData, null);
    }

    private void SendData(string message)
    {
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(message);

        // Send the message to the server
        udpClient.Send(sendBytes, sendBytes.Length, remoteEndPoint);

        Debug.Log("Sent to server: " + message);
    }

    public void SendData(byte[] sendBytes)
    {
        // Send the message to the server
        udpClient.Send(sendBytes, sendBytes.Length, remoteEndPoint);

        Debug.Log("sendBytes : " + sendBytes.Length);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
