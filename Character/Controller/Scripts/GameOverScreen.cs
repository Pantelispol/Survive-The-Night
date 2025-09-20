using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private GameObject hudCanvas; 


    public void Setup(int days,int hours)
    {
        gameObject.SetActive(true);
        //int adjustedHours = hours -6 ;
        pointsText.text = "You survived " + days + " days and " + hours + " hours";

        if (hudCanvas != null)
        {
            hudCanvas.SetActive(false); // Hide HUD
        }
        CursorManager.UnlockAndShow();
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
