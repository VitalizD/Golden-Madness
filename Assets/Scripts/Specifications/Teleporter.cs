using UnityEngine;
using UnityEngine.UI;
using System;

public class Teleporter : MonoBehaviour
{
    public enum States
    {
        Stayed,
        Darkening,
        Lightening
    }

    [Tooltip("Чем выше, тем больше продлится обездвиживание персонажа")]
    [SerializeField] private float maxStunPlayerTime = 3f;

    private float fadeSpeed = 1.2f;
    private float alphaInterpolation = 0;
    private States state = States.Stayed;
    private Vector2 toPosition;
    private Action actionAfterTransition;

    private Image blackFilterImage;

    public States State { get => state; }

    public void Go(Vector2 to, Action actionAfterTransition, float fadeSpeed)
    {
        toPosition = to;
        Go(actionAfterTransition, fadeSpeed);
    }

    public void Go(Action actionAfterTransition, float fadeSpeed)
    {
        blackFilterImage.enabled = true;
        this.fadeSpeed = fadeSpeed;
        this.actionAfterTransition = actionAfterTransition;
        state = States.Darkening;

        if (Player.instanse != null)
            Player.instanse.SetStun(maxStunPlayerTime - fadeSpeed);
    }

    private void Awake()
    {
        blackFilterImage = GetComponent<Image>();
    }

    private void Start()
    {
        blackFilterImage.enabled = false;
    }

    private void Update()
    {
        ChangeAlpha();
    }

    private void ChangeAlpha()
    {
        if (state != States.Stayed)
        {
            var delta = Time.deltaTime * fadeSpeed;

            if (state == States.Darkening && alphaInterpolation < 1)
            {
                alphaInterpolation += delta;

                if (alphaInterpolation >= 1)
                    Teleport();
            }
            else if (state == States.Lightening && alphaInterpolation > 0)
            {
                alphaInterpolation -= delta;

                if (alphaInterpolation <= 0)
                {
                    state = States.Stayed;
                    blackFilterImage.enabled = false;
                }
            }

            if (blackFilterImage != null)
                blackFilterImage.color = new Color(blackFilterImage.color.r, blackFilterImage.color.g, blackFilterImage.color.b, alphaInterpolation);
        }
    }

    private void Teleport()
    {
        if (toPosition != Vector2.zero && Player.instanse != null)
            Player.instanse.transform.position = toPosition;

        actionAfterTransition?.Invoke();
        actionAfterTransition = null;
        state = States.Lightening;
        toPosition = Vector2.zero;
    }
}
