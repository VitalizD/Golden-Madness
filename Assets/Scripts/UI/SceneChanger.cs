using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
	private const float fadeSpeed = 0.7f;

	[SerializeField] private GameObject control;
	[SerializeField] private GameObject exitMenu;
	[SerializeField] private string sceneName = "";

	private Teleporter teleporter;

    public string SceneName { get => sceneName;}

    private void Awake()
    {
		var blackFilter = GameObject.FindGameObjectWithTag(ServiceInfo.BlackFilterTag);
		if (blackFilter != null)
			teleporter = blackFilter.GetComponent<Teleporter>();
	}

    public void ChangeScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void ChangeScene()
	{
		SceneManager.LoadScene(sceneName);
	}

	public void GoToTutorialLevelOrVillage()
    {
		var playButton = GetComponent<PlayButton>();
		if (playButton != null && playButton.ForcedTutorial)
        {
			ServiceInfo.TutorialDone = false;
			LoadTutorial(ServiceInfo.TutorialLevel);
			return;
		}

		var tutorialDoneInVillage = bool.Parse(PlayerPrefs.GetString(PlayerPrefsKeys.TutorialDone, "false"));
		var tutorialDoneInCave = bool.Parse(PlayerPrefs.GetString(PlayerPrefsKeys.TutorialDoneInCave, "false"));
		ServiceInfo.TutorialDone = tutorialDoneInVillage;

		if (tutorialDoneInVillage)
			teleporter.Go(() => SceneManager.LoadScene(ServiceInfo.VillageScene), 0.7f);
		else if (tutorialDoneInCave)
			LoadTutorial(ServiceInfo.VillageScene);
		else
			LoadTutorial(ServiceInfo.TutorialLevel);
    }

	public void Exit()
	{
		Application.Quit();
	}

	private void LoadTutorial(string scene)
    {
		PlayerPrefs.DeleteAll();
		teleporter.Go(() => SceneManager.LoadScene(scene), fadeSpeed);
	}

	public void Control()
    {
		control.SetActive(true);
    }

	public void ExitMenu()
    {
		exitMenu.SetActive(true);
    }

	public void ControlButton()
	{
		control.SetActive(false);
	}

	public void ExitMenuReturn()
	{
		exitMenu.SetActive(false);
	}
}
