using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkSocket
{
    public string host = "localhost";
    public int port = 50000;

    internal bool socketReady = false;

    TcpClient tcpSocket;
    NetworkStream netStream;

    StreamWriter socketWriter;
    StreamReader socketReader;

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

    public void writeSocket(string data)
    {
        if (!socketReady)
            return;

        socketWriter.Write(data);
        socketWriter.Flush();
    }

    public string readSocket()
    {
        if (!socketReady)
            return "";

        if (netStream.DataAvailable)
            return socketReader.ReadToEnd();

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
