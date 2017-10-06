using UnityEngine;
using UnityEngine.UI;

public class RecordingTimer : MonoBehaviour
{
    [SerializeField]
    private Text recordingTimerText;
    private float recordingTimer;
    private bool timerOn;

	// Use this for initialization
	void Start ()
    {
        recordingTimer = 0.0f;
        timerOn = false;
	}
	
	// Update is called once per fram
	void Update ()
    {
        if (!timerOn)
            return;

        recordingTimer += Time.deltaTime;

        int seconds = (int)(recordingTimer % 60);
        int minutes = (int)(recordingTimer / 60);
        int hours = (int)(recordingTimer / 3600) % 24;

        recordingTimerText.text = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);
	}

    public void setTimerOn(bool timerOn)
    {
        this.timerOn = timerOn;
    }

    public void resetRecordingTimerText()
    {
        recordingTimerText.text = "0:00:00";
        recordingTimer = 0.0f;
    }
}
