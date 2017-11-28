using UnityEngine;
using UnityEngine.Events;

public class ModalDialog : MonoBehaviour
{
    private ModalPanel modalPanel;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private string title;

    [SerializeField]
    private string message;

    [SerializeField]
    private bool iconActive;

    [SerializeField]
    private string messageType;

    void Start ()
    {
        modalPanel = ModalPanel.Instance();
	}

    public void showErrorDialog(UnityAction OkEvent)
    {
        modalPanel.MessageBox(icon, title, message, null, null, null, OkEvent, iconActive, messageType);
    }

    public void showQuestionDialog(UnityAction YesEvent, UnityAction NoEvent)
    {
        modalPanel.MessageBox(icon, title, message, YesEvent, NoEvent, null, null, iconActive, messageType);
    }

    public bool isDialogActive()
    {
        return modalPanel.ModalPanelObject.activeSelf;
    }
}
