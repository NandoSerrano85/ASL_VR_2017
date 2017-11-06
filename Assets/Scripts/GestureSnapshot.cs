using Leap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HandController))]
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

    private HandController _controller;

    private FeatureVector featureVector;

    public string GestureInputText { get { return gestureInputField.text; } set { gestureInputField.text = value; } }
    public bool GestureInputInteractable { get { return gestureInputField.interactable; } set { gestureInputField.interactable = value; } }
    public bool GestureSubmitButtonInteractable { get { return submitGestureButton.interactable; } set { submitGestureButton.interactable = value; } }

    void Start()
    {
        _controller = GetComponent<HandController>();

        if (controlsText != null) controlsText.text = header + "\n" + takeSnapShotKey + " - Take A Snapshot\n";
    }

    void Update()
    {
        if (Input.GetKeyDown(takeSnapShotKey) && !GestureInputInteractable)
        {
            Frame frame = _controller.GetFrame();

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
        if(!string.IsNullOrEmpty(GestureInputText))
        {
            featureVector.Gesture = GestureInputText.Trim();

            dataService.InsertGesture(featureVector);

            GestureInputText = "";
            GestureInputInteractable = false;
            GestureSubmitButtonInteractable = false;
        }
    }
}
