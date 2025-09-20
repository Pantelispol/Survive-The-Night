using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TimeManager;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;

    [SerializeField] private Gradient graddientNightToSunrise;
    [SerializeField] private Gradient graddientSunriseToDay;
    [SerializeField] private Gradient graddientDayToSunset;
    [SerializeField] private Gradient graddientSunsetToNight;

    [SerializeField] private Light globalLight;

    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private GameObject inventoryCanvas;
    private bool gameEnded = false;
    private bool hasContinued = false;
    private bool keyPressed = false;
    [SerializeField, Tooltip("How many real-time seconds equal one in-game hour.")]
    private float realSecondsPerInGameHour = 5f;
    private float RealSecondsPerInGameMinute => realSecondsPerInGameHour / 60f;

    private int minutes;

    [System.Serializable]
    public enum TimeOfDay
    {
        Night,
        Sunrise,
        Day,
        Sunset
    }

    [SerializeField, ReadOnly]
    private TimeOfDay currentTimeOfDay;
    public TimeOfDay CurrentTimeOfDay => currentTimeOfDay;

    public int Minutes
    { get { return minutes; } set { minutes = value; OnMinutesChange(value); } }

    [SerializeField] private int hours = 0;

    public int Hours
    { get { return hours; } set { hours = value; OnHoursChange(value); } }

    [SerializeField] private int days;

    public int Days
    { get { return days; } set { days = value; } }

    private float tempSecond;

    public static TimeManager Instance { get; private set; }
    public int GetHours()
    {
        return hours;
    }

    public int GetDays()
    {
        return days;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ApplySkyboxForCurrentTime();
    }

    public void Update()
    {
        tempSecond += Time.deltaTime;

        if (tempSecond >= RealSecondsPerInGameMinute)
        {
            Minutes += 1;
            tempSecond = 0;
        }
        if (!gameEnded && !hasContinued && Days == 3 && Hours == 6)
        {
            EndGame();
        }
        //if (!keyPressed && Input.anyKeyDown && gameEnded)
        //{
        //    keyPressed = true;
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //    gameEnded = false;
        //    if (endGameCanvas != null)
        //    {
        //        inventoryCanvas.SetActive(true);
        //        endGameCanvas.SetActive(false);
        //        SceneManager.LoadScene("Main Menu");
        //        //Time.timeScale = 1f;
        //    }
        //    else
        //    {
        //        Debug.LogWarning("End Game Canvas is not assigned in the TimeManager!");
        //    }
        //}
    }

    private void OnMinutesChange(int value)
    {
        globalLight.transform.Rotate(Vector3.up, (1f / (1440f / 4f)) * 360f, Space.World);
        if (value >= 60)
        {
            Hours++;
            minutes = 0;
        }
        if (Hours >= 24)
        {
            Hours = 0;
            Days++;
        }
    }

    private void OnHoursChange(int value)
    {
        if (value == 6)
        {
            StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, 10f));
            StartCoroutine(LerpLight(graddientNightToSunrise, 10f));
            currentTimeOfDay = TimeOfDay.Sunrise;
        }
        else if (value == 8)
        {
            StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, 10f));
            StartCoroutine(LerpLight(graddientSunriseToDay, 10f));
            currentTimeOfDay = TimeOfDay.Day;
        }
        else if (value == 18)
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, 10f));
            StartCoroutine(LerpLight(graddientDayToSunset, 10f));
            currentTimeOfDay = TimeOfDay.Sunset;
        }
        else if (value == 22)
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, 10f));
            StartCoroutine(LerpLight(graddientSunsetToNight, 10f));
            currentTimeOfDay = TimeOfDay.Night;
        }
    }

    private IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
    {
        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }
        RenderSettings.skybox.SetTexture("_Texture1", b);
    }

    private IEnumerator LerpLight(Gradient lightGradient, float time)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            globalLight.color = lightGradient.Evaluate(i / time);
            RenderSettings.fogColor = globalLight.color;
            yield return null;
        }
    }

    private void ApplySkyboxForCurrentTime()
    {
        if (Hours >= 6 && Hours < 8)
        {
            RenderSettings.skybox.SetTexture("_Texture1", skyboxNight);
            RenderSettings.skybox.SetTexture("_Texture2", skyboxSunrise);
            RenderSettings.skybox.SetFloat("_Blend", 1f);
            globalLight.color = graddientNightToSunrise.Evaluate(1f);
            RenderSettings.fogColor = globalLight.color;
            currentTimeOfDay = TimeOfDay.Sunrise;
        }
        else if (Hours >= 8 && Hours < 18)
        {
            RenderSettings.skybox.SetTexture("_Texture1", skyboxSunrise);
            RenderSettings.skybox.SetTexture("_Texture2", skyboxDay);
            RenderSettings.skybox.SetFloat("_Blend", 1f);
            globalLight.color = graddientSunriseToDay.Evaluate(1f);
            RenderSettings.fogColor = globalLight.color;
            currentTimeOfDay = TimeOfDay.Day;
        }
        else if (Hours >= 18 && Hours < 22)
        {
            RenderSettings.skybox.SetTexture("_Texture1", skyboxDay);
            RenderSettings.skybox.SetTexture("_Texture2", skyboxSunset);
            RenderSettings.skybox.SetFloat("_Blend", 1f);
            globalLight.color = graddientDayToSunset.Evaluate(1f);
            RenderSettings.fogColor = globalLight.color;
            currentTimeOfDay = TimeOfDay.Sunset;
        }
        else
        {
            RenderSettings.skybox.SetTexture("_Texture1", skyboxSunset);
            RenderSettings.skybox.SetTexture("_Texture2", skyboxNight);
            RenderSettings.skybox.SetFloat("_Blend", 1f);
            globalLight.color = graddientSunsetToNight.Evaluate(1f);
            RenderSettings.fogColor = globalLight.color;
            currentTimeOfDay = TimeOfDay.Night;
        }
    }

    private void EndGame()
    {
        gameEnded = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (endGameCanvas != null)
        {
            inventoryCanvas.SetActive(false);
            endGameCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("End Game Canvas is not assigned in the TimeManager!");
        }
    }

    public float CurrentHour
    {
        get { return Hours + (Minutes / 60f); }
    }

    public void ContinueEndlessMode()
    {
        endGameCanvas.SetActive(false);
        inventoryCanvas.SetActive(true);
        hasContinued = true; 
        gameEnded = false;
        Time.timeScale = 1f;
    }


    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Main Menu");
    }

}