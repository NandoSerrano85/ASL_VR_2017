using Leap;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    private List<Frame> gestureFrames;

    public string GestureInputText { get { return gestureInputField.text; } set { gestureInputField.text = value; } }
    public bool GestureInputInteractable { get { return gestureInputField.interactable; } set { gestureInputField.interactable = value; } }
    public bool GestureSubmitButtonInteractable { get { return submitGestureButton.interactable; } set { submitGestureButton.interactable = value; } }
    public string GestureStatusText { get { return gestureStatusText.text; } set { gestureStatusText.text = value; } }

    private bool lastActiveViewState;
    private bool isGatheringFrames;
    private float recordingTimer;

    [SerializeField]
    private ModalDialog errorModalDialog;

    [SerializeField]
    private ModalDialog resetModalDialog;

    private ToggleableObject toggleableObject;

    private AudioSource buttonAudioSource;

    [SerializeField]
    private AudioClip buttonClickSound;

    [SerializeField]
    private GameObject processingFramesBackground;

    [SerializeField]
    private GameObject loadingCircle;

    [SerializeField]
    private Text processingFramesText;

    private void Start()
    {
        gestureInputFieldPlaceHolderText = gestureInputField.placeholder.GetComponent<Text>();
        buttonAudioSource = GetComponent<AudioSource>();

        toggleableObject = GetComponent<ToggleableObject>();
        lastActiveViewState = true;

        isGatheringFrames = false;
        recordingTimer = 5.0f;

        gestureFrames = new List<Frame>();

        if (controlsText != null)
        {
            controlsText.text = header + "\n\n" + takeSnapShotKey +
                " - Take a snapshot\n" + toggleableObject.toggleKey + " - Toggle view\n" +
                resetSnapshotKey + " - Reset Snapshot taken";

        }
    }

    private void Update()
    {
        if(isGatheringFrames && recordingTimer > 0)
            gatherGestureFrames();

        if (!GestureInputInteractable && !isGatheringFrames && Input.GetKeyDown(takeSnapShotKey))
        {
            isGatheringFrames = true;

            lastActiveViewState = false;
            toggleableObject.toggleObject(lastActiveViewState);
        }

        if (!gestureInputField.isFocused && Input.GetKeyDown(toggleableObject.toggleKey))
        {
            lastActiveViewState = !lastActiveViewState;
            toggleableObject.toggleObject(lastActiveViewState);
        }

        if (Input.GetKeyDown(resetSnapshotKey) && !gestureInputField.isFocused &&
           !errorModalDialog.isDialogActive() && gestureFrames.Count != 0)
        {
            resetModalDialog.showQuestionDialog(YesResetGestureEvent, NoResetGestureEvent);
        }

        if (gestureInputField.isFocused)
            gestureInputFieldPlaceHolderText.text = "";
        else
            gestureInputFieldPlaceHolderText.text = "Enter Gesture Name...";
    }

    private void gatherGestureFrames()
    {
        Frame frame = handController.GetFrame();

        if (frame.Hands.Count > 0)
        {
            gestureFrames.Add(frame);
        }

        recordingTimer -= Time.deltaTime;

        if (recordingTimer < 0)
        {
            isGatheringFrames = false;
            recordingTimer = 5.0f;
            GestureInputInteractable = true;
            GestureSubmitButtonInteractable = true;
            lastActiveViewState = true;
            toggleableObject.toggleObject(lastActiveViewState);
            GestureStatusText = "Process complete. Enter a name for the gesture.";
            StartCoroutine(resetText());
        }
    }

    private IEnumerator createGesture(string gestureName)
    {
        processingFramesBackground.SetActive(true);
        loadingCircle.SetActive(true);
        processingFramesText.text = "Creating Gesture...";

        Thread gestureThread = new Thread(() => processGestureFrames(gestureName));
        gestureThread.Start();

        while(gestureFrames.Count > 0)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1.0f);

        loadingCircle.SetActive(false);
        processingFramesText.text = "Gesture Created";

        yield return new WaitForSeconds(1.0f);

        processingFramesBackground.SetActive(false);

        GestureInputInteractable = false;
        GestureSubmitButtonInteractable = false;
    }

    public void sendGestureToDatabase()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);

        string gestureName = GestureInputText.Trim();

        if (!string.IsNullOrEmpty(gestureName))
        {
            StartCoroutine(createGesture(gestureName));
        }
        else
            errorModalDialog.showErrorDialog(OkEvent);

        GestureInputText = "";
    }

    private void processGestureFrames(string gestureName)
    {
        /*
         * I'm assuming that each frame is the same gesture and that the user didn't
         * change gesture in any of them. This means that the class label is the same
         * and I should call the gesture to class function once.
         */
        int gestureClassLabel = -1;

        foreach (Frame gestureFrame in gestureFrames)
        {
            FeatureVectorPreprocessor featureVectorPreProcessor = new FeatureVectorPreprocessor();

            FeatureVector featureVector = featureVectorPreProcessor.createFeatureVector(gestureFrame);

            featureVector.Gesture = gestureName;

            if (gestureClassLabel == -1)
            {
                gestureClassLabel = dataService.gestureToClassLabel(gestureName);
            }

            featureVector.GestureClassLabel = gestureClassLabel;

            dataService.InsertGesture(featureVector);
        }

        gestureFrames.Clear();
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
        GestureStatusText = "Gesture discarded. Ready for a new gesture.";
        gestureFrames.Clear();
        GestureInputInteractable = false;
        GestureSubmitButtonInteractable = false;
        StartCoroutine(resetText());
    }

    public void NoResetGestureEvent()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);
    }
}
