using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    private float masterVol = 1f;

    [Range(0, 1)] [SerializeField] private float vol=1f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private playMethod method;
    [SerializeField] private bool isLooped;
    [SerializeField] private bool is3D;
    [SerializeField] private float maxDistance=20f;
    [SerializeField] private Vector3 position = new Vector3(0,0,0);
    [SerializeField] private float delay;
    [SerializeField] private AudioRolloffMode rolloffMode=AudioRolloffMode.Linear;
    [SerializeField] private List<AudioClip> audioClips;

    public float MasterVol { get => masterVol; set => masterVol = value; }
    public Vector3 Position { get => position; set => position = value; }

    public AudioSource Play(AudioSource audioSourceParam = null) 
    {
        var src = audioSourceParam;
        if (src == null)
        {
            var _obj = new GameObject("Sound", typeof(AudioSource));
            src = _obj.GetComponent<AudioSource>();
            src.gameObject.transform.position = position;
            src.loop = isLooped;
            src.volume = vol*masterVol;
            src.maxDistance = maxDistance;
            src.rolloffMode = rolloffMode;
            src.spatialBlend = is3D ? value3D : value2D;
        }
        var clip = audioClips[(int)Random.Range(0, audioClips.Count)];
        src.clip = clip;
        playMethodDict =
        new Dictionary<playMethod, System.Action<AudioSource>>
        {
            {playMethod.Play, audioSrc=>audioSrc.Play() },
            {playMethod.PlayOneShot, audioSrc=>audioSrc.PlayOneShot(clip) },
            {playMethod.PlayDelayed, audioSrc=>audioSrc.PlayDelayed(delay) },
            {playMethod.PlayClipAtPoint, audioSrc => AudioSource.PlayClipAtPoint(clip,position) }
        };
        playMethodDict[method].Invoke(src);
        Destroy(src.gameObject, src.clip.length);
        Debug.Log(position);
        return src;
    }
}
