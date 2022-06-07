using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Text hp;

    //public void Start()
    //{
    //    slider.maxValue = 100;
    //    slider.value = 100;
    //    hp.text = 100 + "/100";
    //}

    public void SetHealth(int health)
    {
        slider.value = health / 100f;
        hp.text = health + "/100";
    }

}
