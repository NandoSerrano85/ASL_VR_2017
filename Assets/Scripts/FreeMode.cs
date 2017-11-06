using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Leap;

public class FreeMode : MonoBehaviour
{
    [SerializeField]
    private Classifier gestureClassifier;

    [SerializeField]
    private HandController handController;

    [SerializeField]
    private Text gestureSignText;

    public string GestureSign { get { return gestureSignText.text; } set {gestureSignText.text = value; } }

    public readonly static string coroutineName = "freeMode";

    public void startFreeMode()
    {
        StartCoroutine(coroutineName);
    }

    private IEnumerator freeMode()
    {
        while (true)
        {
            Frame frame = handController.GetFrame();

            if(frame.Hands.Count > 0)
            {
                GestureSign = gestureClassifier.classifyGesture(frame);
            }

            yield return null;
        }
    }

    public void stopFreeMode()
    {
        StopCoroutine(coroutineName);
    }
}
