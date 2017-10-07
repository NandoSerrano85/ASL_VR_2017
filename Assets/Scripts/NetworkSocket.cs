using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkSocket : MonoBehaviour
{
    [SerializeField] private string host = "localhost";
    [SerializeField] private int port = 50000;
    [SerializeField] private bool autoConnect = false;

    private bool connected;

    private TcpClient tcpSocket;
    private NetworkStream netStream;

    private StreamWriter socketWriter;
    private StreamReader socketReader;

    void Start()
    {
        if (autoConnect)
            connect();
    }

    public void connect()
    {
        try
        {
            tcpSocket = new TcpClient(host, port);
            netStream = tcpSocket.GetStream();
            socketReader = new StreamReader(netStream, Encoding.Default);
            socketWriter = new StreamWriter(netStream, Encoding.Default);
            connected = tcpSocket.Connected;
        }
        catch (Exception e)
        {
            Debug.Log("Couldn't make a connection to the server: " + e.Message);
        }
    }

    public void writeSocket(string jsonString)
    {
        if (!connected || socketWriter == null)
            return;

        socketWriter.Write(jsonString);
        socketWriter.Flush();
    }

    public string readSocket()
    {
        if (!connected || socketReader == null)
            return "";

        if (netStream.DataAvailable)
            return socketReader.ReadLine();

        return "";
    }

    public void closeSocket()
    {
        if (!connected)
            return;

        try
        {
            socketWriter.Close();
            socketReader.Close();
            tcpSocket.Close();

            connected = tcpSocket.Connected;
        }
        catch(SocketException e)
        {
            Debug.Log("The connection couldn't close properly: " + e.Message);
        }
    }

    void OnDestroy()
    {
       writeSocket("quit");
       closeSocket();
    }
}