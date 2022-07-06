using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySoundObjectAfterDonePlaying : MonoBehaviour
{
    private AudioSource source;
    private float timer = 0f;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (timer >= source.clip.length)
        {
            Destroy(gameObject);
        }
        timer += Time.deltaTime;
    }
}
