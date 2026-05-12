using UnityEngine;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GameUXManager : MonoBehaviour
{
    [Header("References")]
    public LoopManager loopManager;
    public PlayerMovement playerMovement;
    public MobileInputBridge mobileInput;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject howToPlayPanel;
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Settings UI")]
    public Slider sensitivitySlider;
    public Text sensitivityValueText;
    public Toggle soundToggle;
    public Toggle mobileControlsToggle;

    [Header("Runtime")]
    public bool gameStarted = false;
    public bool paused = false;

    private float defaultSensitivity = 2f;

    private void Start()
    {
        Time.timeScale = 1f;

        if (playerMovement != null)
        {
            defaultSensitivity = playerMovement.mouseSensitivity;
            playerMovement.SetMovementEnabled(false);
        }

        ShowMainMenu();

        ApplySettingsToUI();
    }

    private void Update()
    {
        if (PausePressed() && gameStarted)
        {
            if (paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        paused = false;

        HideAllPanels();

        Time.timeScale = 1f;

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true);
        }

        if (mobileInput != null)
        {
            mobileInput.SetMobileControlsVisible(mobileControlsToggle == null || mobileControlsToggle.isOn);
        }
    }

    public void ShowMainMenu()
    {
        gameStarted = false;
        paused = false;

        Time.timeScale = 1f;

        HideAllPanels();

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
        }

        if (mobileInput != null)
        {
            mobileInput.SetMobileControlsVisible(false);
        }
    }

    public void ShowHowToPlay()
    {
        HideAllPanels();

        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    public void ShowSettings()
    {
        HideAllPanels();

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        ApplySettingsToUI();
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    public void PauseGame()
    {
        if (!gameStarted)
        {
            return;
        }

        paused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
        }

        if (mobileInput != null)
        {
            mobileInput.SetMobileControlsVisible(false);
        }
    }

    public void ResumeGame()
    {
        paused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true);
        }

        if (mobileInput != null)
        {
            mobileInput.SetMobileControlsVisible(mobileControlsToggle == null || mobileControlsToggle.isOn);
        }
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;
        paused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (loopManager != null)
        {
            loopManager.RestartGame();
        }

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true);
        }

        if (mobileInput != null)
        {
            mobileInput.SetMobileControlsVisible(mobileControlsToggle == null || mobileControlsToggle.isOn);
        }
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        paused = false;

        if (loopManager != null)
        {
            loopManager.RestartGame();
        }

        ShowMainMenu();
    }

    public void OnSensitivityChanged(float value)
    {
        if (playerMovement != null)
        {
            playerMovement.mouseSensitivity = value;
        }

        if (sensitivityValueText != null)
        {
            sensitivityValueText.text = value.ToString("0.0");
        }
    }

    public void OnSoundToggleChanged(bool enabled)
    {
        AudioListener.volume = enabled ? 1f : 0f;
    }

    public void OnMobileControlsToggleChanged(bool enabled)
    {
        if (mobileInput != null)
        {
            mobileInput.SetMobileControlsVisible(gameStarted && !paused && enabled);
        }
    }

    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    private void ApplySettingsToUI()
    {
        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0.5f;
            sensitivitySlider.maxValue = 4.0f;
            sensitivitySlider.value = playerMovement != null ? playerMovement.mouseSensitivity : defaultSensitivity;
        }

        if (sensitivityValueText != null)
        {
            sensitivityValueText.text = (playerMovement != null ? playerMovement.mouseSensitivity : defaultSensitivity).ToString("0.0");
        }

        if (soundToggle != null)
        {
            soundToggle.isOn = AudioListener.volume > 0.01f;
        }

        if (mobileControlsToggle != null)
        {
            mobileControlsToggle.isOn = true;
        }
    }

    private bool PausePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }
}