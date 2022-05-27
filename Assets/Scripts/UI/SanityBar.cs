using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanityBar : MonoBehaviour
{
    public Slider slider;
    public Text sp;

    public void Start()
    {
        slider.maxValue = 100f;
        slider.value = 100f;
        sp.text = 100 + "/100";
    }
    public void SetSanity(float sanity)
    {
        slider.value = sanity;
        sp.text = System.Math.Round(sanity) + "/100";
    }
}
