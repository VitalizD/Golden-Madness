using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    //[SerializeField] private AudioClip; 
    public enum SoundName {
        PlayerHit,
        PlayerDig,
        PlayerWalk,
    };
    [SerializeField] private SoundAudioClip[] soundAudioClipArray;
    //[SerializeField] private Dictionary<SoundName,AudioClip> soundAudioClipArray;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(SoundName sound) {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(sound));
    }

    private AudioClip GetAudioClip(SoundName soundName) {
        foreach (var sound in soundAudioClipArray) 
        {
            if (sound.soundName == soundName) {
                return sound.audioClip;
            }
        }
        return null;
    }

    [System.Serializable]
    private class SoundAudioClip {
        public SoundName soundName;
        public AudioClip audioClip;
    }
}
