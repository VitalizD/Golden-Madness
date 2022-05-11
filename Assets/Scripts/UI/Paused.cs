using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Paused : MonoBehaviour
{
    [SerializeField] GameObject pause;
    [SerializeField] GameObject exitMenu;

    void Start()
    {
        pause.SetActive(false);
        exitMenu.SetActive(false);
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
        SceneManager.LoadScene(5);
        Time.timeScale = 1;
    }

    public void Control()
    {
        SceneManager.LoadScene(4);
        Time.timeScale = 1;
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
        Time.timeScale = 0;
    }

    public void Exit()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }
}
