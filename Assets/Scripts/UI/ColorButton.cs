using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color highlighted;

    private Text text;
    private Color normal;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
        normal = text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = highlighted;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InitColor();
    }

    public void InitColor()
    {
        text.color = normal;
    }
}