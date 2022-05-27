using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SanityController : MonoBehaviour
{
    [SerializeField] private bool enableDecreasing = true;
    [SerializeField] [Range(0, 100)] private float sanity = 100f;

    private readonly float decreasingBetweenTime = 1f;
    private Coroutine decreaseSanity;
    private Consumables consumables;

    public SanityBar sanityBar;

    public Text smockingPipes;

    public float Sanity 
    { 
        get => sanity; 
        set
        {
            if (value < 0)
            {
                sanity = 0;
                sanityBar.SetSanity(0);
            }
            else if (value > 100)
            {
                sanity = 100f;
                sanityBar.SetSanity(100f);
            }
            else
            { 
                sanity = value;
                sanityBar.SetSanity(value);
            }
        }
    }

    public float DecreasingSanity { get; set; } = 0;

    public bool DecreasingEnabled { get => enableDecreasing; set => enableDecreasing = value; }

    private void Awake()
    {
        consumables = GetComponent<Consumables>();
    }
    private void Start()
    {
        decreaseSanity = StartCoroutine(DecreaseSanity());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4) && consumables.SmokingPipesCount > 0)
        {
            Sanity += Consumables.SmokingPipesRecovery;
            sanityBar.SetSanity(Sanity);
            --consumables.SmokingPipesCount;
            smockingPipes.text = "" + consumables.SmokingPipesCount;
        }
    }
    private IEnumerator DecreaseSanity()
    {
        while (true)
        {
            yield return new WaitForSeconds(decreasingBetweenTime);
            if (enableDecreasing)
            {
                sanity -= DecreasingSanity;
                sanityBar.SetSanity(sanity);
            }
        }
    }
}
