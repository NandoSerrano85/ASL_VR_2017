using Leap;
using System.Collections.Generic;

public class FeatureVector
{
    public readonly static int NUM_FEATURES = 8;

    public List<Vector> getFeaturePoints(Frame frame)
    {
        List<Vector> featurePoints = new List<Vector>();

        foreach (Hand hand in frame.Hands)
        {
            featurePoints.Add(hand.StabilizedPalmPosition);

            foreach (Finger finger in hand.Fingers)
            {
                featurePoints.Add(finger.StabilizedTipPosition);
            }
        }

        return featurePoints;
    }

    /*
     * featurePoints[0] = Palm Vector
     * featurePoints[1] = Thumb Finger Vector
     * featurePoints[2] = Index Finger Vector
     * featurePoints[3] = Middle Finger Vector
     * featurePoints[4] = Ring Finger Vector
     * featurePoints[5] = Pinky Finger Vector
     */
    public List<float> createFeatureVector(List<Vector> featurePoints)
    {
        List<float> featureVector = new List<float>();

        for (int j = 0; j < NUM_FEATURES; j++)
        {
            if (j < 5)
                featureVector.Add(featurePoints[0].DistanceTo(featurePoints[j + 1]));
            else
            {
                int offset = NUM_FEATURES - j;
                featureVector.Add(featurePoints[offset + 1].DistanceTo(featurePoints[offset + 2]));
            }
        }
        return featureVector;
    }
}
