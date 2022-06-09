using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanityBar : MonoBehaviour
{
    public Slider slider;
    public Text sp;
    [SerializeField] private SanityController sanityController;

    //public void Start()
    //{
    //    slider.maxValue = 100f;
    //    slider.value = sanityController.Sanity;
    //    sp.text = sanityController.Sanity + "/100";
    //}

    public void SetSanity(float sanity)
    {
        slider.value = sanity / 100f;
        sp.text = System.Math.Round(sanity) + "/100";
    }
}
