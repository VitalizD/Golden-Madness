using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Paused : MonoBehaviour
{
    public static bool gameIsPause = false;
    public static bool gameIsControl = false;
    public static bool gameIsExitMenu = false;

    [SerializeField] GameObject pause;
    [SerializeField] GameObject control;
    [SerializeField] GameObject exitMenu;

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
        if (gameIsControl || gameIsExitMenu)
        {
            Pause();
        }
    }

    public void Resume()
    {
        pause.SetActive(false);
        Time.timeScale = 1f;
        gameIsPause = false;
    }

    void Pause()
    {
        pause.SetActive(true);
        Time.timeScale = 0f;
        gameIsPause = true;
    }

    public void Control()
    {
        control.SetActive(true);
        gameIsControl = true;
    }

    public void ControlResume()
    {
        control.SetActive(false);
        gameIsControl = false;
    }

    public void ExitMenu()
    {
        exitMenu.SetActive(true);
        gameIsExitMenu = true;
    }

    public void ExitMenuResume()
    {
        exitMenu.SetActive(false);
        gameIsExitMenu = false;
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScreen");
    }

    public void ToVillage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Village");
    }
}