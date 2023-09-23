using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanelController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text healtText;
    [SerializeField]
    private Image healtImage;
    private PlayerController playerController;
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        healtText.text = playerController.health.ToString();
    }

    void Update()
    {
        healtText.text = playerController.health.ToString();
        if (playerController.health <= 30)
        {
            healtText.color = Color.red;
            healtImage.color = Color.red;
        }
    }
}
