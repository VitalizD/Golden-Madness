using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public static SceneMusic Instanse;

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);
    }

    public Transform SafeZoneMusic {
        get
        {
            return gameObject.transform.Find("SZMusic"); 
        }
    }
    public Transform LevelMusic
    {
        get
        {
            return gameObject.transform.Find("Music");
        }
    }
}
