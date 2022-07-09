using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private bool enable = true;
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float frequency;
    

    private Light light_;

    private void Awake()
    {
        light_ = GetComponent<Light>();
    }

    private void Update()
    {
        if (enable)
            light_.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(10, Time.time / frequency));
    }
}
