using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RecordingList : MonoBehaviour
{
    private Dropdown recordingFilesDropDown;

    [SerializeField]
    private HandController handController;

    [SerializeField]
    private GestureRecording recordingControls;

    private List<string> recordingFilePaths;
    private List<string> recordingFileNames;
  
    private void Start()
    {
        recordingFilesDropDown = GetComponent<Dropdown>();
        recordingFilePaths = new List<string>();
        recordingFileNames = new List<string>();
    }


    private void Update()
    {
        if (recordingControls.SavedPath != "")
        {
            addRecordingToList(recordingControls.CurrentRecordingFilePath);
            recordingControls.SavedPath = "";
        }
    }

    public void addRecordingToList(string recordingFile)
    {
        recordingFileNames.Add(Path.GetFileName(recordingFile));
        recordingFilePaths.Add(recordingFile);

        recordingFilesDropDown.ClearOptions();
        recordingFilesDropDown.AddOptions(recordingFileNames);
        recordingFilesDropDown.RefreshShownValue();
    }

    public void recordingListDropDown_IndexChanged(int index)
    {
        string recordingFilePath = recordingFilePaths[index];

        if (!File.Exists(recordingFilePath))
        {
            refreshDropDownList(recordingFilePath);
            return;
        }

        recordingControls.CurrentRecordingFilePath = recordingFilePath;
        recordingControls.FileLoaded = false;
    }

    public void populateRecordingDropDownList()
    {
        if (!Directory.Exists(recordingControls.RecordingDirectory))
            Directory.CreateDirectory(recordingControls.RecordingDirectory);

        var filePaths = Directory.GetFiles(recordingControls.RecordingDirectory);

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

        if (recordingFilesDropDown.options.Count > 0)
            recordingControls.CurrentRecordingFilePath = recordingFilePaths[0];
    }

    private void refreshDropDownList(string recordingFilePath)
    {
        recordingFileNames.Remove(Path.GetFileName(recordingFilePath));
        recordingFilePaths.Remove(recordingFilePath);

        recordingFilesDropDown.ClearOptions();
        recordingFilesDropDown.AddOptions(recordingFileNames);
        recordingFilesDropDown.RefreshShownValue();

        recordingControls.CurrentRecordingFilePath = recordingFilePaths[recordingFilesDropDown.value];
    }
}
