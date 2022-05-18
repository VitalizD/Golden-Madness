using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreen : MonoBehaviour
{
    public void ToggleFullScreen()
    {
        if (Screen.fullScreen = !Screen.fullScreen)
        {
            Screen.fullScreen = Screen.fullScreen;
            Screen.SetResolution(Screen.width, Screen.height, true, 60);
        }
        else
        {
            Screen.fullScreen = false;
        }
    }
}
