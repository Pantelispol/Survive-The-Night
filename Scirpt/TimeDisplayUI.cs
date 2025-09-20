using TMPro;
using UnityEngine;

public class TimeDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;

    private void Update()
    {
        if (TimeManager.Instance != null && timeText != null)
        {
            int day = TimeManager.Instance.GetDays();
            int hour = TimeManager.Instance.GetHours();
            timeText.text = $"Day: {day}  |  Hour: {hour:00}:00";
        }
    }
}
