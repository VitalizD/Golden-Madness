using UnityEngine;
using System.Collections;
using TMPro;
using Agava.YandexGames;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject GameOverAd;
    [SerializeField] private float timeBeforeTeleport = 5f;
    [SerializeField] private float fadeSpeed = 0.7f;

    private Animation animation_;
    private Teleporter teleporter;
    private SceneChanger sceneChanger;
    private TextMeshProUGUI text;
    private Player player;

    private readonly string showAnimationName = "Show";

    public void ShowGameOverAd()
    {
        GameOverAd.SetActive(true);
    }

    public void Ad()
    {
        VideoAd.Show();
        GameOverAd.SetActive(false);
        player.ViewedAd();
    }

    public void ShowAndReturnToVillage()
    {
        GameOverAd.SetActive(false);
        text.enabled = true;
        animation_.Play(showAnimationName);
        StartCoroutine(GoToVillage());
    }

    private void Awake()
    {
        animation_ = GetComponent<Animation>();
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.BlackFilterTag).GetComponent<Teleporter>();
        sceneChanger = GetComponent<SceneChanger>();
        text = GetComponent<TextMeshProUGUI>();
        text.enabled = false;
    }

    private IEnumerator GoToVillage()
    {
        yield return new WaitForSeconds(timeBeforeTeleport);
        void action() 
        { 
            sceneChanger.ChangeScene();
            Player.instanse.gameObject.SetActive(true);
            text.enabled = false;
        }
        teleporter.Go(new Vector2(), action, fadeSpeed);
    }
}
