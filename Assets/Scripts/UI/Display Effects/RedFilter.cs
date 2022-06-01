using UnityEngine;
using UnityEngine.UI;

public class RedFilter : MonoBehaviour
{
    [SerializeField] private float removingFilterSpeed;
    [SerializeField] private float alphaLimit = 0.5f;

    private Image image;
    private float colorInterpolation = 0;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void ChangeColor(int delta)
    {
        colorInterpolation += (delta / 100f) * 2;
        if (colorInterpolation > alphaLimit)
            colorInterpolation = alphaLimit;
    }

    public void RemoveFilter()
    {
        colorInterpolation = 0;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }

    private void Update()
    {
        if (colorInterpolation > 0)
        {
            colorInterpolation -= Time.deltaTime * removingFilterSpeed;
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, colorInterpolation));
        }
    }
}
