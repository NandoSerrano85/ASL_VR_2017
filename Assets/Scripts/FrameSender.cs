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

        Dictionary<string, string> frameDictionary = getFrameData(currentFrame);

        string jsonString = JsonConvert.SerializeObject(frameDictionary, Formatting.Indented);
        networkSocket.InputBuffer = jsonString;
    }

    private Dictionary<string, string> getFrameData(Frame currentFrame)
    {
        Dictionary<string, string> frameData = new Dictionary<string, string>();

        foreach (Hand hand in currentFrame.Hands)
        {
            frameData["Hand id"] = hand.Id.ToString();
            frameData["Palm position"] = hand.PalmPosition.ToString();
            frameData["Fingers"] = hand.Fingers.Count.ToString();

            // Get the hand's normal vector and direction
            Vector normal = hand.PalmNormal;
            Vector direction = hand.Direction;

            float handpitch = direction.Pitch * 180.0f / (float)Math.PI;
            float roll = normal.Roll * 180.0f / (float)Math.PI;
            float yaw = direction.Yaw * 180.0f / (float)Math.PI;

            frameData["Hand pitch"] = handpitch.ToString();
            frameData["Roll"] = roll.ToString();
            frameData["Yaw"] = yaw.ToString();

            // Get the Arm bone
            Arm arm = hand.Arm;

            frameData["Arm Direction"] = arm.Direction.ToString();
            frameData["Wrist Position"] = arm.WristPosition.ToString();
            frameData["Elbow Position"] = arm.ElbowPosition.ToString();

            // Get fingers
            foreach (Finger finger in hand.Fingers)
            {
                frameData["Finger id"] = finger.Id.ToString();
                frameData["Finger Type"] = finger.Type.ToString();
                frameData["Length"] = finger.Length.ToString();
                frameData["Width"] = finger.Width.ToString();

                // Get finger bones
                Bone bone;

                for (int b = 0; b < 4; b++)
                {
                    bone = finger.Bone((Bone.BoneType)b);

                    frameData["Bone"] = bone.Type.ToString();
                    frameData["start"] = bone.PrevJoint.ToString();
                    frameData["end"] = bone.NextJoint.ToString();
                    frameData["direction"] = bone.Direction.ToString();
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
