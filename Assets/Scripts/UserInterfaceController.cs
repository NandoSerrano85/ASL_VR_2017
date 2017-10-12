using UnityEngine;
using UnityEngine.UI;
using SFB;
using System.IO;
using UnityEditor;

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
    private HandController handController;

    private RecordingControls recordingControls;

    [SerializeField]
    private RecordingList recordingList;

    public static readonly string [] animationTriggers = {"MenuSlideOutTrigger", "MenuSlideInTrigger",
                                                          "FadeOutTrigger", "FadeInTrigger"};

    private ExtensionFilter[] extensions;

    private void Start()
    {
        recordingControls = handController.GetComponent<RecordingControls>();
        buttonAudioSource = GetComponent<AudioSource>();
        extensions = new ExtensionFilter[] {new ExtensionFilter("Byte Files", "bytes")};
    }

    public void importGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);

        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);

        foreach (string path in paths)
        {
            string file = Path.GetFileName(path);
            FileUtil.CopyFileOrDirectory(path, Application.dataPath + "/Resources/Recordings/" + file);
            recordingList.addRecordingToList(file);
        }
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
        if(handController.GetLeapRecorder().state != RecorderState.Stopped)
        {
            handController.ResetRecording();
            handController.StopRecording();         
        }

        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
        gestureViewAnimator.SetTrigger(animationTriggers[1]);
        recordViewAnimator.SetTrigger(animationTriggers[3]);
        recordingControls.RecordingText = "";
        handController.enableRecordPlayback = false;
    }

    public void playButtonHighlightSound(Button button)
    {
        if (button.IsInteractable())
            buttonAudioSource.PlayOneShot(buttonSoundClips[1]);
    }
}
