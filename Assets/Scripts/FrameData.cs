using System.Collections;
using UnityEngine;
using Leap;

[RequireComponent(typeof(HandController))]
public class FrameData : MonoBehaviour
{
    private HandController handController;
    public Frame Frame { get; set; }
    private long currentFrameID;

    // Use this for initialization
    void Start()
    {
        handController = GetComponent<HandController>();
        Frame = handController.GetFrame();
        currentFrameID = Frame.Id;
    }

    // Update is called once per frame
    void Update()
    {
        Frame frame = handController.GetFrame();

        if (frame.Id == currentFrameID)
            return;

        Frame = frame;
        currentFrameID = Frame.Id;
    }

    public bool isHandVisible()
    {
        return Frame.Hands.Count > 0;
    }
}
