using System.Collections.Generic;
using UnityEngine;

public static class FeatureVectorScorer
{
    public static float euclideanSimilarity(List<float> distanceVector1, List<float> distanceVector2)
    {
        float euclideanDistance = 0.0f;

        for (int i = 0; i < FeatureVectorProcessor.NUM_FEATURES; i++)
        {
            euclideanDistance += Mathf.Pow(distanceVector1[i] - distanceVector2[i], 2);
        }

        // as number gets closer to 0 the gesture is completely different 
        return 1 / (1 + Mathf.Sqrt(euclideanDistance));
    }

    public static float cosineSimilarity(List<float> distanceVector1, List<float> distanceVector2)
    {
        float dotProduct = 0.0f;
        float vectorAMag = 0.0f;
        float vectorBMag = 0.0f;

        for (int i = 0; i < FeatureVectorProcessor.NUM_FEATURES; i++)
        {
            dotProduct += (distanceVector1[i] * distanceVector2[i]);
            vectorAMag += Mathf.Pow(distanceVector1[i], 2);
            vectorBMag += Mathf.Pow(distanceVector2[i], 2);
        }

        return dotProduct / (Mathf.Sqrt(vectorAMag) * Mathf.Sqrt(vectorBMag));
    }
}
