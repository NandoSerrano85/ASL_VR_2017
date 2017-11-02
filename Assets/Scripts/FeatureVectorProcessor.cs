using Leap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeatureVectorProcessor
{
    public readonly static int NUM_FEATURES = 5;

    public List<Vector2> getFeaturePoints(Frame frame)
    {
        List<Vector2> featurePoints = new List<Vector2>();

        foreach (Hand hand in frame.Hands)
        {
            featurePoints.Add(UnityVectorExtension.ToUnityScaled(hand.PalmPosition));

            foreach (Finger finger in hand.Fingers)
            {
                featurePoints.Add(UnityVectorExtension.ToUnityScaled(finger.TipPosition));
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
    public List<float> createFeatureVector(List<Vector2> featurePoints)
    {
        List<float> featureVector = new List<float>();

        for (int i = 0; i < NUM_FEATURES; i++)
        {
           featureVector.Add(Vector2.Distance(featurePoints[0], featurePoints[i + 1]));
        }

        return normalizeFeatureVector(featureVector);
    }

    private List<float> normalizeFeatureVector(List<float> featureVector)
    {
        List<float> normalizedFeatureVector = new List<float>();

        float minDistance = featureVector.Min();
        float maxDistance = featureVector.Max() - minDistance;

        foreach (float distance in featureVector)
        {
            normalizedFeatureVector.Add(normalizeDistance(distance, minDistance, maxDistance));
        }

        return normalizedFeatureVector;
    }

    private float normalizeDistance(float distance, float minDistance, float maxDistance)
    {
        return (distance - minDistance) / maxDistance;
    }
}
