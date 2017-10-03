using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkSocket : MonoBehaviour
{
    public string host = "localhost";
    public int port = 50000;

    internal bool socketReady = false;

    TcpClient tcpSocket;
    NetworkStream netStream;

    StreamWriter socketWriter;
    StreamReader socketReader;

    void Awake()
    {
        setUpSocket();
    }

    private void setUpSocket()
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

    public void writeSocket(string jsonString)
    {
        if (!socketReady)
            return;

        socketWriter.Write(jsonString);
        socketWriter.Flush();
    }

    public string readSocket()
    {
        if (!socketReady)
            return "";

        if (netStream.DataAvailable)
            Debug.Log(socketReader.ReadToEnd());

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
