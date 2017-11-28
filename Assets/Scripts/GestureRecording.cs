using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleableObject))]
public class GestureRecording : MonoBehaviour
{
    [Multiline]
    [SerializeField]
    private string header;

    [SerializeField] private Text controlsText;
    [SerializeField] private Text recordingSavePathText;

    [SerializeField] private KeyCode beginRecordingKey = KeyCode.R;
    [SerializeField] private KeyCode endRecordingKey = KeyCode.R;
    [SerializeField] private KeyCode beginPlaybackKey = KeyCode.P;
    [SerializeField] private KeyCode pausePlaybackKey = KeyCode.P;
    [SerializeField] private KeyCode stopPlaybackKey = KeyCode.S;

    [SerializeField]
    private HandController handController;

    private ToggleableObject toggleableObject;

    [SerializeField]
    private InputField recordingFileInputField;

    private Text recordingFileInputFieldPlaceHolderText;

    [SerializeField]
    private Button submitRecordingButton;

    [SerializeField]
    private GameObject background;

    private Material backgroundMaterial;

    public string RecordingSavedPathText { get { return recordingSavePathText.text; } set { recordingSavePathText.text = value; } }
    public string CurrentRecordingFilePath { get; set; }
    public string SavedPath { get; set; }
    public bool FileLoaded { get; set; }
    public bool RecordingInputInteractable { get { return recordingFileInputField.interactable; } set { recordingFileInputField.interactable = value; } }
    public bool RecordingSubmitButtonInteractable { get { return submitRecordingButton.interactable; } set { submitRecordingButton.interactable = value; } }
    public string RecordingFileInputText { get { return recordingFileInputField.text; } set { recordingFileInputField.text = value; } }
    public string RecordingDirectory { get; set; }

    private bool lastActiveViewState;

    [SerializeField]
    private ModalDialog errorModalDialog;

    private AudioSource buttonAudioSource;

    [SerializeField]
    private AudioClip buttonClickSound;

    private void Start()
    {
        FileLoaded = false;
        SavedPath = "";
        RecordingDirectory = Application.persistentDataPath + "/Recordings/";
        errorModalDialog = GetComponent<ModalDialog>();
        recordingFileInputFieldPlaceHolderText = recordingFileInputField.placeholder.GetComponent<Text>();
        toggleableObject = GetComponent<ToggleableObject>();
        backgroundMaterial = background.GetComponent<Renderer>().material;
        buttonAudioSource = GetComponent<AudioSource>();
        lastActiveViewState = true;
    }

    private void Update()
    {
        if (controlsText != null) controlsText.text = header + "\n\n" + toggleableObject.toggleKey + " - Toggle view\n";

        switch (handController.GetLeapRecorder().state)
        {
            case RecorderState.Recording:
                allowEndRecording();
                break;
            case RecorderState.Playing:
                allowPausePlayback();
                allowStopPlayback();
                break;
            case RecorderState.Paused:
                allowBeginPlayback();
                allowStopPlayback();
                break;
            case RecorderState.Stopped:
                allowBeginRecording();
                allowBeginPlayback();
                break;
        }


        if (Input.GetKeyDown(toggleableObject.toggleKey) && !recordingFileInputField.isFocused)
        {
            lastActiveViewState = !lastActiveViewState;
            toggleableObject.toggleObject(lastActiveViewState);
        }

        if (recordingFileInputField.isFocused)
            recordingFileInputFieldPlaceHolderText.text = "";
        else
            recordingFileInputFieldPlaceHolderText.text = "Enter Recording Name...";

        if (handController.GetLeapRecorder().state == RecorderState.Playing &&
            handController.GetRecordingProgress() == 1.0f)
        {
            lastActiveViewState = true;
            toggleableObject.toggleObject(lastActiveViewState);
            handController.StopRecording();
            backgroundMaterial.SetFloat("_ColorSpaceGamma", 1.99f);
        }
    }

    private void allowBeginRecording()
    {
        if (controlsText != null) controlsText.text += beginRecordingKey + " - Begin Recording\n";

        if (Input.GetKeyDown(beginRecordingKey) && !RecordingInputInteractable)
        {
            lastActiveViewState = false;
            toggleableObject.toggleObject(lastActiveViewState);

            handController.ResetRecording();
            handController.Record();

            RecordingSavedPathText = "";
        }
    }

