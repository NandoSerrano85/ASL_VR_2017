using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class StateObject
{
    public Socket workSocket = null;
    public const int bufferSize = 4096;
    public byte[] buffer = new byte[bufferSize];
    public StringBuilder stringBuilder = new StringBuilder();
}

public class AsyncSocketClient
{
    private const int port = 50000;

    private ManualResetEvent connectDone = new ManualResetEvent(false);
    private ManualResetEvent sendDone = new ManualResetEvent(false);
    private ManualResetEvent receiveDone = new ManualResetEvent(false);

    private string response = string.Empty;

    public void startClient(string message)
    {
        try
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            client.BeginConnect(remoteEP, new AsyncCallback(connectBack), client);
            connectDone.WaitOne();

            send(client, message);
            sendDone.WaitOne();

            receive(client);
            receiveDone.WaitOne();

            Debug.Log("Response Received: " + response);

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        catch(SocketException e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void receive(Socket client)
    {
        try
        {
            StateObject state = new StateObject();
            state.workSocket = client;

            client.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveCallBack), state);
        }
        catch(SocketException e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void receiveCallBack(IAsyncResult ar)
    {
        try
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            int bytesRead = client.EndReceive(ar);

            state.stringBuilder.Append(Encoding.Default.GetString(state.buffer, 0, bytesRead));

            if (state.stringBuilder.Length > 1)
                response = state.stringBuilder.ToString();

            receiveDone.Set();
        }
        catch (SocketException e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void connectBack(IAsyncResult ar)
    {
        try
        {
            Socket client = (Socket)ar.AsyncState;

            client.EndConnect(ar);

            connectDone.Set();
        }
        catch(SocketException e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void send(Socket client, string data)
    {
        byte[] byteData = Encoding.Default.GetBytes(data);

        client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(sendCallBack), client);
    }

    private void sendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket client = (Socket)ar.AsyncState;

            client.EndSend(ar);

            sendDone.Set();
        }
        catch(SocketException e)
        {
            Debug.Log(e.ToString());
        }
    }
}