using UnityEngine;

public class BackgroundMaterialProperties : MonoBehaviour
{
    [SerializeField]
    private HandController handController;

    private Material backgroundMaterial;

	// Use this for initialization
	void Start ()
    {
        backgroundMaterial = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (handController.GetLeapRecorder().state == RecorderState.Playing ||
            handController.GetLeapRecorder().state == RecorderState.Paused)
        {
            backgroundMaterial.SetFloat("_ColorSpaceGamma", 0.0f);
            backgroundMaterial.renderQueue = 1000;
        }
        else
        {
            backgroundMaterial.SetFloat("_ColorSpaceGamma", 1.9f);
            backgroundMaterial.renderQueue = 3000;
        }
    }
}
