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
  
    private void Start()
    {
        recordingControls = handController.GetComponent<RecordingControls>();
        recordingFilePaths = new List<string>();
        recordingFileNames = new List<string>();

        populateRecordingDropDownList();

        if (recordingFilesDropDown.options.Count == 1)
            loadRecordingFile(recordingFilePaths[0]);

        recordingControls.enabled = false;
    }

    private void Update()
    {
        if (recordingControls.SavedPath != "")
        {
            addRecordingToList(recordingControls.SavedPath);
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

        if (recordingFilesDropDown.options.Count == 1)
            loadRecordingFile(recordingFilePaths[0]);
    }

    public void recordingListDropDown_IndexChanged(int index)
    {
        loadRecordingFile(recordingFilePaths[index]);
    }

    private void populateRecordingDropDownList()
    {
        var filePaths = Directory.GetFiles(Application.persistentDataPath + "/Recordings/");

        foreach (string file in filePaths)
        {
            recordingFileNames.Add(Path.GetFileName(file));
            recordingFilePaths.Add(file);
        }

        recordingFilesDropDown.AddOptions(recordingFileNames);
    }

    private void loadRecordingFile(string recordingFilePath)
    {
        byte[] recordingFileBytes = File.ReadAllBytes(recordingFilePath);
        handController.GetLeapRecorder().Load(recordingFileBytes);
    }
}
