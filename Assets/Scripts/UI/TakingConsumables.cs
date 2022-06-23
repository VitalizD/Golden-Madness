using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class TakingConsumables : MonoBehaviour
{
    public static TakingConsumables Instanse { get; private set; } = null;

    private const string textAnimationName = "Show";
    private const string iconAnimationName = "Show";

    [SerializeField] private bool inVillage = false;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;

    private Animation textAnimation;
    private Animation iconAnimation;

    private readonly Queue<(string name, int count, Sprite icon)> consumablesQueue = new Queue<(string name, int count, Sprite icon)>();
    private bool isPlaying = false;

    public void AddConsumable(string name, int count, Sprite icon)
    {
        if (inVillage)
            return;

        consumablesQueue.Enqueue((name, count, icon));
        Play();
    }

    private void Play()
    {
        if (isPlaying)
            return;

        gameObject.SetActive(true);
        isPlaying = true;
        Next();
    }

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);

        textAnimation = text.GetComponent<Animation>();
        iconAnimation = icon.GetComponent<Animation>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Next()
    {
        if (consumablesQueue.Count == 0)
        {
            isPlaying = false;
            gameObject.SetActive(false);
            return;
        }

        var consumable = consumablesQueue.Dequeue();
        text.text = $"{consumable.name} x{consumable.count}";
        icon.sprite = consumable.icon;

        textAnimation.Play(textAnimationName);
        iconAnimation.Play(iconAnimationName);

        StartCoroutine(WaitAndNext());
    }

    private IEnumerator WaitAndNext()
    {
        yield return new WaitForSeconds(textAnimation.GetClip(textAnimationName).length + 0.01f);
        Next();
    }
}
