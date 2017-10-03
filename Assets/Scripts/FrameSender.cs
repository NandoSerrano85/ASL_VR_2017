using UnityEngine;
using Leap;
using Leap.Unity;
using Newtonsoft.Json;
using System.Threading;

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
    }

    void Update()
    {
        Frame currentFrame = leapProvider.CurrentFrame;

        if (currentFrameID == currentFrame.Id)
            return;

        currentFrameID = currentFrame.Id;

        string jsonString = JsonConvert.SerializeObject(currentFrame.Hands, Formatting.Indented,
                                                        new JsonSerializerSettings()
                                                        {
                                                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                        });

        Thread socketThread = new Thread(() => sendJsonToSocket(jsonString));
        socketThread.Start();
    }

    private void sendJsonToSocket(string jsonString)
    {
        networkSocket.writeSocket(jsonString);

        using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@"LeapMotionTrainingDataExample.txt", true))
        {
            file.WriteLine(jsonString);
        }
    }

    void OnApplicationQuit()
    {
        networkSocket.closeSocket();
    }
}
