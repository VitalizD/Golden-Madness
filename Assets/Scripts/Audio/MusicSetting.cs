using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSetting : MonoBehaviour
{
    [SerializeField] private Slider musicVolume;
    public static MusicSetting Instanse;
    [SerializeField] [Range(0, 1)] private float defaultVolume = 0.5f;

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
        }
        else if (Instanse == this)
            Destroy(gameObject);
        musicVolume.onValueChanged.AddListener(vol => SaveMusicVolPref(vol));
        musicVolume.value = PlayerPrefs.GetFloat(SoundSettingsPrefs.MusicVolume, defaultVolume);
    }

    private void SaveMusicVolPref(float value)
    {
        PlayerPrefs.SetFloat(SoundSettingsPrefs.MusicVolume, value);
    }
}
