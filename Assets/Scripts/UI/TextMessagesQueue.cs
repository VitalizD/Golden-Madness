using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class TextMessagesQueue : MonoBehaviour
{
    public static TextMessagesQueue Instanse { get; private set; } = null;

    private const string showAnimationName = "Show";
    private const string hideAnimationName = "Hide";

    //[SerializeField] private bool inVillage = false;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;
    [SerializeField] private SFX addItemSFX;

    private Animation anim;

    private readonly Queue<(string text, Sprite icon, float delay)> consumablesQueue = new Queue<(string text, Sprite icon, float delay)>();
    private bool isPlaying = false;

    public void Add(string text, Sprite icon, float delay = 1f)
    {
        consumablesQueue.Enqueue((text, icon, delay));
        Play();
    }

    private void Play()
    {
        if (isPlaying)
            return;
        addItemSFX.Play();
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

        anim = GetComponent<Animation>();
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
        addItemSFX.Play();
        var message = consumablesQueue.Dequeue();
        text.text = message.text;

        if (message.icon == null)
            icon.enabled = false;
        else
        {
            icon.enabled = true;
            icon.sprite = message.icon;
        }

        anim.Play(showAnimationName);
        StartCoroutine(WaitAndNext(message.delay));
    }

    private IEnumerator WaitAndNext(float delay)
    {
        yield return new WaitForSeconds(anim.GetClip(showAnimationName).length + delay);
        anim.Play(hideAnimationName);
        yield return new WaitForSeconds(anim.GetClip(hideAnimationName).length + 0.01f);
        Next();
    }
}
