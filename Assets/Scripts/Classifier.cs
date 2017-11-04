using UnityEngine;
using Leap;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UI;

[RequireComponent(typeof(HandController))]
public class Classifier : MonoBehaviour
{
    private Controller leapController;
    private long currentFrameID;
    private DataService dataService;
    private string gesture;
    private bool isClassifyingGesture;
    [SerializeField] private Text gestureSignText;

    void Start()
    {
        leapController = GetComponent<HandController>().GetLeapController();
        currentFrameID = leapController.Frame().Id;
        dataService = new DataService();
        isClassifyingGesture = false;
        gesture = "";
    }

    void Update()
    {
        Frame frame = leapController.Frame();

        if (currentFrameID == frame.Id)
            return;

        currentFrameID = frame.Id; // save id

        if (frame.Hands.Count > 0 & !isClassifyingGesture)
        {
            FeatureVectorProcessor featureVectorProcessor = new FeatureVectorProcessor();
            List<Vector2> featurePoints = featureVectorProcessor.getFeaturePoints(frame);

            FeatureVector featureVector = featureVectorProcessor.createFeatureVector(featurePoints);

            Thread gestureThread = new Thread(() => classifyGesture(featureVector));
            gestureThread.Start();

            isClassifyingGesture = true;
        }

        if(gesture != "")
        {
            gestureSignText.text = gesture;
            isClassifyingGesture = false;
            gesture = "";
        }
    }

    // either get the entire DB or you go one by one.
    // compare the current featureVector with the one in the database.
    // call either euclidean or cosine.
    // if the score is greater than the currents score, store the score and the feature vector from the DB.
    // Repeat until you've gone through the entire DB.
    // return gesture.
    private void classifyGesture(FeatureVector featureVector)
    {
        
    }
}