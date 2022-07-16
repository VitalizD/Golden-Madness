using UnityEngine;
using UnityEngine.SceneManagement;

public class Paused : MonoBehaviour
{
    public static Paused Instanse { get; private set; } = null;

    private static bool gameIsPause = false;
    private static bool gameIsControl = false;
    private static bool gameIsExitMenu = false;

    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject control;
    [SerializeField] private GameObject exitMenu;
    [SerializeField] private float fadeSpeed = 0.7f;
    [SerializeField] private ColorButton[] buttons;

    private SpriteRenderer selection;

    public bool IsPaused { get => gameIsPause; }

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);

        selection = GameObject.FindGameObjectWithTag(ServiceInfo.SelectionTag).GetComponent<SpriteRenderer>();
        Pause();
        Resume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Teleporter.Instanse.State == Teleporter.States.Stayed)
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
        selection.enabled = true;
        pause.SetActive(false);
        Time.timeScale = 1f;
        gameIsPause = false;

        foreach (var button in buttons)
        {
            button.InitColor();
        }
    }

    public void Pause()
    {
        selection.enabled = false;
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
        gameIsExitMenu = false;
        gameIsPause = false;
        pause.SetActive(false);
        exitMenu.SetActive(false);
        Time.timeScale = 1f;
        Teleporter.Instanse.Go(() => SceneManager.LoadScene("MainScreen"), fadeSpeed);
    }

    public void ToVillage()
    {
        gameIsPause = false;
        Time.timeScale = 1f;
        pause.SetActive(false);
        Teleporter.Instanse.Go(() => SceneManager.LoadScene("Village"), fadeSpeed);
    }
}