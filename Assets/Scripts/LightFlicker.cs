using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private bool enable = true;
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float frequency;
    [SerializeField] private SFX torchSFX;

    private Light light_;

    private void Awake()
    {
        light_ = GetComponent<Light>();
        torchSFX.Position = gameObject.transform.position;
        torchSFX.Play();

    }

    private void Update()
    {
        if (enable)
            light_.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(10, Time.time / frequency));
    }
}
