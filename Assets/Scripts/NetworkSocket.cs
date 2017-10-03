using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class NetworkSocket : MonoBehaviour
{
    public string host = "localhost";
    public int port = 50000;

    internal bool socketReady = false;

    private string inputBuffer;
    public string InputBuffer { get {return inputBuffer; } internal set { inputBuffer = value; } }

    TcpClient tcpSocket;
    NetworkStream netStream;

    StreamWriter socketWriter;
    StreamReader socketReader;

    void Update()
    {
        string receivedData = readSocket();

        if (InputBuffer != "")
        {
            writeSocket(inputBuffer);
            InputBuffer = "";
        }

        if (receivedData != "")
        {
            Debug.Log(receivedData);
        }
    }

    void Awake()
    {
        setUpSocket();
    }

    void OnApplicationQuit()
    {
        closeSocket();
    }

    public void setUpSocket()
    {
        try
        {
            tcpSocket = new TcpClient(host, port);

            netStream = tcpSocket.GetStream();
            socketReader = new StreamReader(netStream, Encoding.Default);
            socketWriter = new StreamWriter(netStream, Encoding.Default);

            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket Error: " + e);
        }
    }

    public void writeSocket(string line)
    {
        if (!socketReady)
            return;

        line = line + "\r\n";
        socketWriter.Write(line);
        socketWriter.Flush();
    }

    public string readSocket()
    {
        if (!socketReady)
            return "";

        if (netStream.DataAvailable)
            return socketReader.ReadLine();

        return "";
    }

    public void closeSocket()
    {
        if (!socketReady)
            return;

        socketWriter.Close();
        socketReader.Close();
        tcpSocket.Close();

        socketReady = false;
    }
}
