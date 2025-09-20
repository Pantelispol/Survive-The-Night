using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused = false;
    public GameObject PauseMenuCanvas;
    private InventoryManager inventoryManager;
    private PlayerHealthSystem playerHealth;
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        LoadVolume();
        Time.timeScale = 1f;
        UpdateCursorState(); 
        inventoryManager = Object.FindAnyObjectByType<InventoryManager>();
        playerHealth = Object.FindAnyObjectByType<PlayerHealthSystem>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && (playerHealth.GetCurrentHealth() > 0))
        {
            if (Paused)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
    }

    void Stop()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
        UpdateCursorState(); 
        if (inventoryManager != null)
        {
            inventoryManager.ToggleInventory(false);
        }
        StartCoroutine(DelayedCursorLock());
    }

    private System.Collections.IEnumerator DelayedCursorLock()
    {
        yield return null;
        UpdateCursorState(); 
    }

    public void Play()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
        UpdateCursorState(); 
    }

    public void MainMenuButton()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
        Paused = false;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
    }

    void UpdateCursorState()
    {
        if (Paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void UpdateSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }
}
