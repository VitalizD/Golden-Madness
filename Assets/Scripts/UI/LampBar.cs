using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LampBar : MonoBehaviour
{
    public Slider slider;
    [SerializeField] private Lamp Lamp; 

    public void Start()
    {
        slider.maxValue = 100f;
        slider.value = Lamp.FuelCount;
    }

    public void Update()
    {
        slider.value = Lamp.FuelCount;
    }
    public void SetLamp(float lamp)
    {
        slider.value = lamp;
    }
}
