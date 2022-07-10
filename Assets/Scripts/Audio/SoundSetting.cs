using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private Slider gameVolume;
    public static SoundSetting Instanse;
    //public delegate void VolumeChange(float x);
    //public event VolumeChange onVolumeChanged;

    public Slider GameVolume { get => gameVolume;}

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);
        gameVolume.value = 0.5f;
    }

    //some objects awake generates faster
    public void Init()
    {
        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);
        gameVolume.value = 0.5f;
    }

    private void Update()
    {
        gameVolume.gameObject.transform.position = Player.Instanse.transform.position+new Vector3(0,1,0);
        //gameVolume.onValueChanged.AddListener(x => onVolumeChanged?.Invoke(x));

    }
    
}
