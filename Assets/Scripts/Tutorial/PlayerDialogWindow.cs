using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerDialogWindow : MonoBehaviour
{
    private Animation animation_;
    private TextMeshProUGUI text;

    private Coroutine waitAndHide = null;

    private readonly string showAnimationName = "Show";
    private readonly string hideAnimationName = "Hide";
    private (string text, float timeIsSeconds) inQueue = (null, 0);
    private bool isShowed = false;

    public void Show(string text, float timeInSeconds = 4f)
    {
        inQueue = (text, timeInSeconds);
        Show();
    }

    public void Hide()
    {
        animation_.Play(hideAnimationName);
    }

    private void Awake()
    {
        animation_ = GetComponent<Animation>();
        text = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    // Назначен на ключ в анимации "Hide"
    private void OnEndHideAnimation()
    {
        isShowed = false;
        if (inQueue.text != null)
            Show();
        else
            gameObject.SetActive(false);
    }

    private void Show()
    {
        if (isShowed)
        {
            StopCoroutine(waitAndHide);
            Hide();
        }
        else if (inQueue.text != null)
        {
            this.text.text = inQueue.text;
            animation_.Play(showAnimationName);
            waitAndHide = StartCoroutine(WaitAndHide(inQueue.timeIsSeconds));
            inQueue = (null, 0);
            isShowed = true;
        }
    }

    private IEnumerator WaitAndHide(float time)
    {
        yield return new WaitForSeconds(time);
        Hide();
    }
}
