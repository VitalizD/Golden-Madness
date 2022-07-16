using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using Utils;


[CreateAssetMenu(menuName = "SFX", fileName = "New SFX")]
public class SFX : ScriptableObject
{
    public enum playMethod {
        Play,
        PlayOneShot,
        PlayClipAtPoint,
        PlayDelayed,
        PlayScheduled
    }

    private Dictionary<playMethod, System.Action<AudioSource>> playMethodDict;

    private float value2D = 0f;
    private float value3D = 1f;
    private static float masterVol = SoundSetting.DefaultVolume;
    private AudioSource audioSource;
    public event System.Action<float> onVolumeChanged;

    [Range(0, 1)] [SerializeField] private float vol=1f;
    [SerializeField] private playMethod method;
    [SerializeField] private bool isLooped;
    [SerializeField] private bool is3D;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance=20f;
    [SerializeField] private Vector3 position = new Vector3(0,0,0);
    [SerializeField] private float delay;
    [SerializeField] private AudioRolloffMode rolloffMode=AudioRolloffMode.Linear;
    [SerializeField] private List<AudioClip> audioClips;

    public static float MasterVol 
    { 
        get => masterVol;
        set
        {
            if(value<0)
                masterVol = 0;
            else if (value > 1)
                masterVol = 1;
            else masterVol = value;
            //if (audioSource != null) audioSource.volume = vol * masterVol;
        }
    }
    public Vector3 Position { get => position; set => position = value; }
    public AudioSource AudioSource { get => audioSource;}


    //THIS BULLSHIT DOESN'T WORK LIKE M.O. AWAKE, NEED TO CREATE S.O. MANUALY (Like OnCreate).
    //REF:https://www.reddit.com/r/gamedev/comments/anjhfs/im_doing_some_weird_shit_with_scriptable_objects/
    //private void Awake()
    //{
    //UPDATE: NVM
    //Dont touch this bullshit https://forum.unity.com/threads/how-to-create-persistent-listener-to-an-event.264228/
    //    Debug.Log("Awake");
    //    onVolumeChanged += ChangeMasterVolume;
    //    var targetInfo = UnityEvent.GetValidMethodInfo(this, nameof(ChangeMasterVolume), new System.Type[0]);
    //    UnityAction methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), this, targetInfo) as UnityAction;//delegat ChangeMasterVolume
    //    var action = new UnityAction<GameObject>(onVolumeChanged);
    //    var slider = SoundSetting.Instanse.GameVolume;
    //    SoundSetting.Instanse.GameVolume.onValueChanged.AddPer((value) =>
    //    {
    //        MasterVol = value;
    //        Debug.Log($"Vol must be changed to {value}");
    //        if (audioSource != null)
    //        {
    //            audioSource.volume = vol * masterVol;
    //        }
    //    }
    //    );
    //    UnityEventTools.AddPersistentListener(BigExplosionEvent, methodDelegate);
    //    UnityEventTools.AddVoidPersistentListener(slider.onValueChanged, methodDelegate);
    //}

    public void ChangeMasterVolume(float volValue)
    {
        //Debug.Log($"Vol must be changed to {volValue}");
        MasterVol = volValue;
        //this.Log($"Vol must be changed to {masterVolume}");
        if (audioSource != null)
        {
            audioSource.volume = vol*masterVol;
        }
    }

    public AudioSource Play()
    {
        //this.Log(audioSource);
        if (audioSource == null)
        {
            onVolumeChanged += ChangeMasterVolume;
            SoundSetting.Instanse.GameVolume.onValueChanged.AddListener(x => onVolumeChanged(x));
            var _obj = new GameObject("Sound", typeof(AudioSource));
            audioSource = _obj.GetComponent<AudioSource>();
            audioSource.gameObject.transform.position = position;
            //_obj.transform.parent = audioSource.transform;
            audioSource.loop = isLooped;
            audioSource.volume = vol * masterVol;
            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;
            audioSource.rolloffMode = rolloffMode;
            audioSource.spatialBlend = is3D ? value3D : value2D;
        }
        playMethodDict =
            new Dictionary<playMethod, System.Action<AudioSource>>
            {
                {playMethod.Play, audioSrc=>audioSrc.Play() },
                {playMethod.PlayOneShot, audioSrc=>audioSrc.PlayOneShot(audioSrc.clip) },
                {playMethod.PlayDelayed, audioSrc=>audioSrc.PlayDelayed(delay) },
                {playMethod.PlayClipAtPoint, audioSrc => AudioSource.PlayClipAtPoint(audioSrc.clip,position) }
            };
        audioSource.clip = audioClips[(int)Random.Range(0, audioClips.Count)];
        if (audioSource.clip != null)
        {
            playMethodDict[method].Invoke(audioSource);
            Destroy(audioSource.gameObject, audioSource.clip.length);
        }
        else
        {
            Destroy(audioSource.gameObject);
        }
        return audioSource;
    }

    public IEnumerator SoundFade(float time)
    {
        if (audioSource == null) yield break;
        var startVol = audioSource.volume;
        for (float currentVol = startVol; currentVol > 0; currentVol -= 0.01f)
        {
            audioSource.volume = currentVol;
            yield return new WaitForSeconds(time/100);
        }
        audioSource.enabled = false;
    }

    //public AudioSource Play(AudioSource audioSourceParam = null)
    //{
    //    var src = audioSourceParam;
    //    if (src == null)
    //    {
    //        var _obj = new GameObject("Sound", typeof(AudioSource));
    //        src = _obj.GetComponent<AudioSource>();
    //        src.gameObject.transform.position = position;
    //        src.loop = isLooped;
    //        src.volume = vol * masterVol;
    //        src.minDistance = minDistance;
    //        src.maxDistance = maxDistance;
    //        src.rolloffMode = rolloffMode;
    //        src.spatialBlend = is3D ? value3D : value2D;
    //    }
    //    var clip = audioClips[(int)Random.Range(0, audioClips.Count)];
    //    src.clip = clip;
    //    playMethodDict =
    //    new Dictionary<playMethod, System.Action<AudioSource>>
    //    {
    //        {playMethod.Play, audioSrc=>audioSrc.Play() },
    //        {playMethod.PlayOneShot, audioSrc=>audioSrc.PlayOneShot(clip) },
    //        {playMethod.PlayDelayed, audioSrc=>audioSrc.PlayDelayed(delay) },
    //        {playMethod.PlayClipAtPoint, audioSrc => AudioSource.PlayClipAtPoint(clip,position) }
    //    };
    //    playMethodDict[method].Invoke(src);
    //    Destroy(src.gameObject, src.clip.length);
    //    return src;
    //}
}
