using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class TorchSound : MonoBehaviour
{
    [SerializeField] private SFX torchSFX;



    private void Awake()
    {
        //this.Log(gameObject);
        torchSFX.Position = gameObject.transform.position;
        torchSFX.Play();
        //SoundSetting.Instanse.GameVolume.onValueChanged.AddListener(SFX.);
    }

    private void Update()
    {
        if (!torchSFX.AudioSource.isPlaying)
        {
            this.Log(gameObject);
        }
    }

}
