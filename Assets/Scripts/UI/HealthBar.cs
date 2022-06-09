using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Text hp;

    public void SetHealth(int health)
    {
        slider.value = health / 100f;
        hp.text = health + "/100";
    }
}
