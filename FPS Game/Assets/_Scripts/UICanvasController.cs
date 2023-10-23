using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasController : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;
    private MenuController menuController;
    public Color crosshairColor;
    void Start()
    {
        menuController = FindObjectOfType<MenuController>();
        if (menuController != null && menuController.crosshair)
        {
            crosshairImage.sprite = menuController.crosshair;
            crosshairImage.color = crosshairColor;
        }
    }

    void Update()
    {
        if (menuController != null && menuController.crosshair != crosshairImage.sprite)
        {
            crosshairImage.sprite = menuController.crosshair;
        }
        if (menuController != null && crosshairColor != crosshairImage.color)
        {
            crosshairImage.color = crosshairColor;
        }
    }
}
