using UnityEngine;
using UnityEngine.UI;

public class WebcamDisplay : MonoBehaviour
{
    private RawImage rawImage;
    private WebCamTexture webcamTexture;

    void Start()
    {
        rawImage = GetComponent<RawImage>();

        // Check if there are available cameras
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("No cameras found!");
            return;
        }

        // Use the first available camera (change index if needed)
        webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name);
        rawImage.texture = webcamTexture;
        webcamTexture.Play();
    }

    void OnDestroy()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}