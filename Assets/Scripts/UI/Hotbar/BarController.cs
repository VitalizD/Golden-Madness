using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI value;

    public float GetValue() => slider.value * 100f;

    public void SetValue(float value)
    {
        if (slider != null)
            slider.value = value / 100f;

        if (this.value != null)
            this.value.text =  $"{Mathf.Round(value)} / 100";
    }
}
