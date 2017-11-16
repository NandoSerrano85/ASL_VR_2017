using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using System.Collections.Generic;

public class FreeMode : MonoBehaviour
{
    [SerializeField]
    private GestureClassifier gestureClassifier;

    [SerializeField]
    private HandController handController;

    [SerializeField]
    private Text gestureSignText;

    public string GestureSign { get { return gestureSignText.text; } set {gestureSignText.text = value; } }

    public readonly static string coroutineName = "freeMode";

    private FeatureVectorPreProcessor featureVectorPreProcessor;

    public void startFreeMode()
    {
        featureVectorPreProcessor = new FeatureVectorPreProcessor();
        StartCoroutine(coroutineName);
    }

    private IEnumerator freeMode()
    {
        while (true)
        {
            Frame frame = handController.GetFrame();

            if(frame.Hands.Count > 0 && gestureClassifier.ModelExists)
            {
                FeatureVector featureVector = featureVectorPreProcessor.createFeatureVector(frame);
                GestureSign = gestureClassifier.classifyGesture(featureVector.createInputVector());
            }

            yield return null;
        }
    }

    public void stopFreeMode()
    {
        StopCoroutine(coroutineName);
    }
}
