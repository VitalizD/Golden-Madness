using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickaxeStrengthBar : MonoBehaviour
{
    public Slider slider;
    [SerializeField] private Player player;
    
    public void Start()
    {
        slider.maxValue = 100f;
        slider.value = player.PickaxeStrength;
    }

    public void SetStrength(float strength) 
    {
        slider.value = strength / 100f;
    }
}
