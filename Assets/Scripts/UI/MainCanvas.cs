using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas instanse = null;

    public void FinishTutorial()
    {
        ServiceInfo.TutorialDone = true;
        PlayerPrefs.SetString("TutorialDone", true.ToString());
    }

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
