using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadPrefs : MonoBehaviour
{
    [Header("General Setting")]
    [SerializeField] private bool canUse = false;
    [SerializeField] private MenuController menuController;

    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;

    [Header("Brightness Setting")]
    [SerializeField] private TMP_InputField brightnessInputValue = null;
    [SerializeField] private Slider brightnessSlider = null;

    [Header("Quality Level Setting")]
    [SerializeField] private TMP_Dropdown qualityDropdown = null;

    [Header("Fullscreen Setting")]
    [SerializeField] private Toggle fullScreenToggle = null;

    [Header("Sensitivity Setting")]
    [SerializeField] private TMP_InputField sensitivityInputValue = null;
    [SerializeField] private Slider sensitivitySlider = null;

    private void Awake()
    {
        if (canUse)
        {
            if (PlayerPrefs.HasKey("masterVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("masterVolume");

                volumeTextValue.text = localVolume.ToString("0.0");
                volumeSlider.value = localVolume;
                AudioListener.volume = localVolume;
            }
            else
            {
                menuController.ResetButton("Audio");
            }

            if (PlayerPrefs.HasKey("masterQuality"))
            {
                int localQuality = PlayerPrefs.GetInt("masterQuality");

                qualityDropdown.value = localQuality;
                QualitySettings.SetQualityLevel(localQuality);
            }

            if (PlayerPrefs.HasKey("crosshairIndex"))
            {
                int localCrosshair = PlayerPrefs.GetInt("crosshairIndex");

                menuController.loadedCrosshairIndex = localCrosshair;
            }

            if (PlayerPrefs.HasKey("masterBrightness"))
            {
                float localBrightness = PlayerPrefs.GetFloat("masterBrightness");

                brightnessInputValue.text = localBrightness.ToString("0.0");
                brightnessSlider.value = localBrightness;
            }

            if (PlayerPrefs.HasKey("masterFullScreen"))
            {
                int localFullscreen = PlayerPrefs.GetInt("masterFullScreen");

                if (localFullscreen == 1)
                {
                    Screen.fullScreen = true;
                    fullScreenToggle.isOn = true;
                }
                else
                {
                    Screen.fullScreen = false;
                    fullScreenToggle.isOn = false;
                }
            }

            if (PlayerPrefs.HasKey("masterSensitivity"))
            {
                float localSensitivity = PlayerPrefs.GetFloat("masterSensitivity");

                sensitivityInputValue.text = localSensitivity.ToString("0.0");
                sensitivitySlider.value = localSensitivity;
            }

            if (PlayerPrefs.HasKey("masterResolution"))
            {
                int localResolutionIndex = PlayerPrefs.GetInt("masterResolution");
                Resolution[] resolutions = Screen.resolutions;
                Screen.SetResolution(
                    resolutions[localResolutionIndex].width,
                    resolutions[localResolutionIndex].height,
                    fullScreenToggle.isOn ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed,
                    resolutions[localResolutionIndex].refreshRateRatio
                    );
                menuController.loadedResolutionIndex = localResolutionIndex;
            }
        }
    }
}
