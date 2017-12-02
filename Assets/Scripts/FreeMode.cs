using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Leap;

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

    private FeatureVectorPreprocessor featureVectorPreprocessor;

    public void startFreeMode()
    {
        featureVectorPreprocessor = new FeatureVectorPreprocessor();
        StartCoroutine(coroutineName);
    }

    private IEnumerator freeMode()
    {
        while (true)
        {
            Frame frame = handController.GetFrame();

            if(gestureClassifier.ModelExists && frame.Hands.Count > 0)
            {
                FeatureVector featureVector = featureVectorPreprocessor.createFeatureVector(frame);
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
