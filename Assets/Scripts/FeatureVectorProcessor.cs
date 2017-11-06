using Leap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeatureVectorProcessor
{
    // The number of features in the feature vector.
    public readonly static int NUM_FEATURES = 5;

    /*
     * Creates a list of 6  X,Y coordinate points in the following order:
     * 
     * featurePoints[0] = Palm Position Vector
     * featurePoints[1] = Thumb Tip Position Finger Vector
     * featurePoints[2] = Index Tip Position Finger Vector
     * featurePoints[3] = Middle Tip Position Finger Vector
     * featurePoints[4] = Ring Tip Position Finger Vector
     * featurePoints[5] = Pinky Tip Position Finger Vector
     */
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
     * This function creates the distance between the palm of the hand and tip of each finger.
     */
    public FeatureVector createFeatureVector(List<Vector2> featurePoints)
    {
        List<float> featureVectorList = new List<float>();

        for (int i = 0; i < NUM_FEATURES; i++)
        {
           featureVectorList.Add(Vector2.Distance(featurePoints[0], featurePoints[i + 1]));
        }

        featureVectorList = normalizeFeatureVector(featureVectorList);

        FeatureVector featureVector = new FeatureVector()
        {
            PalmToThumb = featureVectorList[0],
            PalmToIndex = featureVectorList[1],
            PalmToMiddle = featureVectorList[2],
            PalmToRing = featureVectorList[3],
            PalmToPinky = featureVectorList[4]
        };

        return featureVector;
    }

    /*
     * Each distance in the feature vector is normalize between [0, 1] so that it doesn't matter
     * how big the hand is. The formula that was used to normalize the distance was
     * 
     * NormalizedDistance = distance - minimum distance / max distance
     */
    private List<float> normalizeFeatureVector(List<float> featureVectorList)
    {
        List<float> normalizedFeatureVectorList = new List<float>();

        float minDistance = featureVectorList.Min();
        float maxDistance = featureVectorList.Max() - minDistance;

        foreach (float distance in featureVectorList)
        {
            normalizedFeatureVectorList.Add(normalizeDistance(distance, minDistance, maxDistance));
        }

        return normalizedFeatureVectorList;
    }

    private float normalizeDistance(float distance, float minDistance, float maxDistance)
    {
        return (distance - minDistance) / maxDistance;
    }
}
