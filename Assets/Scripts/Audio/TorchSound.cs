using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchSound : MonoBehaviour
{
    [SerializeField] private SFX torchSFX;
    private AudioSource torchAudioSource;


    private void Awake()
    {
        torchSFX.Position = gameObject.transform.position;
        torchAudioSource = torchSFX.Play();
    }

    void Update()
    {
        SoundSetting.Instanse.GameVolume.onValueChanged.AddListener(value => torchAudioSource.volume = value);
    }
}