    private void allowBeginPlayback()
    {
        if (controlsText != null) controlsText.text += beginPlaybackKey + " - Begin Playback\n";

        if (Input.GetKeyDown(beginPlaybackKey) && !RecordingInputInteractable)
        {
            if (loadRecordingFile())
            {
                backgroundMaterial.SetFloat("_ColorSpaceGamma", 0.0f);
                lastActiveViewState = false;
                toggleableObject.toggleObject(lastActiveViewState);

                handController.PlayRecording();
            }
        }
    }

    private void allowEndRecording()
    {
        if (controlsText != null) controlsText.text += endRecordingKey + " - End Recording\n";

        if (Input.GetKeyDown(endRecordingKey))
        {
            lastActiveViewState = true;
            toggleableObject.toggleObject(lastActiveViewState);
            handController.StopRecording();
            RecordingInputInteractable = true;
            RecordingSubmitButtonInteractable = true;
        }
    }

    private void allowPausePlayback()
    {
        if (controlsText != null) controlsText.text += pausePlaybackKey + " - Pause Playback\n";

        if (Input.GetKeyDown(pausePlaybackKey))
        {
            handController.PauseRecording();
        }
    }

    private void allowStopPlayback()
    {
        if (controlsText != null) controlsText.text += stopPlaybackKey + " - Stop Playback\n";

        if (Input.GetKeyDown(stopPlaybackKey))
        {
            backgroundMaterial.SetFloat("_ColorSpaceGamma", 1.99f);
            lastActiveViewState = true;
            toggleableObject.toggleObject(lastActiveViewState);

            handController.StopRecording();
        }
    }

    public void saveRecordingFile()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);

        string recordingFileName = RecordingFileInputText.Trim();

        if (!string.IsNullOrEmpty(recordingFileName))
        {
            SavedPath = RecordingDirectory + recordingFileName + ".bytes";
            CurrentRecordingFilePath = SavedPath;
            handController.GetLeapRecorder().SaveToNewFile(CurrentRecordingFilePath);
            string condensedPath = shortenPath(CurrentRecordingFilePath);
            RecordingSavedPathText = "Recording saved to: " + condensedPath;

            RecordingInputInteractable = false;
            RecordingSubmitButtonInteractable = false;

            StartCoroutine(resetText());
        }
        else
            errorModalDialog.showErrorDialog(OkEvent);

        RecordingFileInputText = "";
    }

    public string shortenPath(string absolutePath, int limit = 55, string delimiter = "...")
    {
        if (string.IsNullOrEmpty(absolutePath))
        {
            return "";
        }

        var name = Path.GetFileName(absolutePath);
        int nameLen = name.Length;
        int pathLen = absolutePath.Length;
        var dir = absolutePath.Substring(0, pathLen - nameLen);

        int delimlen = delimiter.Length;
        int idealMinLen = nameLen + delimlen;

        var slash = (absolutePath.IndexOf("/") > -1 ? "/" : "\\");

        if (limit < ((2 * delimlen) + 1))
        {
            return "";
        }

        if (limit >= pathLen)
        {
            return absolutePath;
        }

        if (limit < idealMinLen)
        {
            return delimiter + name.Substring(0, (limit - (2 * delimlen))) + delimiter;
        }

        if (limit == idealMinLen)
        {
            return delimiter + name;
        }

        return dir.Substring(0, (limit - (idealMinLen + 1))) + delimiter + slash + name;
    }

    private bool loadRecordingFile()
    {
        if (!File.Exists(CurrentRecordingFilePath))
            return false;

        if (!FileLoaded)
        {
            byte [] recordingFileBytes = File.ReadAllBytes(CurrentRecordingFilePath);
            FileLoaded = true;
            handController.GetLeapRecorder().Load(recordingFileBytes);
        }

        return true;
    }

    private IEnumerator resetText()
    {
        yield return new WaitForSeconds(4);
        RecordingSavedPathText = "";
    }

    public void OkEvent()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);
    }
}
