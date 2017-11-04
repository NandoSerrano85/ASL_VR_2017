using UnityEngine;
using Leap;
using System.Collections.Generic;

public class Classifier
{
    //private Controller leapController;
    //private long currentFrameID;
    public string Gesture { get; set; }
    //private bool isClassifyingGesture;
    //[SerializeField] private Text gestureSignText;

    public Classifier()
    {
        //leapController = GetComponent<HandController>().GetLeapController();
        //currentFrameID = leapController.Frame().Id;
        //isClassifyingGesture = false;
        Gesture = "";
    }

    void classifyGesture(Frame frame)
    {
        //Frame frame = leapController.Frame();

        //if (currentFrameID == frame.Id)
        //    return;

        //currentFrameID = frame.Id; // save id

        FeatureVectorProcessor featureVectorProcessor = new FeatureVectorProcessor();
        List<Vector2> featurePoints = featureVectorProcessor.getFeaturePoints(frame);

        FeatureVector featureVector = featureVectorProcessor.createFeatureVector(featurePoints);

        DataService dataService = new DataService();

        List<FeatureVector> listVectors = dataService.getAllFeatureVectors();
        float score = 0.0f;
        float newScore = 0.0f;

        foreach (FeatureVector vector in listVectors)
        {
            newScore = FeatureVectorScorer.euclideanSimilarity(featureVector.createDistanceVector(), vector.createDistanceVector());

            if (newScore > score)
            {
                score = newScore;
                Gesture = vector.Gesture;
            }
        }

        //Thread gestureThread = new Thread(() => classifyGesture(featureVector));
        //gestureThread.Start();

        //isClassifyingGesture = true;

        //if(gesture != "")
        //{
        //    gestureSignText.text = gesture;
        //    isClassifyingGesture = false;
        //    gesture = "";
        //}
    }

    // either get the entire DB or you go one by one.
    // compare the current featureVector with the one in the database.
    // call either euclidean or cosine.
    // if the score is greater than the currents score, store the score and the feature vector from the DB.
    // Repeat until you've gone through the entire DB.
}