using UnityEngine;

public class ToggleableObject : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.T;

    [SerializeField]
    private GameObject objectToToggle;

    public void toggleObject(bool toggleState)
    {
        objectToToggle.SetActive(toggleState);
    }
}
