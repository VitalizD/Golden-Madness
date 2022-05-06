using UnityEngine;
using System.Collections;

public class SanityController : MonoBehaviour
{
    [SerializeField] private bool enableDecreasing = true;
    [SerializeField] [Range(0, 100)] private float sanity = 100f;

    private readonly float decreasingBetweenTime = 1f;
    private Coroutine decreaseSanity;

    public float Sanity 
    { 
        get => sanity; 
        set
        {
            if (value < 0) sanity = 0;
            else if (value > 100) sanity = 100f;
            else sanity = value;
        }
    }

    public float DecreasingSanity { get; set; } = 0;

    public bool DecreasingEnabled { get => enableDecreasing; set => enableDecreasing = value; }

    private void Start()
    {
        decreaseSanity = StartCoroutine(DecreaseSanity());
    }

    private IEnumerator DecreaseSanity()
    {
        while (true)
        {
            yield return new WaitForSeconds(decreasingBetweenTime);
            if (enableDecreasing)
                sanity -= DecreasingSanity;
        }
    }
}
