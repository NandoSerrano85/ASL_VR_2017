using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour
{
    private AudioSource buttonAudioSource;

    [SerializeField]
    private AudioClip[] buttonSoundClips;

    [SerializeField]
    private Animator userInterfaceViewAnimator;

    [SerializeField]
    private HandController handController;

    [SerializeField]
    private GestureSnapshot snapshotControls;

    [SerializeField]
    private FreeMode freeMode;

    public static readonly string[] animationTriggers = { "SlideOutAndFadeInTrigger", "SlideInFadeOutTrigger" };

    private void Start()
    {
        buttonAudioSource = GetComponent<AudioSource>();
        freeMode.startFreeMode();
    }

    public void importGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);

        if (!handController.IsConnected())
            return;
    }

    public void recordGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);

        if (!handController.IsConnected())
            return;

        userInterfaceViewAnimator.SetTrigger(animationTriggers[0]);
        snapshotControls.enabled = true;

        freeMode.stopFreeMode();
    }

    public void createGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);

        if (!handController.IsConnected())
            return;
    }

    public void returnToPreviousMenu(string methodName)
    {
        Invoke(methodName, 0.0f);
    }

    private void snapShotViewToGestureView()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
        userInterfaceViewAnimator.SetTrigger(animationTriggers[1]);

        snapshotControls.GestureInputText = "";
        snapshotControls.GestureInputInteractable = false;
        snapshotControls.GestureSubmitButtonInteractable = false;
        snapshotControls.enabled = false;

        freeMode.startFreeMode();
        freeMode.GestureSign = "No Gesture Detected";
    }

    public void playButtonHighlightSound(Button button)
    {
        if (button.IsInteractable())
            buttonAudioSource.PlayOneShot(buttonSoundClips[1]);
    }
}
