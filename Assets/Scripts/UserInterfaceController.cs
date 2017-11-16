using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour
{
    private AudioSource buttonAudioSource;

    [SerializeField]
    private AudioClip buttonClickSound;

    [SerializeField]
    private Animator userInterfaceViewAnimator;

    [SerializeField]
    private HandController handController;

    [SerializeField]
    private GestureSnapshot snapshotControls;

    [SerializeField]
    private FreeMode freeMode;

    [SerializeField]
    private GestureClassifier gestureClassifier;

    [SerializeField]
    private InputField gestureInputField;

    [SerializeField]
    private Button submitButton;

    [SerializeField]
    private GameObject trainingClassifierBackground;

    [SerializeField]
    private GameObject loadingCircle;

    [SerializeField]
    private Text trainingStatusText;

    public static readonly string[] animationTriggers = { "SlideOutAndFadeInTrigger", "SlideInFadeOutTrigger" };

    private void Start()
    {
        buttonAudioSource = GetComponent<AudioSource>();
        freeMode.startFreeMode();
    }

    public void importGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);

        if (!handController.IsConnected())
            return;
    }

    public void recordGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);

        if (!handController.IsConnected())
            return;
    }

    public void createGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);

        if (!handController.IsConnected())
            return;

        userInterfaceViewAnimator.SetTrigger(animationTriggers[0]);
        snapshotControls.enabled = true;

        freeMode.stopFreeMode();
        gestureInputField.interactable = false;
        submitButton.interactable = false;
    }

    public void trainClassifierClick()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);

        if (!handController.IsConnected())
            return;

        StartCoroutine(startTraining());
    }

    private IEnumerator startTraining()
    {
        trainingClassifierBackground.SetActive(true);
        loadingCircle.SetActive(true);
        trainingStatusText.text = "Training Classifier...";

        gestureClassifier.ModelExists = false;
        StartCoroutine(gestureClassifier.trainClassifier());

        while(!gestureClassifier.ModelExists)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.5f);

        loadingCircle.SetActive(false);
        trainingStatusText.text = "Training Complete";

        yield return new WaitForSeconds(0.5f);

        trainingClassifierBackground.SetActive(false);
    }

    public void returnToPreviousMenu(string methodName)
    {
        Invoke(methodName, 0.0f);
    }

    private void snapShotViewToGestureView()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);
        userInterfaceViewAnimator.SetTrigger(animationTriggers[1]);

        snapshotControls.GestureInputText = "";
        snapshotControls.GestureInputInteractable = false;
        snapshotControls.GestureSubmitButtonInteractable = false;
        snapshotControls.enabled = false;

        freeMode.startFreeMode();
        freeMode.GestureSign = "No Gesture Detected";
    }
}
