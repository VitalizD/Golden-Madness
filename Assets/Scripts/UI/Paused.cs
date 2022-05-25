using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Paused : MonoBehaviour
{
    [SerializeField] GameObject pause;
    [SerializeField] GameObject exitMenu;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject control;

    void Start()
    {
        pause.SetActive(false);
        exitMenu.SetActive(false);
        settings.SetActive(false);
        control.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void PauseOff()
    {
        pause.SetActive(false);
        Time.timeScale = 1;
    }

    public void Settings()
    {
        pause.SetActive(false);
        settings.SetActive(true);
        Time.timeScale = 0;
    }

    public void Control()
    {
        pause.SetActive(false);
        control.SetActive(true);
        Time.timeScale = 0;
    }

    public void Menu()
    {
        pause.SetActive(false);
        exitMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Return()
    {
        pause.SetActive(true);
        exitMenu.SetActive(false);
        settings.SetActive(false);
        control.SetActive(false);
        Time.timeScale = 0;
    }

    public void Exit()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }
}
