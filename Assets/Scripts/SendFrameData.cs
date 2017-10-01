using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
using System;

public class SendFrameData: MonoBehaviour
{
    private LeapServiceProvider leapProvider;
    private NetworkSocket networkSocket;
    private long currentFrameID;

    void Start()
    {
        leapProvider = GetComponent<LeapServiceProvider>();
        networkSocket = new NetworkSocket();
        currentFrameID = leapProvider.CurrentFrame.Id;
    }

    void Update()
    {
        Frame currentFrame = leapProvider.CurrentFrame;

        if (currentFrameID == currentFrame.Id)
            return;

        currentFrameID = currentFrame.Id;

        Dictionary<string, string> frameData = getFrameData(currentFrame);

        string jsonString = JsonUtility.ToJson(frameData);
        networkSocket.writeSocket(jsonString);
    }

    private Dictionary<string, string> getFrameData(Frame currentFrame)
    {
        Dictionary<string, string> frameData = new Dictionary<string, string>();

        foreach (Hand hand in currentFrame.Hands)
        {
            frameData.Add("Hand id", hand.Id.ToString());
            frameData.Add("Palm position", hand.PalmPosition.ToString());
            frameData.Add("Fingers", hand.Fingers.Count.ToString());

            // Get the hand's normal vector and direction
            Vector normal = hand.PalmNormal;
            Vector direction = hand.Direction;

            float handpitch = direction.Pitch * 180.0f / (float)Math.PI;
            float roll = normal.Roll * 180.0f / (float)Math.PI;
            float yaw = direction.Yaw * 180.0f / (float)Math.PI;

            frameData.Add("Hand pitch", handpitch.ToString());
            frameData.Add("Roll", roll.ToString());
            frameData.Add("Yaw", yaw.ToString());

            // Get the Arm bone
            Arm arm = hand.Arm;

            frameData.Add("Arm Direction", arm.Direction.ToString());
            frameData.Add("Wrist Position", arm.WristPosition.ToString());
            frameData.Add("Elbow Position", arm.ElbowPosition.ToString());

            // Get fingers
            foreach (Finger finger in hand.Fingers)
            {
                frameData.Add("Finger id", finger.Id.ToString());
                frameData.Add("Finger Type", finger.Type.ToString());
                frameData.Add("Length", finger.Length.ToString());
                frameData.Add("Width", finger.Width.ToString());

                // Get finger bones
                Bone bone;

                for (int b = 0; b < 4; b++)
                {
                    bone = finger.Bone((Bone.BoneType)b);

                    frameData.Add("Bone", bone.Type.ToString());
                    frameData.Add("start", bone.PrevJoint.ToString());
                    frameData.Add("end", bone.NextJoint.ToString());
                    frameData.Add("direction", bone.Direction.ToString());
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
