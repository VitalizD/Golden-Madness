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

    public static Teleporter Instanse { get; private set; } = null;

    [Tooltip("Чем выше, тем больше продлится обездвиживание персонажа")]
    [SerializeField] private float maxStunPlayerTime = 3f;

    private float fadeSpeed = 1.2f;
    private float alphaInterpolation = 0;
    private States currentState = States.Stayed;
    private Vector2 toPosition;
    private Action actionAfterTransition;
    private Action actionAfterLightening;

    private Image blackFilterImage;

    public States State { get => currentState; }

    public void Pause()
    {
        currentState = States.Stayed;
    }

    public void Resume()
    {
        if (alphaInterpolation > 0)
            currentState = States.Lightening;
    }

    public void Go(Vector2 to, Action actionAfterTransition, float fadeSpeed, Action actionAfterLightening = null)
    {
        toPosition = to;
        Go(actionAfterTransition, fadeSpeed, actionAfterLightening);
    }

    public void Go(Action actionAfterTransition, float fadeSpeed, Action actionAfterLightening = null)
    {
        blackFilterImage.enabled = true;
        this.fadeSpeed = fadeSpeed;
        this.actionAfterTransition = actionAfterTransition;
        this.actionAfterLightening = actionAfterLightening;
        currentState = States.Darkening;

        if (Player.Instanse != null)
            Player.Instanse.SetStun(maxStunPlayerTime - fadeSpeed);
    }

    public void Stop()
    {
        currentState = States.Stayed;
        blackFilterImage.enabled = false;
        alphaInterpolation = 0;
        blackFilterImage.color = new Color(blackFilterImage.color.r, blackFilterImage.color.g, blackFilterImage.color.b, 0);
    }

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);

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
        if (currentState != States.Stayed)
        {
            var delta = Time.deltaTime * fadeSpeed;

            if (currentState == States.Darkening && alphaInterpolation < 1)
            {
                alphaInterpolation += delta;

                if (alphaInterpolation >= 1)
                    Teleport();
            }
            else if (currentState == States.Lightening && alphaInterpolation > 0)
            {
                alphaInterpolation -= delta;

                if (alphaInterpolation <= 0)
                {
                    currentState = States.Stayed;
                    blackFilterImage.enabled = false;
                    actionAfterLightening?.Invoke();
                }
            }

            if (blackFilterImage != null)
                blackFilterImage.color = new Color(blackFilterImage.color.r, blackFilterImage.color.g, blackFilterImage.color.b, alphaInterpolation);
        }
    }

    private void Teleport()
    {
        if (toPosition != Vector2.zero && Player.Instanse != null)
            Player.Instanse.transform.position = toPosition;

        actionAfterTransition?.Invoke();
        actionAfterTransition = null;
        currentState = States.Lightening;
        toPosition = Vector2.zero;
    }
}
