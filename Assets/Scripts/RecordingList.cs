using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RecordingList : MonoBehaviour
{
    [SerializeField]
    private Dropdown recordingFilesDropDown;

    [SerializeField]
    private HandController handController;

    private RecordingControls recordingControls;

    private List<string> recordingList;

    private void Start()
    {
        recordingControls = handController.GetComponent<RecordingControls>();
        recordingList = new List<string>();
        populateRecordingDropDownList();
    }

    private void Update()
    {
        if (recordingControls.SavedPath != "")
        {
            string file = Path.GetFileName(recordingControls.SavedPath);
            addRecordingToList(Path.GetFileName(file));
            recordingControls.SavedPath = "";
        }
    }

    public void addRecordingToList(string recordingFile)
    {
        recordingList.Add(recordingFile);
        recordingFilesDropDown.ClearOptions();
        recordingFilesDropDown.AddOptions(recordingList);
        recordingFilesDropDown.RefreshShownValue();
        string relativePath = "Assets/Resources/Recordings/" + recordingFile;
        AssetDatabase.ImportAsset(relativePath);
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
        var filePaths = Directory.GetFiles(Application.dataPath + "/Resources/Recordings/").Where(name => !name.EndsWith(".meta"));

        foreach (string file in filePaths)
        {
            recordingList.Add(Path.GetFileName(file));
        }

        recordingFilesDropDown.AddOptions(recordingList);
    }
}
