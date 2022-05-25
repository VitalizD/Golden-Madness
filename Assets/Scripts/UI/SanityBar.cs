using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanityBar : MonoBehaviour
{
    public Slider slider;

    public void Start()
    {
        slider.maxValue = 100f;
        slider.value = 100f;
    }
    public void SetSanity(float sanity)
    {
        slider.value = sanity;
    }
}
