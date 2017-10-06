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

    public static readonly string [] animationTriggers = {"MenuSlideOutTrigger", "MenuSlideInTrigger",
                                                          "FadeOutTrigger", "FadeInTrigger"};

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
        recordViewAnimator.SetTrigger(animationTriggers[2]);
    }

    public void createGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
    }

    public void returnToPreviousMenu(string methodName)
    {
        Invoke(methodName, 0.0f);
    }

    private void gestureViewToRecordView()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
        gestureViewAnimator.SetTrigger(animationTriggers[1]);
        recordViewAnimator.SetTrigger(animationTriggers[3]);
    }

    public void playButtonHighlightSound(Button button)
    {
        if (button.IsInteractable())
            buttonAudioSource.PlayOneShot(buttonSoundClips[1]);
    }
}
