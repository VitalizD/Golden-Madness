using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Paused : MonoBehaviour
{
    public static bool gameIsPause = false;
    public static bool gameIsControl = false;

    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject control;
    //[SerializeField] GameObject exitMenu;

    //void Start()
    //{
    //    pause.SetActive(false);
    //    exitMenu.SetActive(false);
    //    control.SetActive(false);
    //}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        while (gameIsControl)
        {
            Time.timeScale = 0f;
        }
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gameIsPause = false;
    }

    void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gameIsPause = true;
    }

    public void Control()
    {
        control.SetActive(true);
        gameIsControl = true;
    }

    //public void Menu()
    //{
    //    pause.SetActive(false);
    //    exitMenu.SetActive(true);
    //    Time.timeScale = 0;
    //}

    //public void Return()
    //{
    //    pause.SetActive(true);
    //    exitMenu.SetActive(false);
    //    control.SetActive(false);
    //    Time.timeScale = 0;
    //}

    //public void Exit()
    //{
    //    SceneManager.LoadScene(1);
    //    Time.timeScale = 1;
    //}
}
