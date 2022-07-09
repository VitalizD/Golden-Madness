using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private Slider gameVolume;
    public static SoundSetting Instanse;

    public Slider GameVolume { get => gameVolume;}

    private void Awake()
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
        gameVolume.onValueChanged.AddListener(x => SFX.MasterVol = x);
    }
    
}
