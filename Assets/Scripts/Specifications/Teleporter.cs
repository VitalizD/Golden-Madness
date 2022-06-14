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

    public static Teleporter instanse = null;

    [Tooltip("Чем выше, тем больше продлится обездвиживание персонажа")]
    [SerializeField] private float maxStunPlayerTime = 3f;

    private float fadeSpeed = 1.2f;
    private float alphaInterpolation = 0;
    private States state = States.Stayed;
    private Vector2 toPosition;
    private Action actionAfterTransition;
    private Action actionAfterLightening;

    private Image blackFilterImage;

    public States State { get => state; }

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
        state = States.Darkening;

        if (Player.Instanse != null)
            Player.Instanse.SetStun(maxStunPlayerTime - fadeSpeed);
    }

    public void Stop()
    {
        state = States.Stayed;
        blackFilterImage.enabled = false;
        alphaInterpolation = 0;
        blackFilterImage.color = new Color(blackFilterImage.color.r, blackFilterImage.color.g, blackFilterImage.color.b, 0);
    }

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else if (instanse == this)
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
        state = States.Lightening;
        toPosition = Vector2.zero;
    }
}
