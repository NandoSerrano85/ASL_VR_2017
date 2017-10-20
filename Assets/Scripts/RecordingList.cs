using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RecordingList : MonoBehaviour
{
    [SerializeField]
    private Dropdown recordingFilesDropDown;

    [SerializeField]
    private HandController handController;

    private RecordingControls recordingControls;

    private List<string> recordingFilePaths;
    private List<string> recordingFileNames;

    private static string recordingDirectory;
  
    private void Start()
    {
        recordingControls = handController.GetComponent<RecordingControls>();
        recordingDirectory = Application.persistentDataPath + "/Recordings/";
        recordingFilePaths = new List<string>();
        recordingFileNames = new List<string>();
        recordingControls.enabled = false;
    }

    private void Update()
    {
        if (recordingControls.SavedPath != "")
        {
            recordingControls.saveRecordingFile(recordingFileNames);
            recordingControls.RecordingText = "Recording saved to:\n" + recordingControls.CurrentRecordingFilePath;
            addRecordingToList(recordingControls.CurrentRecordingFilePath);
            recordingControls.SavedPath = "";
        }

        if (recordingFileNames.Count != getFilesInRecordingDirectory().Length)
            populateRecordingDropDownList();
    }

    public void addRecordingToList(string recordingFile)
    {
        recordingFileNames.Add(Path.GetFileName(recordingFile));
        recordingFilePaths.Add(recordingFile);

        recordingFilesDropDown.ClearOptions();
        recordingFilesDropDown.AddOptions(recordingFileNames);
        recordingFilesDropDown.RefreshShownValue();

        if (recordingFilesDropDown.options.Count == 1)
            recordingControls.CurrentRecordingFilePath = recordingFilePaths[0];
    }

    public void recordingListDropDown_IndexChanged(int index)
    {
        recordingControls.CurrentRecordingFilePath = recordingFilePaths[index];
        recordingControls.FileLoaded = false;
    }

    public void populateRecordingDropDownList()
    {
        var filePaths = getFilesInRecordingDirectory();

        recordingFileNames.Clear();
        recordingFilePaths.Clear();

        foreach (string file in filePaths)
        {
            recordingFileNames.Add(Path.GetFileName(file));
            recordingFilePaths.Add(file);
        }

        recordingFilesDropDown.ClearOptions();
        recordingFilesDropDown.AddOptions(recordingFileNames);
        recordingFilesDropDown.RefreshShownValue();

        if (recordingFilesDropDown.options.Count == 1)
            recordingControls.CurrentRecordingFilePath = recordingFilePaths[0];
    }

    private string [] getFilesInRecordingDirectory()
    {
        if (!Directory.Exists(recordingDirectory))
            Directory.CreateDirectory(recordingDirectory);

        return Directory.GetFiles(recordingDirectory);
    }
}
