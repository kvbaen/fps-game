using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Linq;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;

    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private Camera menuCamera = null;
    [SerializeField] private GameObject pausedGameDialog;
    [SerializeField] public GameObject diedDialog;
    [SerializeField] private GameObject BO_UI;
    [SerializeField] private GameObject endGameDialog;
    [SerializeField] private TMP_Text timerText;
    private bool IsESCPressed => Input.GetKeyDown(KeyCode.Escape);

    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private GameObject confirmationPrompt = null;
    [SerializeField] private float defaultVolume = 0.5f;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_InputField sensitivityInputValue = null;
    [SerializeField] private Slider sensitivitySlider = null;
    [SerializeField] private float defaultSensitivity = 1f;
    [SerializeField] private TMP_Dropdown crosshairDropdown = null;
    public float mainSensitivity = 1f;
    [SerializeField] private string crosshairFolder;
    [SerializeField] private int defaultCrosshair;
    private Sprite[] crosshairSprites = null;
    public Sprite crosshair;
    public int loadedCrosshairIndex = -1;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_InputField brightnessInputValue = null;
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private float defaultBrightness = 1;
    [SerializeField] private Toggle fullScreenToggle = null;
    [SerializeField] private TMP_Dropdown qualityDropdown = null;

    private int _qualityLevel;
    private bool _isFullScreen = true;
    private float _brightnessLevel;
    public bool _isGamePaused;

    [Header("Resolution Dropdowns")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    Resolution resolution;
    public int loadedResolutionIndex = -1;

    [Header("Maps Settings")]
    public string mapsFolder;
    public List<string> mapNames;
    public string selectedMap;
    AsyncOperation loadingOperation;

    private bool IsPlayerAlive = true;
    private Button playButton;
    private Button playAgainButton;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        mapNames = new List<string>();
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " (" + resolutions[i].refreshRateRatio + " Hz)";
            options.Add(option);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        crosshairSprites = Resources.LoadAll<Sprite>(crosshairFolder);
        List<TMP_Dropdown.OptionData> crosshairItems = new List<TMP_Dropdown.OptionData>();
        foreach (var crosshair in crosshairSprites)
        {
            var crosshairItem = new TMP_Dropdown.OptionData(crosshair.name, crosshair);
            crosshairItems.Add(crosshairItem);
        }
        crosshairDropdown.ClearOptions();
        crosshairDropdown.AddOptions(crosshairItems);
        crosshairDropdown.RefreshShownValue();
        if (loadedCrosshairIndex >= 0)
        {
            crosshairDropdown.value = loadedCrosshairIndex;
        }
        if (loadedResolutionIndex >= 0)
        {
            resolutionDropdown.value = loadedResolutionIndex;
        }
        LoadMaps();
        GetReferenceOfButtons();
    }

    private void Update()
    {
        /*if (IsPlayerAlive && endGameStatement)
        {
            if (!wonDialog.activeSelf) EndGame();
            return;
        }*/

        if (IsESCPressed && SceneManager.loadedSceneCount > 1 && IsPlayerAlive)
        {
            if (_isGamePaused)
            {
                ResumeGame();
                _isGamePaused = false;
            }
            else
            {
                PauseGame();
                _isGamePaused = true;
            }
        }

        if (!IsPlayerAlive && !diedDialog.activeSelf)
        {
            Died();
        }

        ControlVisibilityOfButtons();
    }

    void OnEnable()
    {
        PlayerController.PlayerDied += PlayerDied;
        TimerPanelController.CurrentTimeEmitter += SetTimerText;
    }

    public void SetResolution(int resolutionIndex)
    {
        resolution = resolutions[resolutionIndex];
    }

    public void SetCrosshair(int crosshairIndex)
    {
        crosshair = crosshairSprites[crosshairIndex];
    }

    public void EnterMap()
    {
        IsPlayerAlive = true;
        foreach (var mapName in mapNames)
        {
            if (mapName.Equals(selectedMap))
            {
                if (SceneManager.loadedSceneCount > 1)
                {
                    int mapIndex = SceneManager.GetSceneAt(1).buildIndex;
                    loadingOperation = SceneManager.UnloadSceneAsync(mapIndex);
                    loadingOperation.completed += (asyncOperation) =>
                    {
                        loadingOperation = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Additive);
                        loadingOperation.completed += (asyncOperation) =>
                        {
                            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
                        };
                    };
                }
                else if (SceneManager.loadedSceneCount == 1)
                {
                    loadingOperation = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Additive);
                    loadingOperation.completed += (asyncOperation) =>
                    {
                        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
                    };
                }
                menuCanvas.SetActive(false);
                menuCamera.gameObject.SetActive(false);
            }
        }
    }
    private void LoadMaps()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            var sceneName = Path.GetFileNameWithoutExtension(path);
            if (path[..path.LastIndexOf('/')] == mapsFolder)
            {
                mapNames.Add(sceneName);
            }
        }
    }
    public void SwitchToMainMenu()
    {
        Scene scene = SceneManager.GetSceneAt(1);
        if (scene.name != "MainMenu")
        {
            loadingOperation = SceneManager.UnloadSceneAsync(scene);
            loadingOperation.completed += (asyncOperation) =>
            {
                diedDialog.SetActive(false);
                pausedGameDialog.SetActive(false);
                Image backgroundGO = menuCanvas.transform.GetChild(1).GetComponent<Image>();
                backgroundGO.sprite = Resources.Load<Sprite>("MenuBackground");
                backgroundGO.color = new Color(255, 255, 255, 1);
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
            };
        }
        if(!IsPlayerAlive)
        {
            IsPlayerAlive = true;
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void SetSenitivity(float sensitivity)
    {
        mainSensitivity = sensitivity;
        sensitivityInputValue.text = sensitivity.ToString("0.0");
    }

    public void SetSensitivityinput(string sensitivity)
    {
        sensitivitySlider.value = float.Parse(sensitivity);
        SetSenitivity(float.Parse(sensitivity));
    }

    public void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSensitivity", mainSensitivity);
        PlayerPrefs.SetInt("crosshairIndex", crosshairSprites.ToList<Sprite>().IndexOf(crosshair));
        StartCoroutine(ConfirmationBox());
    }

    public void SetBrightnessInput(string brightness)
    {
        brightnessSlider.value = float.Parse(brightness);
        SetBrightness(float.Parse(brightness));
    }
    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessInputValue.text = brightness.ToString("0.0");
    }

    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }
    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);

        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullScreen", _isFullScreen ? 1 : 0);
        Screen.fullScreen = _isFullScreen;

        PlayerPrefs.SetInt("masterResolution", resolutions.ToList<Resolution>().IndexOf(resolution));
        Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.FullScreenWindow, resolution.refreshRateRatio);
        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            PlayerPrefs.SetFloat("masterVolume", defaultVolume);
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }
        if (MenuType == "Gameplay")
        {
            sensitivityInputValue.text = defaultSensitivity.ToString("0.0");
            sensitivitySlider.value = defaultSensitivity;
            mainSensitivity = defaultSensitivity;
            crosshairDropdown.value = defaultCrosshair;
            GameplayApply();
        }
        if (MenuType == "Graphics")
        {
            brightnessSlider.value = defaultBrightness;
            brightnessInputValue.text = defaultBrightness.ToString("0.0");

            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = true;
            Screen.fullScreen = true;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            PlayerPrefs.SetInt("masterResolution", resolutions.Length);
            GraphicsApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }

    public void SetTimerText(TMP_Text currentTime)
    {
        timerText.text = currentTime.text;
    }

    public void PlayerDied()
    {
        IsPlayerAlive = false;
    }

    public void EndGame()
    {
        menuCanvas.SetActive(true);
        endGameDialog.SetActive(true);
        Image backgroundGO = menuCanvas.transform.GetChild(1).GetComponent<Image>();
        backgroundGO.sprite = null;
        backgroundGO.color = new Color(0, 0, 0, 0.6f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Died()
    {
        menuCanvas.SetActive(true);
        diedDialog.SetActive(true);
        Image backgroundGO = menuCanvas.transform.GetChild(1).GetComponent<Image>();
        backgroundGO.sprite = null;
        backgroundGO.color = new Color(0, 0, 0, 0.6f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        menuCanvas.SetActive(true);
        pausedGameDialog.SetActive(true);
        Image backgroundGO = menuCanvas.transform.GetChild(1).GetComponent<Image>();
        backgroundGO.sprite = null;
        backgroundGO.color = new Color(0, 0, 0, 0.6f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        pausedGameDialog.SetActive(false);
        menuCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GetReferenceOfButtons()
    {
        Transform playButtonTransform = BO_UI.transform.Find("Play BTN");
        playButton = playButtonTransform.GetComponent<Button>();
        Transform playAgainButtonTransform = BO_UI.transform.Find("Play Again BTN");
        playAgainButton = playAgainButtonTransform.GetComponent<Button>();
        playAgainButton.onClick.AddListener(HandlePlayAgainButtonClick);
    }

    private void HandlePlayAgainButtonClick()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        diedDialog.SetActive(false);
        pausedGameDialog.SetActive(false);
        menuCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ControlVisibilityOfButtons()
    {
        if (!IsPlayerAlive)
        {
            playButton.gameObject.SetActive(false);
            playAgainButton.gameObject.SetActive(true);
        } else
        {
            playButton.gameObject.SetActive(true);
            playAgainButton.gameObject.SetActive(false);
        }
    }
}
