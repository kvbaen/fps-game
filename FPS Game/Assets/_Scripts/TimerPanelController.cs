using TMPro;
using UnityEngine;

public class TimerPanelController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timerText;
    private float currentTime = 0;

    void Update()
    {
        currentTime += Time.deltaTime;
        timerText.text = currentTime.ToString("0.00");
    }
}
