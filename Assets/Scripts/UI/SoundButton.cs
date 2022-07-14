using UnityEngine;

public class SoundButton : MonoBehaviour
{
    public AudioSource myFx;
    public AudioClip hoverFx;
    public AudioClip clickFx;

    private void Start()
    {
        if (myFx != null)
            myFx.volume = SFX.MasterVol;
        SoundSetting.Instanse.GameVolume.onValueChanged.AddListener(value => ChangeVol(value));
    }

    public void HoverSound()
    {
        myFx.PlayOneShot(hoverFx);
    }

    public void ClickSound()
    {
        myFx.PlayOneShot(clickFx);
    }

    public void ChangeVol(float value)
    {
        if (myFx != null) myFx.volume = value;
    }
}
