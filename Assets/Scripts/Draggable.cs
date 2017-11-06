using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler
{
    private RectTransform m_transform;

    // Use this for initialization
    void Start ()
    {
        m_transform = GetComponent<RectTransform>();
	}

    public void OnDrag(PointerEventData eventData)
    {
        m_transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
    }
}
