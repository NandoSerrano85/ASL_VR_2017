using UnityEngine;
using Leap;
using System.Collections.Generic;

public class Classifier : MonoBehaviour
{
    [SerializeField]
    private DataService dataService;

    private List<FeatureVector> featureVectors;
    private FeatureVectorProcessor featureVectorProcessor;

    private void Start()
    {
        featureVectorProcessor = new FeatureVectorProcessor();
        featureVectors = dataService.getAllFeatureVectors();
    }

    /*
     * Retrieve all the feature vectors from the local database. Compare the current featureVector with
     * each feature vector that is in the database. Calculate the score for each one using either euclidean
     * similarity or cosine similarity. The one with the highest score is the gesture that we return.
     */
    public string classifyGesture(Frame frame)
    {
        List<Vector2> featurePoints = featureVectorProcessor.getFeaturePoints(frame);
        FeatureVector featureVector = featureVectorProcessor.createFeatureVector(featurePoints);

        string gesture = "";
        float score = 0.0f;

        foreach (FeatureVector vector in featureVectors)
        {
            float newScore = FeatureVectorScorer.euclideanSimilarity(featureVector.createDistanceVector(), vector.createDistanceVector());

            if (newScore > score)
            {
                score = newScore;
                gesture = vector.Gesture;
            }
        }

        return gesture;
    }

    public void addFeatureVector(FeatureVector featureVector)
    {
        featureVectors.Add(featureVector);
    }
}