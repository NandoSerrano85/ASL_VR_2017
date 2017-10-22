using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HandController))]
public class RecordingControls : MonoBehaviour
{
    [Multiline]
    public string header;
    public Text controlsText;
    [SerializeField]
    private Text recordingText;

    public KeyCode beginRecordingKey = KeyCode.R;
    public KeyCode endRecordingKey = KeyCode.R;
    public KeyCode beginPlaybackKey = KeyCode.P;
    public KeyCode pausePlaybackKey = KeyCode.P;
    public KeyCode stopPlaybackKey = KeyCode.S;

    private HandController _controller;

    public string SavedPath { get; set; }
    public string RecordingText { get { return recordingText.text; } set { recordingText.text = value; } }
    public string CurrentRecordingFilePath { get; set; }
    public bool FileLoaded { get; set; }

    private byte[] recordingFileBytes;

    void Start()
    {
        SavedPath = "";
        _controller = GetComponent<HandController>();
        FileLoaded = false;
    }

    void Update()
    {
        if (controlsText != null) controlsText.text = header + "\n";

        switch (_controller.GetLeapRecorder().state)
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
    }

    private void allowBeginRecording()
    {
        if (controlsText != null) controlsText.text += beginRecordingKey + " - Begin Recording\n";

        if (Input.GetKeyDown(beginRecordingKey))
        {
            _controller.ResetRecording();
            _controller.Record();
            RecordingText = "";
        }
    }

    private void allowBeginPlayback()
    {
        if (controlsText != null) controlsText.text += beginPlaybackKey + " - Begin Playback\n";

        if (Input.GetKeyDown(beginPlaybackKey))
        {
            if(loadRecordingFile())
                _controller.PlayRecording();
        }
    }

    private void allowEndRecording()
    {
        if (controlsText != null) controlsText.text += endRecordingKey + " - End Recording\n";

        if (Input.GetKeyDown(endRecordingKey))
        {
            SavedPath = _controller.FinishAndSaveRecording();
        }
    }

    private void allowPausePlayback()
    {
        if (controlsText != null) controlsText.text += pausePlaybackKey + " - Pause Playback\n";

        if (Input.GetKeyDown(pausePlaybackKey))
        {
            _controller.PauseRecording();
        }
    }

    private void allowStopPlayback()
    {
        if (controlsText != null) controlsText.text += stopPlaybackKey + " - Stop Playback\n";

        if (Input.GetKeyDown(stopPlaybackKey))
        {
            _controller.StopRecording();
        }
    }

    private bool loadRecordingFile()
    {
        if (!File.Exists(CurrentRecordingFilePath))
            return false;

        if (!FileLoaded)
        {
            recordingFileBytes = File.ReadAllBytes(CurrentRecordingFilePath);
            FileLoaded = true;
        }

        _controller.GetLeapRecorder().Load(recordingFileBytes);

        return true;
    }

    private string getUniqueRecordingFileName(List<string> recordingFilePaths)
    {
        int recordCounter = 1;
        string newRecordingFilePath = SavedPath + recordCounter + ".bytes";

        for (int i = 0; i < recordingFilePaths.Count; i++)
        {
            if (!recordingFilePaths.Contains(newRecordingFilePath))
                break;

            recordCounter++;
            newRecordingFilePath = SavedPath + recordCounter + ".bytes";
        }

        return newRecordingFilePath;
    }

    public void saveRecordingFile(List<string> recordingFilePaths)
    {
        CurrentRecordingFilePath = getUniqueRecordingFileName(recordingFilePaths);
        _controller.GetLeapRecorder().SaveToNewFile(CurrentRecordingFilePath);
    }
}
