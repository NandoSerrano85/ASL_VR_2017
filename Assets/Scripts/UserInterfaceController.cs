﻿using UnityEngine;
using UnityEngine.UI;
using SFB;

public class UserInterfaceController : MonoBehaviour
{
    private AudioSource buttonAudioSource;

    [SerializeField]
    private AudioClip[] buttonSoundClips;

    [SerializeField]
    private Animator userInterfaceViewAnimator;

    [SerializeField]
    private HandController handController;

    private RecordingControls recordingControls;

    //[SerializeField]
    //private RecordingGesture recordingGesture;

    public static readonly string[] animationTriggers = { "SlideOutAndFadeInTrigger", "SlideInFadeOutTrigger" };

    //private ExtensionFilter[] extensions;

    private void Start()
    {
        recordingControls = handController.GetComponent<RecordingControls>();
        buttonAudioSource = GetComponent<AudioSource>();
        //extensions = new ExtensionFilter[] {new ExtensionFilter("Byte Files", "bytes")};
    }

    public void importGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);

        if (!handController.IsConnected())
            return;

        //var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);

        //foreach (string path in paths)
        //{
        //    recordingGesture.addRecordingToList(path);
        //}
    }

    public void recordGestureClick()
    {
        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);

        if (!handController.IsConnected())
            return;

        //recordingGesture.populateRecordingDropDownList();
        userInterfaceViewAnimator.SetTrigger(animationTriggers[0]);
        recordingControls.enabled = true;
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

    private void gestureViewToRecordView()
    {
        //if(handController.GetLeapRecorder().state != RecorderState.Stopped)
        //{
        //    handController.ResetRecording();
        //    handController.StopRecording();         
        //}

        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
        userInterfaceViewAnimator.SetTrigger(animationTriggers[1]);
        recordingControls.GestureInputText = "";
        recordingControls.enabled = false;
    }

    public void playButtonHighlightSound(Button button)
    {
        if (button.IsInteractable())
            buttonAudioSource.PlayOneShot(buttonSoundClips[1]);
    }
}
