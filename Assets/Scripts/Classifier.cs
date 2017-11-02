﻿using UnityEngine;
using Leap;
using System.Collections.Generic;

[RequireComponent(typeof(HandController))]
public class Classifier : MonoBehaviour
{
    private Controller leapController;
    private long currentFrameID;
    private List<float> featureVector1;
    private List<float> featureVector2;

    void Start()
    {
        leapController = GetComponent<HandController>().GetLeapController();
        currentFrameID = leapController.Frame().Id;
    }

    void Update()
    {
        Frame frame = leapController.Frame();

        if (currentFrameID == frame.Id)
            return;

        currentFrameID = frame.Id;

        /*
         * 
         * This section is for testing purposes until a database is created.
         */
        if(frame.Hands.Count > 0 && Input.GetKeyDown(KeyCode.Z))
        {
            FeatureVectorProcessor featureVector = new FeatureVectorProcessor();
            List<Vector2> featurePoints = featureVector.getFeaturePoints(frame);

            featureVector1 = featureVector.createFeatureVector(featurePoints);
        }

        if(frame.Hands.Count > 0 && Input.GetKeyDown(KeyCode.X))
        {
            FeatureVectorProcessor featureVector = new FeatureVectorProcessor();
            List<Vector2> featurePoints = featureVector.getFeaturePoints(frame);

            featureVector2 = featureVector.createFeatureVector(featurePoints);
        }

        // There is at least one hand in the scene.
        if (frame.Hands.Count > 0 && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(FeatureVectorScorer.euclideanSimilarity(featureVector1, featureVector2));
            Debug.Log(FeatureVectorScorer.cosineSimilarity(featureVector1, featureVector2));
        }
    }
}