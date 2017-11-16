using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
    private RectTransform rectComponent;

    [SerializeField]
    private float rotatetSpeed = 300.0f;

	// Use this for initialization
	private void Start ()
    {
        rectComponent = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	private void Update ()
    {
        rectComponent.Rotate(0f, 0f, rotatetSpeed * Time.deltaTime);
	}

    private void OnDisable()
    {
        rectComponent.localRotation = Quaternion.identity;
    }
}
