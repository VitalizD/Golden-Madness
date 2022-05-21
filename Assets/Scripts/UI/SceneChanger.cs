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
		var tutorialDone = bool.Parse(PlayerPrefs.GetString(PlayerPrefsKeys.TutorialDone, "false"));
		ServiceInfo.TutorialDone = tutorialDone;

		if (tutorialDone || !(playButton != null && playButton.ForcedTutorial))
			SceneManager.LoadScene(ServiceInfo.VillageScene);
		else
        {
			PlayerPrefs.DeleteAll();
			SceneManager.LoadScene(ServiceInfo.TutorialLevel);
		}
    }

	public void Exit()
	{
		Application.Quit();
	}
}
