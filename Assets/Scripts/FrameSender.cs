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
        public Dictionary<string, List<string>> hands;
        public Dictionary<string, List<string>> pointables;
        public Dictionary<string, List<string>> fingers;
        public Dictionary<string, List<string>> bones;
    }

    void Start()
    {
        leapController = GetComponent<HandController>().GetLeapController();
        networkSocket = FindObjectOfType<NetworkSocket>();
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
        networkSocket.writeSocket(jsonString);
        writeJsonToFile(frameData);
    }

    private FrameData getFrameData(Frame frame)
    {
        FrameData frameData = new FrameData();

        frameData.frameID = frame.Id;
        frameData.timeStamp = frame.Timestamp;

        frameData.hands = new Dictionary<string, List<string>>();
        frameData.pointables = new Dictionary<string, List<string>>();
        frameData.fingers = new Dictionary<string, List<string>>();
        frameData.bones = new Dictionary<string, List<string>>();

        int count = 1;

        foreach (Hand hand in frame.Hands)
        {
            string handName = "Hand " + count;

            frameData.hands.Add(handName, new List<string>());
            frameData.hands[handName].Add(hand.Id.ToString());

            if (hand.IsLeft)
            {
                frameData.hands[handName].Add("Left");
            }
            else
            {
                frameData.hands[handName].Add("Right");
            }

            frameData.hands[handName].Add(hand.Direction.ToString());
            frameData.hands[handName].Add(hand.PalmNormal.ToString());
            frameData.hands[handName].Add(hand.PalmPosition.ToString());
            frameData.hands[handName].Add(hand.PalmVelocity.ToString());
            frameData.hands[handName].Add(hand.StabilizedPalmPosition.ToString());
            frameData.hands[handName].Add(hand.PinchStrength.ToString());
            frameData.hands[handName].Add(hand.GrabStrength.ToString());
            frameData.hands[handName].Add(hand.Confidence.ToString());

            count++;
        }

        count = 1;

        foreach (Pointable point in frame.Pointables)
        {
            string pointableName = "Pointable " + count;

            frameData.pointables.Add(pointableName, new List<string>());
            frameData.pointables[pointableName].Add(point.Id.ToString());
            frameData.pointables[pointableName].Add(point.Direction.ToString());
            frameData.pointables[pointableName].Add(point.Hand.Id.ToString());
            frameData.pointables[pointableName].Add(point.Length.ToString());
            frameData.pointables[pointableName].Add(point.StabilizedTipPosition.ToString());
            frameData.pointables[pointableName].Add(point.TipPosition.ToString());
            frameData.pointables[pointableName].Add(point.TipVelocity.ToString());
            frameData.pointables[pointableName].Add(point.IsTool.ToString());

            string fingerName = "Finger " + count;

            Finger finger = new Finger(point);

            frameData.fingers.Add(fingerName, new List<string>());
            frameData.fingers[fingerName].Add(finger.JointPosition(Finger.FingerJoint.JOINT_MCP).ToString());
            frameData.fingers[fingerName].Add(finger.JointPosition(Finger.FingerJoint.JOINT_MCP).ToString());
            frameData.fingers[fingerName].Add(finger.JointPosition(Finger.FingerJoint.JOINT_PIP).ToString());
            frameData.fingers[fingerName].Add(finger.JointPosition(Finger.FingerJoint.JOINT_DIP).ToString());
            frameData.fingers[fingerName].Add(finger.JointPosition(Finger.FingerJoint.JOINT_TIP).ToString());

            int boneCount = 1;

            foreach (Bone.BoneType boneType in (Bone.BoneType[])Enum.GetValues(typeof(Bone.BoneType)))
            {
                string boneName = "Bone " + boneCount + " " + fingerName;

                frameData.bones.Add(boneName, new List<string>());

                Bone bone = finger.Bone(boneType);

                if (bone.IsValid)
                {
                   frameData.bones[boneName].Add(bone.Basis.xBasis.ToString());
                   frameData.bones[boneName].Add(bone.Basis.yBasis.ToString());
                   frameData.bones[boneName].Add(bone.Basis.zBasis.ToString());
                }

                boneCount++;
            }

            frameData.fingers[fingerName].Add(finger.Type.ToString());
            count++;
       }


        return frameData;
    }

    void OnApplicationQuit()
    {
        networkSocket.closeSocket();
    }
}