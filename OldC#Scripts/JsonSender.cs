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
    private int totalFramesProcessed;
    private long currentFrameID;

    private FrameData frameData;

    void Start()
    {
        leapController = GetComponent<HandController>().GetLeapController();
        currentFrameID = leapController.Frame().Id;
        totalFramesProcessed = 0;
        frameData = new FrameData();
        frameData.resetFrameData();
    }

    void Update()
    {
        Frame currentFrame = leapController.Frame();

        if (currentFrameID == currentFrame.Id)
            return;

        currentFrameID = currentFrame.Id;

        // There is at least one hand in the scene and we haven't finished processing n frames.
        if (currentFrame.Hands.Count > 0 && totalFramesProcessed != totalFramesToProcess)
            processFrames(currentFrame);
    }

    private void processFrames(Frame currentFrame)
    {
        frameData.FrameID = currentFrame.Id;

        foreach (Hand hand in currentFrame.Hands)
        {
            frameData.PinchStrength = hand.PinchStrength;
            frameData.GrabStrength = hand.GrabStrength;
        }

        Frame previousFrame = leapController.Frame(1);

        for (int i = 0; i < currentFrame.Fingers.Count; i++)
        {
            frameData.AverageDistance += (float)Math.Sqrt((currentFrame.Fingers[i].TipPosition - previousFrame.Fingers[i].TipPosition).MagnitudeSquared);
        }

        for (int i = 0; i < currentFrame.Fingers.Count - 1; i++)
        {
            frameData.AverageSpread += (float)Math.Sqrt((currentFrame.Fingers[i + 1].TipPosition - currentFrame.Fingers[i].TipPosition).MagnitudeSquared);
        }
        for (int i = 0; i < currentFrame.Fingers.Count - 1; i++)
        {
            frameData.AverageTriSpread += getTriSpread(currentFrame.Fingers[i], currentFrame.Fingers[i + 1]);
        }

        totalFramesProcessed++;

        if (totalFramesProcessed == totalFramesToProcess)
        {
            frameData.AverageDistance /= (totalFramesToProcess - 1);
            frameData.AverageSpread /= totalFramesToProcess;
            frameData.AverageTriSpread /= totalFramesToProcess;

            Thread jsonThread = new Thread(() => sendJsonToServer(frameData));
            jsonThread.Start();
        }
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

    private void sendJsonToServer(FrameData frameData)
    {
        string jsonString = JsonConvert.SerializeObject(frameData, Formatting.Indented);

        AsyncSocketClient asyncSocketClient = new AsyncSocketClient();
        asyncSocketClient.startClient(jsonString);

        frameData.resetFrameData();
        totalFramesProcessed = 0;
    }
}