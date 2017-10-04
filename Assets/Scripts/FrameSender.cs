using UnityEngine;
using Leap;
using Leap.Unity;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;

public class FrameSender : MonoBehaviour
{
    private LeapServiceProvider leapProvider;
    private NetworkSocket networkSocket;
    private long currentFrameID;

    void Start()
    {
        leapProvider = GetComponent<LeapServiceProvider>();
        networkSocket = FindObjectOfType<NetworkSocket>();
        currentFrameID = leapProvider.CurrentFrame.Id;
        InvokeRepeating("processFrame", 0.5f, 0.5f);
    }

    private void processFrame()
    {
        Frame currentFrame = leapProvider.CurrentFrame;

        if (currentFrameID == currentFrame.Id)
            return;

        currentFrameID = currentFrame.Id;

        List<Hand> hands = currentFrame.Hands;

        Thread jsonFileThread = new Thread(() => writeJsonToFile(hands));
        jsonFileThread.Start();

        Thread socketThread = new Thread(() => sendJsonToSocket(hands));
        socketThread.Start();
    }

    private void writeJsonToFile(List<Hand> hands)
    {
        using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@"LeapMotionTrainingDataExample.txt", true))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, hands);
        }
    }

    private void sendJsonToSocket(List<Hand> hands)
    {
        string jsonString = JsonConvert.SerializeObject(hands, Formatting.Indented,
                                                        new JsonSerializerSettings()
                                                        {
                                                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                        });
        networkSocket.writeSocket(jsonString);
    }

    void OnApplicationQuit()
    {
        networkSocket.closeSocket();
    }
}
