using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class NetworkSocket
{
    public string host = "localhost";
    public int port = 50000;

    internal bool socketReady = false;

    TcpClient tcpSocket;
    NetworkStream netStream;

    void Update()
    {
        string receivedData = readSocket();

        if(receivedData != "")
        {
            Console.WriteLine(receivedData);
        }
    }

    void Awake()
    {
        setUpSocket();
    }

    public void setUpSocket()
    {
        try
        {
            tcpSocket = new TcpClient(host, port);

            netStream = tcpSocket.GetStream();

            socketReady = true;
        }
        catch(Exception e)
        {
            Debug.WriteLine("Socket Error: " + e);
        }
    }

    public void writeSocket(string jsonString)
    {
        if (!socketReady)
            return;

        using (StreamWriter socketWriter = new StreamWriter(tcpSocket.GetStream(), Encoding.Default))
        {
            socketWriter.Write(jsonString);
            socketWriter.Flush();
        }
    }

    public string readSocket()
    {
        if (!socketReady)
            return "";

        using (StreamReader socketReader = new StreamReader(tcpSocket.GetStream(), Encoding.Default))
        {
            if (netStream.DataAvailable)
                return socketReader.ReadToEnd();
        }

        return "";
    }

    public void closeSocket()
    {
        if (!socketReady)
            return;

        tcpSocket.Close();

        socketReady = false;
    }
}
