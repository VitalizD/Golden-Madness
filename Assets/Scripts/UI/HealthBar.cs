using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Text hp;
    [SerializeField] private Player player;

    public void Start()
    {
        slider.maxValue = 100;
        slider.value = player.Health;
        hp.text = player.Health + "/100";
    }
    public void SetHealth(int health)
    {
        slider.value = health;
        hp.text = health + "/100";
    }

}
