using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    //[SerializeField] private AudioClip; 
    private Dictionary<SoundName, float> soundTimerDict;

    public enum SoundName {
        PlayerHit,
        PlayerDig,
        PlayerWalk,
        PlayerJump,
        PlayerSwing
    };
    //[SerializeField] private SoundAudioClip[] soundAudioClipArray;
    [SerializeField] private List<SoundAudioClip> soundAudioClipList;
    //[SerializeField] private Dictionary<SoundName,AudioClip> soundAudioClipArray;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    private static void Initialize() {
        Instance.soundTimerDict = new Dictionary<SoundName, float>();
        Instance.soundTimerDict[SoundName.PlayerWalk] = 0f;
    }

    private bool CanPlaySound(SoundName sound) {
        switch (sound) {
            default:
                return true;
            case SoundName.PlayerWalk:
                if (soundTimerDict.ContainsKey(sound))
                {
                    float maxTime = 0.25f;
                    float clipTime = GetAudioClip(SoundName.PlayerWalk).length;
                    float lastTimePlayed = soundTimerDict[sound];
                    //float playerMoveTimerMax = clipTime * 1.5f ;
                    //Debug.Log(playerMoveTimerMax);
                    //float playerMoveTimerMax = 0.5f;
                    if (lastTimePlayed + maxTime < Time.time)
                    {
                        soundTimerDict[sound] = Time.time;
                        return true;
                    }
                    else
                        return false;
                }
                else 
                    return true;
                
        
        }
    
    }

    public void PlaySound(SoundName sound) {
        if (CanPlaySound(sound)) 
        {
            GameObject soundGameObject = new GameObject(sound.ToString()+" sound");
            soundGameObject.transform.parent = Instance.transform;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            //Debug.Log(audioSource.clip.length);
            soundGameObject.AddComponent<DestroySoundObjectAfterDonePlaying>();
            audioSource.Play();
        }
        
    }

    private AudioClip GetAudioClip(SoundName soundName) {
        //soundAudioClipArray
        System.Random rnd = new System.Random();
        var listOfSFX=soundAudioClipList.FindAll(x => x.soundName == soundName);
        return listOfSFX[rnd.Next(listOfSFX.Count)].audioClip ;
        //foreach (var sound in soundAudioClipArray) 
        //{
        //    if (sound.soundName == soundName) {
        //        return sound.audioClip;
        //    }
        //}
        //return null;
    }

    [System.Serializable]
    private class SoundAudioClip {
        public SoundName soundName;
        public AudioClip audioClip;
    }
}
