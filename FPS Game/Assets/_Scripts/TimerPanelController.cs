using TMPro;
using UnityEngine;

public class TimerPanelController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timerText;
    private bool stopTimer = true;
    private float currentTime = 0;
    private PlayerController _playerController;
    private MenuController _menuController;
    private bool currentTimeEmitted = false;
    public delegate void SomeAction(TMP_Text currentTime);
    public static event SomeAction CurrentTimeEmitter;

    private void Awake()
    {
        _playerController = GetComponentInParent<PlayerController>();
        _menuController = FindObjectOfType<MenuController>();
    }

    void OnEnable()
    {
        StartingZoneController.ExitingFromStartingZone += StartTimer;
    }

    void Update()
    {
        if (_playerController.health == 0 && !currentTimeEmitted) 
        {
            currentTimeEmitted = true;
            CurrentTimeEmitter.Invoke(timerText);
            return;
        }

        if(!stopTimer && Time.timeScale == 1 && _playerController.health > 0)
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
        int milliseconds = Mathf.Clamp(Mathf.FloorToInt((currentTime * 10) % 10), 0, 9);

        timerText.text = string.Format("{0:00}:{1:00}:{2:0}", minutes, seconds, milliseconds);
    }
}
