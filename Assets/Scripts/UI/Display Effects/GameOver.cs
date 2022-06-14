using UnityEngine;
using System.Collections;
using TMPro;
using Agava.YandexGames;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverAd;
    [SerializeField] private float timeBeforeTeleport = 7f;
    [SerializeField] private float fadeSpeed = 0.5f;

    [Header("Completed Levels Text")]
    [SerializeField] private TextMeshProUGUI completedLevelsText;
    [SerializeField] private float timeBeforeCompletedLevels = 1f;
    [SerializeField] private string completedLevelsTextPrefix = "Пройдено уровней: ";

    private Teleporter teleporter;
    private SceneChanger sceneChanger;
    private TextMeshProUGUI gameOverText;
    private Animation gameOverTextAnimation;
    private Animation completedLevelTextAnimation;

    private readonly string showAnimationName = "Show";

    public void ShowGameOverAd()
    {
        gameOverAd.SetActive(true);
    }

    public void Ad()
    {
        VideoAd.Show();
        gameOverAd.SetActive(false);
        Player.Instanse.ViewedAd();
    }

    public void ShowAndReturnToVillage()
    {
        Player.Instanse.NonViewedAd();
        gameOverAd.SetActive(false);
        gameOverText.enabled = true;
        gameOverTextAnimation.Play(showAnimationName);
        StartCoroutine(GoToVillage());
        StartCoroutine(ShowCompletedLevelText());
    }

    private void Awake()
    {
        gameOverTextAnimation = GetComponent<Animation>();
        completedLevelTextAnimation = completedLevelsText.GetComponent<Animation>();
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.BlackFilterTag).GetComponent<Teleporter>();
        sceneChanger = GetComponent<SceneChanger>();
        gameOverText = GetComponent<TextMeshProUGUI>();
        gameOverText.enabled = false;
        completedLevelsText.enabled = false;
    }

    private IEnumerator GoToVillage()
    {
        yield return new WaitForSeconds(timeBeforeTeleport);
        void action() 
        { 
            sceneChanger.ChangeScene();
            Player.Instanse.gameObject.SetActive(true);
            gameOverText.enabled = false;
            completedLevelsText.enabled = false;
        }
        teleporter.Go(action, fadeSpeed);
    }

    private IEnumerator ShowCompletedLevelText()
    {
        var currentLevel = PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentLevelNumber, 0);
        completedLevelsText.text = completedLevelsTextPrefix + (currentLevel - 1).ToString();
        yield return new WaitForSeconds(timeBeforeCompletedLevels);
        completedLevelsText.enabled = true;
        completedLevelTextAnimation.Play(showAnimationName);
    }
}
