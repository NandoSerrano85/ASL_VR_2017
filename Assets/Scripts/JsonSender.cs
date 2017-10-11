using UnityEngine;
using Leap;
using Newtonsoft.Json;
using System;
using System.Threading;

[RequireComponent(typeof(HandController))]
public class JsonSender : MonoBehaviour
{
    private Controller leapController;
    [SerializeField] private int totalFramesToProcess = 250;
    private NetworkSocket networkSocket;
    private int totalFramesProcessed;
    private long currentFrameID;

    private FrameData frameData;

    [Serializable]
    private class FrameData
    {
        public long frameID;
        public float pinchStrength;
        public float grabStrength;
        public float averageDistance;
        public float averageSpread;
        public float averageTriSpread;
    }

    void Start()
    {
        leapController = GetComponent<HandController>().GetLeapController();
        currentFrameID = leapController.Frame().Id;
        networkSocket = FindObjectOfType<NetworkSocket>();
        networkSocket.connect();
        totalFramesProcessed = 0;
        frameData = new FrameData();
        resetFrameData();
    }

    void Update()
    {
        Frame currentFrame = leapController.Frame();

        if (currentFrameID == currentFrame.Id)
            return;

        currentFrameID = currentFrame.Id;
        
        if(currentFrame.Hands.Count > 0) // There is at least one hand in the scene.
            processFrames(currentFrame);
    }

    private void processFrames(Frame currentFrame)
    {
        frameData.frameID = currentFrame.Id;

        foreach (Hand hand in currentFrame.Hands)
        {
            frameData.pinchStrength = hand.PinchStrength;
            frameData.grabStrength = hand.GrabStrength;
        }

        Frame previousFrame = leapController.Frame(1);

        for (int i = 0; i < currentFrame.Fingers.Count; i++)
        {
            frameData.averageDistance += (float)Math.Sqrt((currentFrame.Fingers[i].TipPosition - previousFrame.Fingers[i].TipPosition).MagnitudeSquared);
        }

        for (int i = 0; i < currentFrame.Fingers.Count - 1; i++)
        {
            frameData.averageSpread += (float)Math.Sqrt((currentFrame.Fingers[i + 1].TipPosition - currentFrame.Fingers[i].TipPosition).MagnitudeSquared);
        }
        for (int i = 0; i < currentFrame.Fingers.Count - 1; i++)
        {
            frameData.averageTriSpread += getTriSpread(currentFrame.Fingers[i], currentFrame.Fingers[i + 1]);
        }

        if (totalFramesProcessed == totalFramesToProcess)
        {
            frameData.averageDistance /= (totalFramesToProcess - 1);
            frameData.averageSpread /= totalFramesToProcess;
            frameData.averageTriSpread /= totalFramesToProcess;

            Thread jsonThread = new Thread(() => processJson(frameData));
            jsonThread.IsBackground = true;
            jsonThread.Start();

            totalFramesProcessed = 0;
        }
        else
            totalFramesProcessed++;
    }

    private float getTriSpread(Finger currentFinger, Finger nextFinger)
    {
        Bone currentBone = currentFinger.Bone(Bone.BoneType.TYPE_METACARPAL);
        Bone nextBone = nextFinger.Bone(Bone.BoneType.TYPE_METACARPAL);

        Vector metacarpal = new Vector(currentBone.Center.x + (nextBone.Center.x - currentBone.Center.x) / 2.0f,
                                       currentBone.Center.y + (nextBone.Center.y - currentBone.Center.y) / 2.0f,
                                       currentBone.Center.z + (nextBone.Center.z - currentBone.Center.z) / 2.0f);

        Vector vector1 = new Vector(currentFinger.TipPosition.x + metacarpal.x, currentFinger.TipPosition.y + metacarpal.y, currentFinger.TipPosition.z + metacarpal.z);
        Vector vector2 = new Vector(nextFinger.TipPosition.x + metacarpal.x, nextFinger.TipPosition.y + metacarpal.y, nextFinger.TipPosition.z + metacarpal.z);

        return 0.5f * Mathf.Sqrt(vector1.MagnitudeSquared) * Mathf.Sqrt(vector2.MagnitudeSquared) * (vector1.AngleTo(vector2) * (180.0f / Mathf.PI));
    }

    private void writeJsonToFile(FrameData frameData)
    {
        using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@"LeapMotionTrainingDataExample.txt", true))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, frameData);
        }
    }

    private void processJson(FrameData frameData)
    {
        string jsonString = JsonConvert.SerializeObject(frameData, Formatting.Indented);

        string receivedGesture = networkSocket.readSocket();

        if (receivedGesture != "")
            Debug.Log(receivedGesture);

        networkSocket.writeSocket(jsonString);
        writeJsonToFile(frameData);
        resetFrameData();
    }

    private void resetFrameData()
    {
        frameData.frameID = -1;
        frameData.pinchStrength = 0.0f;
        frameData.grabStrength = 0.0f;
        frameData.averageDistance = 0.0f;
        frameData.averageSpread = 0.0f;
        frameData.averageTriSpread = 0.0f;
    }
}