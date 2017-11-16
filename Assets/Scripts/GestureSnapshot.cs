using Leap;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ModalDialog))]
public class GestureSnapshot : MonoBehaviour
{
    [Multiline]
    [SerializeField]
    private string header;

    [SerializeField]
    public Text controlsText;

    [SerializeField]
    private InputField gestureInputField;

    [SerializeField]
    private DataService dataService;

    [SerializeField]
    private Button submitGestureButton;

    [SerializeField]
    private KeyCode takeSnapShotKey = KeyCode.S;

    [SerializeField]
    private HandController handController;

    private FeatureVector featureVector;

    public string GestureInputText { get { return gestureInputField.text; } set { gestureInputField.text = value; } }
    public bool GestureInputInteractable { get { return gestureInputField.interactable; } set { gestureInputField.interactable = value; } }
    public bool GestureSubmitButtonInteractable { get { return submitGestureButton.interactable; } set { submitGestureButton.interactable = value; } }

    private ModalDialog errorModalDialog;

    private void Start()
    {
        errorModalDialog = GetComponent<ModalDialog>();
        if (controlsText != null) controlsText.text = header + "\n" + takeSnapShotKey + " - Take a snapshot\n";
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
        FeatureVectorPreProcessor featureVectorPreProcessor = new FeatureVectorPreProcessor();

        featureVector = featureVectorPreProcessor.createFeatureVector(frame);

        GestureInputInteractable = true;
        GestureSubmitButtonInteractable = true;
    }

    public void sendGestureToDatabase()
    {
        string gestureName = GestureInputText.Trim(); 

        if(!string.IsNullOrEmpty(gestureName))
        {
            featureVector.Gesture = gestureName;
            featureVector.GestureClassLabel = dataService.gestureToClassLabel(gestureName);

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
