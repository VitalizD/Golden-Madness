using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingText;

    private bool teleporterResumed = false;


    private void OnLevelWasLoaded(int level)
    {
        if (LevelGeneration.Instanse == null)
        {
            return;
        }
        Teleporter.Instanse.Pause();
        loadingText.SetActive(true);
        teleporterResumed = false;
    }

    private void Update()
    {
        if (LevelGeneration.Instanse == null)
        {
            return;
        }
        if (LevelGeneration.Instanse.IsGenerated && !teleporterResumed)
        {
            Teleporter.Instanse.Resume();
            loadingText.SetActive(false);
            teleporterResumed = true;
        }
    }
}
