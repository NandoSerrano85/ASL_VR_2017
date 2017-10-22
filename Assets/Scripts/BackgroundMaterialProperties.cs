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
        if (handController.GetLeapRecorder().state == RecorderState.Playing && 
            backgroundMaterial.GetFloat("_ColorSpaceGamma") != 0.0f)
        {
            backgroundMaterial.SetFloat("_ColorSpaceGamma", 0.0f);
        }
        else if(handController.GetLeapRecorder().state == RecorderState.Stopped &&
                backgroundMaterial.GetFloat("_ColorSpaceGamma") != 1.9f)
        {
            backgroundMaterial.SetFloat("_ColorSpaceGamma", 1.9f);
        }
    }
}
