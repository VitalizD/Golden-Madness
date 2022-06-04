using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Color Highlighted;
    private Color Normal;

    void Start()
    {
        Normal = GetComponentInChildren<Text>().color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().color = Highlighted;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().color = Normal;
    }
}
