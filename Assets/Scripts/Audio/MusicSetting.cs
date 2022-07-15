using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSetting : MonoBehaviour
{
    [SerializeField] private Slider musicVolume;
    public static MusicSetting Instanse;

    //private MusicSetting()
    //{
    //    musicVolume.value = 0.01f;
    //}
    public Slider MusicVolume { get => musicVolume; }
    //public delegate void VolumeChange(float x);
    //public event VolumeChange onVolumeChanged;



    private void Awake()
    {
        if (Instanse == null)
        {
            Instanse = this;
            DontDestroyOnLoad(Instanse);
        }
        else if (Instanse == this)
            Destroy(gameObject);
        musicVolume.value = 0.01f;
    }

    private void Update()
    {
        //if (Player.Instanse!=null) 
        //    musicVolume.gameObject.transform.position = Player.Instanse.transform.position - new Vector3(0, 1, 0);
        //gameVolume.onValueChanged.AddListener(x => onVolumeChanged?.Invoke(x));

    }
}
