using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
	[SerializeField] private string sceneName = "";

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
			LoadTutorial();
			return;
		}

		var tutorialDone = bool.Parse(PlayerPrefs.GetString(PlayerPrefsKeys.TutorialDone, "false"));
		ServiceInfo.TutorialDone = tutorialDone;

		if (tutorialDone)
			SceneManager.LoadScene(ServiceInfo.VillageScene);
		else
			LoadTutorial();
    }

	public void Exit()
	{
		Application.Quit();
	}

	private void LoadTutorial()
    {
		PlayerPrefs.DeleteAll();
		SceneManager.LoadScene(ServiceInfo.TutorialLevel);
	}
}
