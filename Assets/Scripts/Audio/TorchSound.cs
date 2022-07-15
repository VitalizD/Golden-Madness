using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class TorchSound : MonoBehaviour
{
    [SerializeField] private AudioSource torchAudioSource;



    private void Start()
    {
        //this.Log(gameObject);
        //For whatever fucking reason this fucking bullshit cant work with SFX system, here temp, duck tape solution. This is BAD, but whatever.
        torchAudioSource.volume = SFX.MasterVol;
        torchAudioSource.Play();
        SoundSetting.Instanse.GameVolume.onValueChanged.AddListener(value => torchAudioSource.volume = value);
        //SoundSetting.Instanse.GameVolume.onValueChanged.AddListener(SFX.);
    }


}
