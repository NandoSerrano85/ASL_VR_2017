using System.Collections.Generic;
using UnityEngine;

public static class FeatureVectorScorer
{
    public static float euclideanSimilarity(List<float> vectorA, List<float> vectorB)
    {
        float euclideanDistance = 0.0f;

        for (int i = 0; i < FeatureVectorProcessor.NUM_FEATURES; i++)
        {
            euclideanDistance += Mathf.Pow(vectorA[i] - vectorB[i], 2);
        }

        return 1 / (1 + Mathf.Sqrt(euclideanDistance));
    }

    public static float cosineSimilarity(List<float> vectorA, List<float> vectorB)
    {
        float dotProductResult = 0.0f;
        float vectorAMag = 0.0f;
        float vectorBMag = 0.0f;

        for (int i = 0; i < FeatureVectorProcessor.NUM_FEATURES; i++)
        {
            dotProductResult += (vectorA[i] * vectorB[i]);
            vectorAMag += Mathf.Pow(vectorA[i], 2);
            vectorBMag += Mathf.Pow(vectorB[i], 2);
        }

        return dotProductResult / (Mathf.Sqrt(vectorAMag) * Mathf.Sqrt(vectorBMag));
    }
}
