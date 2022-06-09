using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickaxeStrengthBar : MonoBehaviour
{
    public Slider slider;

    //public void Start()
    //{
    //    slider.maxValue = 100f;
    //    slider.value = 100f;
    //}

    public void SetStrength(float strength) 
    {
        slider.value = strength / 100f;
    }
}
