using UnityEngine;
using System;
using TMPro;

public class InformationWindow : MonoBehaviour
{
    public static InformationWindow instance = null;

    private Animation animation_;
    private TextMeshProUGUI text;
    private Func<bool> hidingCondition = null;

    private readonly int textChildNumber = 0;
    private readonly string showAnimationName = "Show";
    private readonly string hideAnimationName = "Hide";

    public void Show(string text, Func<bool> hidingCondition)
    {
        this.text.text = text;
        this.hidingCondition = hidingCondition;
        animation_.Play(showAnimationName);
    }

    public void Hide()
    {
        animation_.Play(hideAnimationName);
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance == this)
            Destroy(gameObject);

        animation_ = GetComponent<Animation>();
        text = transform.GetChild(textChildNumber).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (hidingCondition != null && hidingCondition())
        {
            hidingCondition = null;
            Hide();
        }
    }
}
