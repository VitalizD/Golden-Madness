using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private Slider gameVolume;
    public static SoundSetting Instanse;
    private Camera camera;
    //public delegate void VolumeChange(float x);
    //public event VolumeChange onVolumeChanged;

    public Slider GameVolume { get => gameVolume;}

    private void Awake()
    {
        if (Instanse == null)
        {
            Instanse = this;
            DontDestroyOnLoad(Instanse);
        }
        else if (Instanse == this)
            Destroy(gameObject);
        gameVolume.value = 0.01f;
    }
    private void Start()
    {
        //CameraController.Instanse.TryGetComponent<Camera>(out camera);
    }

    private void Update()
    {

        //if(camera!=null) gameVolume.gameObject.transform.position = camera.transform.position + new Vector3(0, 1, 0);
        //if (Player.Instanse != null)  gameVolume.gameObject.transform.position = Player.Instanse.transform.position+new Vector3(0,1,0);
        //gameVolume.onValueChanged.AddListener(x => onVolumeChanged?.Invoke(x));

    }
    
}
