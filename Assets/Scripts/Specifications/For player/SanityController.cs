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


    public float Sanity 
    { 
        get => sanity; 
        set
        {
            if (value < 0) sanity = 0;
            else if (value > 100) sanity = 100f;
            else sanity = value;

            if (HotbarController.Instanse != null)
                HotbarController.Instanse.SetBarValue(BarType.Sanity, sanity);
        }
    }

    public float DecreasingSanity { get; set; } = 0;

    public bool DecreasingEnabled { get => enableDecreasing; set => enableDecreasing = value; }

    public void Save()
    {
        PlayerPrefs.SetFloat(PlayerPrefsKeys.Sanity, sanity);
    }

    public void Load()
    {
        Sanity = PlayerPrefs.GetFloat(PlayerPrefsKeys.Sanity, sanity);
    }

    private void Awake()
    {
        consumables = GetComponent<Consumables>();
    }

    private void Start()
    {
        decreaseSanity = StartCoroutine(DecreaseSanity());
        Sanity = sanity;
    }

    private void Update()
    {
        if (Paused.Instanse != null && Paused.Instanse.IsPaused)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha4) && consumables.GetCount(ConsumableType.SmokingPipe) > 0)
        {
            Sanity += consumables.GetRecovery(ConsumableType.SmokingPipe);
            consumables.Add(ConsumableType.SmokingPipe, -1);
        }
    }

    private IEnumerator DecreaseSanity()
    {
        while (true)
        {
            yield return new WaitForSeconds(decreasingBetweenTime);
            if (enableDecreasing)
                Sanity -= DecreasingSanity;
        }
    }
}
