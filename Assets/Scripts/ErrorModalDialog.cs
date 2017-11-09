using UnityEngine;

public class ErrorModalDialog : MonoBehaviour
{
    private ModalPanel modalPanel;

    [SerializeField] private Sprite errorIcon;

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
        modalPanel.MessageBox(errorIcon, "Error", "You can't submit a gesture with no name.", null, null, null, OkEvent, true, "Ok");
    }

    public void OkEvent()
    {
        buttonAudioSource.PlayOneShot(buttonClickSound);
    }
}
