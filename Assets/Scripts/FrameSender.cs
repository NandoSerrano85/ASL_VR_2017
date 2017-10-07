using UnityEngine;
using Leap;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(HandController))]
public class FrameSender : MonoBehaviour
{
    private Controller leapController;
    private NetworkSocket networkSocket;
    private long currentFrameID;

    [Serializable]
    private class FrameData
    {
        public long frameID;
        public long timeStamp;
        public List<Dictionary<string, List<string>>> hands;
        public List<Dictionary<string, List<string>>> pointables;
        public List<Dictionary<string, List<string>>> fingers;
    }

    void Start()
    {
        leapController = GetComponent<HandController>().GetLeapController();
        networkSocket = FindObjectOfType<NetworkSocket>();
        networkSocket.connect();
        currentFrameID = leapController.Frame().Id;
        InvokeRepeating("processFrame", 0.5f, 0.5f);
    }

    private void processFrame()
    {
        Frame frame = leapController.Frame();

        if (currentFrameID == frame.Id)
          return;

        currentFrameID = frame.Id;

        Thread jsonThread = new Thread(() => processJson(frame));
        jsonThread.IsBackground = true;
        jsonThread.Start();
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

    private void processJson(Frame frame)
    {
        FrameData frameData = getFrameData(frame);
        string jsonString = JsonConvert.SerializeObject(frameData, Formatting.Indented);

        string receivedGesture = networkSocket.readSocket();

        if (receivedGesture != "")
            Debug.Log(receivedGesture);

        networkSocket.writeSocket(jsonString);
        writeJsonToFile(frameData);
    }

    private FrameData getFrameData(Frame frame)
    {
        FrameData frameData = new FrameData();

        frameData.frameID = frame.Id;
        frameData.timeStamp = frame.Timestamp;

        frameData.hands = new List<Dictionary<string, List<string>>>();
        frameData.pointables = new List<Dictionary<string, List<string>>>();
        frameData.fingers = new List<Dictionary<string, List<string>>>();

        int count = 1;

        foreach (Hand hand in frame.Hands)
        {
            string handKey = "Hand " + count;

            Dictionary<string, List<string>> handDict = new Dictionary<string, List<string>>();
            handDict.Add(handKey, new List<string>());

            handDict[handKey].Add(hand.Id.ToString());

            if (hand.IsLeft)
            {
                handDict[handKey].Add("Left");
            }
            else
            {
                handDict[handKey].Add("Right");
            }

            handDict[handKey].Add(hand.Direction.ToString());
            handDict[handKey].Add(hand.PalmNormal.ToString());
            handDict[handKey].Add(hand.PalmPosition.ToString());
            handDict[handKey].Add(hand.PalmVelocity.ToString());
            handDict[handKey].Add(hand.StabilizedPalmPosition.ToString());
            handDict[handKey].Add(hand.PinchStrength.ToString());
            handDict[handKey].Add(hand.GrabStrength.ToString());
            handDict[handKey].Add(hand.Confidence.ToString());

            frameData.hands.Add(handDict);
        }

        count = 1;

        foreach (Pointable point in frame.Pointables)
        {
            string pointableKey = "Pointable " + count;

            Dictionary<string, List<string>> pointableDict = new Dictionary<string, List<string>>();
            pointableDict.Add(pointableKey, new List<string>());

            pointableDict[pointableKey].Add(point.Id.ToString());
            pointableDict[pointableKey].Add(point.Direction.ToString());
            pointableDict[pointableKey].Add(point.Hand.Id.ToString());
            pointableDict[pointableKey].Add(point.Length.ToString());
            pointableDict[pointableKey].Add(point.StabilizedTipPosition.ToString());
            pointableDict[pointableKey].Add(point.TipPosition.ToString());
            pointableDict[pointableKey].Add(point.TipVelocity.ToString());
            pointableDict[pointableKey].Add(point.IsTool.ToString());

            frameData.pointables.Add(pointableDict);

            Finger finger = new Finger(point);

            string fingerKey = "Finger " + count;

            Dictionary<string, List<string>> fingerDict = new Dictionary<string, List<string>>();
            fingerDict.Add(fingerKey, new List<string>());

            fingerDict[fingerKey].Add(finger.Type.ToString());
            fingerDict[fingerKey].Add(finger.JointPosition(Finger.FingerJoint.JOINT_MCP).ToString());
            fingerDict[fingerKey].Add(finger.JointPosition(Finger.FingerJoint.JOINT_MCP).ToString());
            fingerDict[fingerKey].Add(finger.JointPosition(Finger.FingerJoint.JOINT_PIP).ToString());
            fingerDict[fingerKey].Add(finger.JointPosition(Finger.FingerJoint.JOINT_DIP).ToString());
            fingerDict[fingerKey].Add(finger.JointPosition(Finger.FingerJoint.JOINT_TIP).ToString());

            foreach (Bone.BoneType boneType in (Bone.BoneType[])Enum.GetValues(typeof(Bone.BoneType)))
            {
                Bone bone = finger.Bone(boneType);

                if (bone.IsValid)
                {
                    fingerDict[fingerKey].Add(boneType.ToString());
                    fingerDict[fingerKey].Add(bone.Basis.xBasis.ToString());
                    fingerDict[fingerKey].Add(bone.Basis.yBasis.ToString());
                    fingerDict[fingerKey].Add(bone.Basis.zBasis.ToString());
                }
            }

            frameData.fingers.Add(fingerDict);

            count++;
       }


        return frameData;
    }
}