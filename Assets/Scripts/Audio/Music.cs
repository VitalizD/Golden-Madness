using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private float fadeTime = 2f;

    private void Start()
    {
        if(musicAudioSource!=null)
            musicAudioSource.volume = MusicSetting.Instanse.MusicVolume.value;
        MusicSetting.Instanse.MusicVolume.onValueChanged.AddListener(x=>ChangeMusicVol(x));
    }

    private void ChangeMusicVol(float value)
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = value;
        }
    }
    public IEnumerator MusicFade(bool isFadeOut)
    {
        var increment = isFadeOut ? -0.01f : 0.01f;
        if (musicAudioSource == null) yield break;
        var startVol = musicAudioSource.volume;
        for (float currentVol = startVol; 
            (isFadeOut&&currentVol > 0f)||(!isFadeOut&&currentVol<MusicSetting.Instanse.MusicVolume.value);
            currentVol += increment)
        {
            musicAudioSource.volume = currentVol;
            yield return new WaitForSeconds(fadeTime / 100);
        }
        if (isFadeOut) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }
}
