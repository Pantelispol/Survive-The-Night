using TMPro;
using UnityEngine;

public class IntroTextManager : MonoBehaviour
{
    public TextMeshProUGUI introText;
    public float displayDuration = 10f;

    void Start()
    {
        if (introText != null)
        {
            introText.gameObject.SetActive(true);
            Invoke(nameof(HideText), displayDuration);
        }
    }

    void HideText()
    {
        introText.gameObject.SetActive(false);
    }
}
