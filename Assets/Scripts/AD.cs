using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AD : MonoBehaviour
{
    void Start()
    {
        ShowAd();
    }

    public void ShowAd()
    {
        Application.ExternalCall("ShowAd");
    }
}
