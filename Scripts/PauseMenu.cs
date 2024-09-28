using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject menu;
    [SerializeField] public string levelToGo;
    [SerializeField] private GameObject panelOptions;
    [SerializeField] public GameObject crosshair;
    [SerializeField] public float volume;
    public FirstPersonController player;
    public static bool isPaused;
    public static float cursorSensitivity = 2.0f;
    public Animator animator;
    public bool subtitleToggle = true;

    private Bus masterBus;
    private void Awake()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        volume = 1f;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            if(isPaused)
            {
                ResumeGame();
            }else
            {
                PauseGame();
            }
        }
    }
    
    public void PauseGame()
    {
        PauseAllFMODEvents();
        pauseMenu.SetActive(true);
        crosshair.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        ResumeAllFMODEvents();
        crosshair.SetActive(true);
        panelOptions.GetComponent<Animator>().SetTrigger("GoDown");
        menu.SetActive(true);
        pauseMenu.SetActive(false);

        Time.timeScale = 1.0f;
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        isPaused = false;
        ResumeAllFMODEvents();
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene(levelToGo);
    }

    public void OpenOptions()
    {
        panelOptions.GetComponent<Animator>().SetTrigger("GoUp");
        menu.SetActive(false);
    }

    public void CloseOptions()
    {
        panelOptions.GetComponent<Animator>().SetTrigger("GoDown");
        menu.SetActive(true);
    }

    public void Sens(Slider slider)
    {
        player.RotationSpeed = slider.value;
    }

    public void Volume(Slider slider)
    {
        volume = slider.value;
    }

    public void SubtitleToggle()
    {
        subtitleToggle = !subtitleToggle;
    }

    public void FullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    void PauseAllFMODEvents()
    {
        FMOD.Studio.Bus masterBus;
        FMODUnity.RuntimeManager.StudioSystem.getBus("bus:/", out masterBus);
        masterBus.setPaused(true);
    }

    void ResumeAllFMODEvents()
    {
        FMOD.Studio.Bus masterBus;
        FMODUnity.RuntimeManager.StudioSystem.getBus("bus:/", out masterBus);
        masterBus.setPaused(false);
    }
}
