using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private bool enable = true;
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float frequency;
    [SerializeField] private SFX torchSFX;
    private AudioSource torchAudioSource;

    private Light light_;

    private void Awake()
    {
        Debug.Log(torchSFX);
        Debug.Log(gameObject);
        light_ = GetComponent<Light>();
        torchSFX.Position = gameObject.transform.position;
        torchAudioSource=torchSFX.Play();

    }

    private void Update()
    {
        SoundSetting.Instanse.GameVolume.onValueChanged.AddListener(value => torchAudioSource.volume = value);
        if (enable)
            light_.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(10, Time.time / frequency));
    }
}
