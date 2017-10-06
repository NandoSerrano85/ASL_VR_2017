using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour
{
    private AudioSource buttonAudioSource;
    [SerializeField]
    private AudioClip[] buttonSoundClips;

    [SerializeField]
    private Animator gestureViewAnimator;
    [SerializeField]
    private Animator recordViewAnimator;

    [SerializeField]
    private RecordingTimer recordingTimer;

    public static readonly string [] animationTriggers = {"MenuSlideOutTrigger", "MenuSlideInTrigger"};

    private void Start()
    {
        buttonAudioSource = GetComponent<AudioSource>();
    }

    public void importGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
    }

    public void recordGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
        gestureViewAnimator.SetTrigger(animationTriggers[0]);
        recordViewAnimator.SetTrigger(animationTriggers[1]);
    }

    public void createGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
    }

    public void resetRecordingClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
        recordingTimer.setTimerOn(false);
        recordingTimer.resetRecordingTimerText();
    }

    public void startRecordingClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
        recordingTimer.setTimerOn(true);
    }

    public void pauseRecordingClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
        recordingTimer.setTimerOn(false);
    }

    public void returnToPreviousMenu(string methodName)
    {
        resetRecordingClick();
        Invoke(methodName, 0.0f);
    }

    private void fromRecordToGestureView()
    {
        recordViewAnimator.SetTrigger(animationTriggers[0]);
        gestureViewAnimator.SetTrigger(animationTriggers[1]);
    }

    public void playButtonHighlightSound(Button button)
    {
        if (button.IsInteractable())
            buttonAudioSource.PlayOneShot(buttonSoundClips[1]);
    }
}
