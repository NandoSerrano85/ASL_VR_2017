using Leap;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleableObject))]
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
    private Text gestureStatusText;

    private Text gestureInputFieldPlaceHolderText;

    [SerializeField]
    private DataService dataService;

    [SerializeField]
    private Button submitGestureButton;

    [SerializeField]
    private KeyCode takeSnapShotKey = KeyCode.S;

    [SerializeField]
    private KeyCode resetSnapshotKey = KeyCode.R;

    [SerializeField]
    private HandController handController;

    private FeatureVector featureVector;

    public string GestureInputText { get { return gestureInputField.text; } set { gestureInputField.text = value; } }
    public bool GestureInputInteractable { get { return gestureInputField.interactable; } set { gestureInputField.interactable = value; } }
    public bool GestureSubmitButtonInteractable { get { return submitGestureButton.interactable; } set { submitGestureButton.interactable = value; } }
    public string GestureStatusText { get { return gestureStatusText.text; } set { gestureStatusText.text = value;} }

    private bool lastActiveViewState;

    [SerializeField]
    private ModalDialog errorModalDialog;

    [SerializeField]
    private ModalDialog resetModalDialog;

    private ToggleableObject toggleableObject;

    private AudioSource buttonAudioSource;

    [SerializeField]
    private AudioClip buttonClickSound;

    private void Start()
    {
        gestureInputFieldPlaceHolderText = gestureInputField.placeholder.GetComponent<Text>();
        buttonAudioSource = GetComponent<AudioSource>();

        toggleableObject = GetComponent<ToggleableObject>();
        lastActiveViewState = true;

        if (controlsText != null)
        {
            controlsText.text = header + "\n\n" + takeSnapShotKey +
                " - Take a snapshot\n" + toggleableObject.toggleKey + " - Toggle view\n" +
                resetSnapshotKey + " - Reset Snapshot taken";

        }
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

        if (!gestureInputField.isFocused && Input.GetKeyDown(toggleableObject.toggleKey))
        {
            lastActiveViewState = !lastActiveViewState;
            toggleableObject.toggleObject(lastActiveViewState);
        }

        if(Input.GetKeyDown(resetSnapshotKey) && !gestureInputField.isFocused && 
           !errorModalDialog.isDialogActive() && featureVector != null)
        {
            resetModalDialog.showQuestionDialog(YesResetGestureEvent, NoResetGestureEvent);
        }

        if (gestureInputField.isFocused)
            gestureInputFieldPlaceHolderText.text = "";
        else
            gestureInputFieldPlaceHolderText.text = "Enter Gesture Name...";
    }

    private void takeSnapShot(Frame frame)
    {
        FeatureVectorPreprocessor featureVectorPreProcessor = new FeatureVectorPreprocessor();

        featureVector = featureVectorPreProcessor.createFeatureVector(frame);

        GestureInputInteractable = true;
        GestureSubmitButtonInteractable = true;
    }

    public void sendGestureToDatabase()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);

        string gestureName = GestureInputText.Trim(); 

        if(!string.IsNullOrEmpty(gestureName))
        {
            featureVector.Gesture = gestureName;
            featureVector.GestureClassLabel = dataService.gestureToClassLabel(gestureName);

            dataService.InsertGesture(featureVector);

            GestureInputInteractable = false;
            GestureSubmitButtonInteractable = false;
            GestureStatusText = "Gesture created. Ready for a new gesture.";
            StartCoroutine(resetText());
        }
        else
            errorModalDialog.showErrorDialog(OkEvent);

        GestureInputText = "";
    }

    private IEnumerator resetText()
    {
        yield return new WaitForSeconds(2);
        GestureStatusText = "";
    }

    public void OkEvent()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);
    }

    public void YesResetGestureEvent()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);
        GestureStatusText = "Ready for a new gesture.";
        featureVector = null;
        GestureInputInteractable = false;
        GestureSubmitButtonInteractable = false;
        StartCoroutine(resetText());
    }

    public void NoResetGestureEvent()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);
    }
}
