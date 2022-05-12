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

    [SerializeField] private float fadeSpeed;

    private States state = States.Stayed;
    private float alphaInterpolation = 0;
    private Vector2 toPosition;
    private Action actionAfterTransition;

    private Image blackFilterImage;

    public States State { get => state; }

    public void Go(Vector2 to, Action actionAfterTransition)
    {
        this.actionAfterTransition = actionAfterTransition;
        toPosition = to;
        state = States.Darkening;
    }

    private void Awake()
    {
        blackFilterImage = GameObject
            .Find(ServiceInfo.MainCanvasName).transform
            .Find(ServiceInfo.BlackFilterName)
            .GetComponent<Image>();
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
                    state = States.Stayed;
            }

            blackFilterImage.color = new Color(blackFilterImage.color.r, blackFilterImage.color.g, blackFilterImage.color.b, alphaInterpolation);
        }
    }

    private void Teleport()
    {
        Player.instanse.transform.position = toPosition;
        actionAfterTransition?.Invoke();
        actionAfterTransition = null;
        state = States.Lightening;
    }
}
