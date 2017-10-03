using System.Collections.Generic;
using System;
using UnityEngine;
using Leap;
using Leap.Unity;
using Newtonsoft.Json;

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

        Dictionary<string, List<string>> frameDictionary = getFrameData(currentFrame);

        string jsonString = JsonConvert.SerializeObject(frameDictionary, Formatting.Indented);
        networkSocket.InputBuffer = jsonString;
    }

    private Dictionary<string, List<string>> getFrameData(Frame currentFrame)
    {
        Dictionary<string, List<string>> frameData = new Dictionary<string, List<string>>();

        frameData.Add("Hand Id", new List<string>());
        frameData.Add("Palm Position", new List<string>());
        frameData.Add("Fingers", new List<string>());

        frameData.Add("Hand Pitch", new List<string>());
        frameData.Add("Roll", new List<string>());
        frameData.Add("Yaw", new List<string>());

        frameData.Add("Arm Direction", new List<string>());
        frameData.Add("Wrist Position", new List<string>());
        frameData.Add("Elbow Position", new List<string>());

        frameData.Add("Finger Id", new List<string>());
        frameData.Add("Finger Type", new List<string>());
        frameData.Add("Finger Length", new List<string>());
        frameData.Add("Finger Width", new List<string>());

        frameData.Add("Bone Type", new List<string>());
        frameData.Add("Bone Start", new List<string>());
        frameData.Add("Bone End", new List<string>());
        frameData.Add("Bone Direction", new List<string>());

        foreach (Hand hand in currentFrame.Hands)
        {
            frameData["Hand Id"].Add(hand.Id.ToString());
            frameData["Palm Position"].Add(hand.PalmPosition.ToString());
            frameData["Fingers"].Add(hand.Fingers.Count.ToString());

            // Get the hand's normal vector and direction
            Vector normal = hand.PalmNormal;
            Vector direction = hand.Direction;

            float handpitch = direction.Pitch * 180.0f / (float)Math.PI;
            float roll = normal.Roll * 180.0f / (float)Math.PI;
            float yaw = direction.Yaw * 180.0f / (float)Math.PI;

            frameData["Hand Pitch"].Add(handpitch.ToString());
            frameData["Roll"].Add(roll.ToString());
            frameData["Yaw"].Add(yaw.ToString());

            // Get the Arm bone
            Arm arm = hand.Arm;

            frameData["Arm Direction"].Add(arm.Direction.ToString());
            frameData["Wrist Position"].Add(arm.WristPosition.ToString());
            frameData["Elbow Position"].Add(arm.ElbowPosition.ToString());

            // Get fingers
            foreach (Finger finger in hand.Fingers)
            {
                frameData["Finger Id"].Add(finger.Id.ToString());
                frameData["Finger Type"].Add(finger.Type.ToString());
                frameData["Finger Length"].Add(finger.Length.ToString());
                frameData["Finger Width"].Add(finger.Width.ToString());

                // Get finger bones
                Bone bone;

                for (int b = 0; b < 4; b++)
                {
                    bone = finger.Bone((Bone.BoneType)b);

                    frameData["Bone Type"].Add(bone.Type.ToString());
                    frameData["Bone Start"].Add(bone.PrevJoint.ToString());
                    frameData["Bone End"].Add(bone.NextJoint.ToString());
                    frameData["Bone Direction"].Add(bone.Direction.ToString());
                }
            }
        }

        return frameData;
    }

    void OnApplicationQuit()
    {
        networkSocket.closeSocket();
    }
}
