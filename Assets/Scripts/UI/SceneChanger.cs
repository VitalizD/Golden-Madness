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

	public void Exit()
	{
		Application.Quit();
	}
}
