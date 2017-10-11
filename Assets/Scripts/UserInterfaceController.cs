using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HandController))]
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
    private Text recordingPathFile;

    [SerializeField]
    private Dropdown recordingFilesDropDown;

    [SerializeField]
    private HandController handController;

    private RecordingControls recordingControls;

    private List<string> recordingList;

    public static readonly string [] animationTriggers = {"MenuSlideOutTrigger", "MenuSlideInTrigger",
                                                          "FadeOutTrigger", "FadeInTrigger"};
    private void Start()
    {
        buttonAudioSource = GetComponent<AudioSource>();
        recordingControls = handController.GetComponent<RecordingControls>();
        recordingList =  new List<string>();
        populateRecordingDropDownList();
    }

    private void Update()
    {
        if (recordingControls.SavedPath != "")
        {
            addRecordingToList(Path.GetFileName(recordingControls.SavedPath));
            recordingControls.SavedPath = "";
        }
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
        if(handController.GetLeapRecorder().state != RecorderState.Stopped)
        {
            handController.ResetRecording();
            handController.StopRecording();         
        }

        buttonAudioSource.PlayOneShot(buttonSoundClips[0]);
        gestureViewAnimator.SetTrigger(animationTriggers[1]);
        recordViewAnimator.SetTrigger(animationTriggers[3]);
        recordingPathFile.text = "";
        handController.enableRecordPlayback = false;
    }

    public void playButtonHighlightSound(Button button)
    {
        if (button.IsInteractable())
            buttonAudioSource.PlayOneShot(buttonSoundClips[1]);
    }

    private void addRecordingToList(string recordingFile)
    {
        recordingList.Add(recordingFile);
        recordingFilesDropDown.ClearOptions();
        recordingFilesDropDown.AddOptions(recordingList);
        recordingFilesDropDown.RefreshShownValue();
    }

    public void recordingListDropDown_IndexChanged(int index)
    {
        string file = "Assets/Resources/Recordings/" + recordingList[index];
        handController.recordingAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(file, typeof(TextAsset));
        AssetDatabase.ImportAsset(file);
        handController.enableRecordPlayback = true;
    }

    private void populateRecordingDropDownList()
    {
        var filePaths = Directory.GetFiles(Application.dataPath + "/Resources/Recordings").Where(name => !name.EndsWith(".meta"));

        foreach (string file in filePaths)
        {
            recordingList.Add(Path.GetFileName(file));
        }

        recordingFilesDropDown.AddOptions(recordingList);
    }
}
