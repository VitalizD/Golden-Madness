using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    //[SerializeField] private float movingSpeed = 20f;
    [SerializeField] private float colorChangeSpeed = 5f;
    [SerializeField] private float flickerDuration = 0.25f;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color alternativeColor;

    private float zPosition;
    private float colorInterpolation = 0;
    private bool enableActiveColor;
    private Color normalColor;
    private SpriteRenderer sprite;

    public void Move(Vector2 toPoint)
    {
        //for animation of selection

        /*transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(toPoint.x, toPoint.y, zPosition),
            movingSpeed * Time.deltaTime);*/

        //insta changing position

        transform.position = new Vector3(toPoint.x, toPoint.y, zPosition);
    }

    public void SetActiveColor() => enableActiveColor = true;

    public void SetNormalColor() => enableActiveColor = false;

    public void SetActive(bool value)
    {
        if (LevelGeneration.Instanse == null || LevelGeneration.Instanse.IsGenerated)
            gameObject.SetActive(value);
    }

    private void Awake()
    {
        zPosition = transform.position.z;
        sprite = GetComponent<SpriteRenderer>();
        normalColor = sprite.color;
    }

    private void Update()
    {
        if (colorInterpolation > 0 || enableActiveColor)
        {
            if (enableActiveColor)
            {
                if (colorInterpolation < 1)
                    colorInterpolation += Time.deltaTime * colorChangeSpeed;
            }
            else if (colorInterpolation > 0)
                colorInterpolation -= Time.deltaTime * colorChangeSpeed;

            sprite.color = Color.Lerp(normalColor, activeColor, colorInterpolation);
        }
        else
            sprite.color = Color.Lerp(normalColor, alternativeColor, Mathf.PingPong(Time.time, flickerDuration));
    }
}
