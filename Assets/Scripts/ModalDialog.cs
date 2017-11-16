using UnityEngine;

public class ModalDialog : MonoBehaviour
{
    private ModalPanel modalPanel;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private string title;

    [SerializeField]
    private string question;

    [SerializeField]
    private bool iconActive;

    [SerializeField]
    private string messageType;

    private AudioSource buttonAudioSource;

    [SerializeField]
    private AudioClip buttonClickSound;

    void Start ()
    {
        buttonAudioSource = GetComponent<AudioSource>();
        modalPanel = ModalPanel.Instance();
	}

    public void showErrorDialog()
    {
        modalPanel.MessageBox(icon, title, question, null, null, null, OkEvent, iconActive, messageType);
    }

    public void OkEvent()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);
    }
}
