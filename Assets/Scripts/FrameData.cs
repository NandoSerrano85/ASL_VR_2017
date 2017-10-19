using System;

[Serializable]
public class FrameData
{
    public long FrameID { get; set;}
    public float PinchStrength { get; set; }
    public float GrabStrength { get; set; }
    public float AverageDistance { get; set; }
    public float AverageSpread { get; set; }
    public float AverageTriSpread { get; set; }

    public void resetFrameData()
    {
        FrameID = -1;
        PinchStrength = 0.0f;
        GrabStrength = 0.0f;
        AverageDistance = 0.0f;
        AverageSpread = 0.0f;
        AverageTriSpread = 0.0f;
    }
}
