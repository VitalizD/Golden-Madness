using UnityEngine;
using UnityEngine.SceneManagement;

public class Paused : MonoBehaviour
{
    private static bool gameIsPause = false;
    private static bool gameIsControl = false;
    private static bool gameIsExitMenu = false;

    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject control;
    [SerializeField] private GameObject exitMenu;
    [SerializeField] private float fadeSpeed = 0.7f;

    private void Update()
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

    public void Pause()
    {
        pause.SetActive(true);
        Time.timeScale = 0f;
        gameIsPause = true;
    }

    public void Control()
    {
        control.SetActive(true);
        gameIsControl = true;
        Teleporter.instanse.Stop();
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
        gameIsExitMenu = false;
        gameIsPause = false;
        pause.SetActive(false);
        exitMenu.SetActive(false);
        Time.timeScale = 1f;
        Teleporter.instanse.Go(() => SceneManager.LoadScene("MainScreen"), fadeSpeed);
    }

    public void ToVillage()
    {
        gameIsPause = false;
        Time.timeScale = 1f;
        pause.SetActive(false);
        Teleporter.instanse.Go(() => SceneManager.LoadScene("Village"), fadeSpeed);
    }
}