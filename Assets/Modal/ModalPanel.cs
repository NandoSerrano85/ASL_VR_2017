using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ModalPanel : MonoBehaviour
{
    public Text Title;     //The Modal Window Title
    public Text Question;  //The Modal Window Question (or statement)
    public Button Button1;   //The first button
    public Button Button2;   //The second button
    public Button Button3;   //The third button
    public Image IconImage; //The Icon Image, if any

    public GameObject ModalPanelObject;       //Reference to the Panel Object
    private static ModalPanel MainModalPanel; //Reference to the Modal Panel, to make sure it's been included

    public static ModalPanel Instance()
    {
        if (!MainModalPanel)
        {
            MainModalPanel = FindObjectOfType(typeof(ModalPanel)) as ModalPanel;

            if (!MainModalPanel)
            {
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
            }
        }
        return MainModalPanel;
    }

    public void MessageBox(Sprite IconPic, string Title, string Question, UnityAction YesEvent, UnityAction NoEvent, UnityAction CancelEvent, UnityAction OkEvent,  bool IconActive, string MessageType)
    {
        ModalPanelObject.SetActive(true);  //Activate the Panel; its default is "off" in the Inspector

        if (MessageType == "YesNoCancel")  //If the user has asked for the Message Box type "YesNoCancel"
        {
            //Button1 is on the far left; Button2 is in the center and Button3 is on the right.  Each can be activated and labeled individually.
            Button1.onClick.RemoveAllListeners();
            Button1.onClick.AddListener(YesEvent);
            Button1.onClick.AddListener(ClosePanel);
            Button1.GetComponentInChildren<Text>().text = "Yes";
            Button2.onClick.RemoveAllListeners();
            Button2.onClick.AddListener(NoEvent);
            Button2.onClick.AddListener(ClosePanel);
            Button2.GetComponentInChildren<Text>().text = "No";
            Button3.onClick.RemoveAllListeners();
            Button3.onClick.AddListener(CancelEvent);
            Button3.onClick.AddListener(ClosePanel);
            Button3.GetComponentInChildren<Text>().text = "Cancel";
            Button1.gameObject.SetActive(true); //We always turn on ONLY the buttons we need, and leave the rest off.
            Button2.gameObject.SetActive(true);
            Button3.gameObject.SetActive(true);
        }

        if (MessageType == "YesNo")        //If the user has asked for the Message Box type "YesNo"
        {
            Button1.onClick.RemoveAllListeners();
            Button2.onClick.RemoveAllListeners();
            Button2.onClick.AddListener(ClosePanel);
            Button2.GetComponentInChildren<Text>().text = "Yes";
            Button3.onClick.RemoveAllListeners();
            Button3.onClick.AddListener(NoEvent);
            Button3.onClick.AddListener(ClosePanel);
            Button3.GetComponentInChildren<Text>().text = "No";
            Button1.gameObject.SetActive(false);
            Button2.gameObject.SetActive(true);
            Button3.gameObject.SetActive(true);
        }

        if (MessageType == "Ok")           //If the user has asked for the Message Box type "Ok"
        {
            Button1.onClick.RemoveAllListeners();
            Button2.onClick.RemoveAllListeners();
            Button2.onClick.AddListener(ClosePanel);
            Button2.onClick.AddListener(OkEvent);
            Button2.GetComponentInChildren<Text>().text = "Ok";
            Button3.onClick.RemoveAllListeners();
            Button1.gameObject.SetActive(false);
            Button2.gameObject.SetActive(true);
            Button3.gameObject.SetActive(false);
        }

        this.Title.text = Title;           //Fill in the Title part of the Message Box
        this.Question.text = Question;     //Fill in the Question/statement part of the Messsage Box

        if (IconActive)                    //If the Icon is active (true)...
        {
            this.IconImage.gameObject.SetActive(true);  //Turn on the icon,
            this.IconImage.sprite = IconPic;            //and assign the picture.
        }
        else
        {
            this.IconImage.gameObject.SetActive(false); //Turn off the icon.
        }
    }

    void ClosePanel()
    {
        ModalPanelObject.SetActive(false); //Close the Modal Dialog
    }
}
