using TMPro;
using UnityEngine;

public class TimerPanelController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timerText;
    private bool stopTimer = true;
    private float currentTime = 0;

    void OnEnable()
    {
        StartingZoneController.ExitingFromStartingZone += StartTimer;
    }

    void Update()
    {
        if(!stopTimer && Time.timeScale == 1)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void StartTimer()
    {
        stopTimer = false;
    }
    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        int milliseconds = Mathf.Clamp(Mathf.FloorToInt((currentTime * 100) % 100), 0, 99);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

}
