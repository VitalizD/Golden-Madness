using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private Slider gameVolume;
    [SerializeField][Range(0,1)] private static float defaultVolume=0.5f;

    public static SoundSetting Instanse;
    //public delegate void VolumeChange(float x);
    //public event VolumeChange onVolumeChanged;

    public Slider GameVolume { get => gameVolume;}
    public static float DefaultVolume { get => defaultVolume;}

    private void Awake()
    {
        if (Instanse == null)
        {
            Instanse = this;
        }
        else if (Instanse == this)
            Destroy(gameObject);
        gameVolume.onValueChanged.AddListener(vol => SaveSoundVolPref(vol));
        gameVolume.value = PlayerPrefs.GetFloat(SoundSettingsPrefs.SoundVolume, defaultVolume);

    }
    
    private void SaveSoundVolPref(float value)
    {
        PlayerPrefs.SetFloat(SoundSettingsPrefs.SoundVolume, value);
    }
    
}
