using Leap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureSnapshot : MonoBehaviour
{
    [Multiline]
    public string header;
    public Text controlsText;

    [SerializeField]
    private InputField gestureInputField;

    [SerializeField]
    private DataService dataService;

    [SerializeField]
    private Button submitGestureButton;

    public KeyCode takeSnapShotKey = KeyCode.S;

    [SerializeField]
    private HandController handController;

    private FeatureVector featureVector;

    public string GestureInputText { get { return gestureInputField.text; } set { gestureInputField.text = value; } }
    public bool GestureInputInteractable { get { return gestureInputField.interactable; } set { gestureInputField.interactable = value; } }
    public bool GestureSubmitButtonInteractable { get { return submitGestureButton.interactable; } set { submitGestureButton.interactable = value; } }

    [SerializeField]
    private ErrorModalDialog errorModalDialog;

    [SerializeField]
    private Classifier classifier;

    private void Start()
    {
        if (controlsText != null) controlsText.text = header + "\n" + takeSnapShotKey + " - Take A Snapshot\n";
    }

    private void Update()
    {
        if (!GestureInputInteractable && Input.GetKeyDown(takeSnapShotKey))
        {
            Frame frame = handController.GetFrame();

            if (frame.Hands.Count > 0)
            {
                takeSnapShot(frame);
            }
        }
    }

    private void takeSnapShot(Frame frame)
    {
        FeatureVectorProcessor featureVectorProcessor = new FeatureVectorProcessor();
        List<Vector2> featurePoints = featureVectorProcessor.getFeaturePoints(frame);

        featureVector = featureVectorProcessor.createFeatureVector(featurePoints);

        GestureInputInteractable = true;
        GestureSubmitButtonInteractable = true;
    }

    public void sendGestureToDatabase()
    {
        string gestureName = GestureInputText.Trim(); 

        if(!string.IsNullOrEmpty(gestureName))
        {
            featureVector.Gesture = gestureName;

            classifier.addFeatureVector(featureVector);
            dataService.InsertGesture(featureVector);

            GestureInputInteractable = false;
            GestureSubmitButtonInteractable = false;
        }
        else
        {
            errorModalDialog.showErrorDialog();
        }

        GestureInputText = "";
    }
}
