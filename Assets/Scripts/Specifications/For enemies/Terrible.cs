using UnityEngine;

public class Terrible : MonoBehaviour
{
    [SerializeField] private float radiusInfluence;
    [SerializeField] private float decreasingSanityInArea;
    [SerializeField] private float decreasingSanityAfterAttack;

    private LayerMask playerMask;
    private SanityController sanity;

    private bool playerInArea = false;

    public float DecreasingSanityAfterAttack { get => decreasingSanityAfterAttack; }

    public void Cancel()
    {
        if (playerInArea)
        {
            if (Player.Instanse != null)
                Player.Instanse.GetComponent<SanityController>().DecreasingSanity -= decreasingSanityInArea;
            playerInArea = false;
        }
    }

    private void Awake()
    {
        playerMask = LayerMask.GetMask(ServiceInfo.PlayerLayerName);
    }

    private void Start()
    {
        sanity = Player.Instanse.GetComponent<SanityController>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radiusInfluence);
    }

    private void FixedUpdate()
    {
        var collider = Physics2D.OverlapCircle(transform.position, radiusInfluence, playerMask);
        if (!playerInArea && collider)
        {
                playerInArea = true;
                sanity.DecreasingSanity += decreasingSanityInArea;
        }
        else if (playerInArea && !collider)
        {
            playerInArea = false;
            sanity.DecreasingSanity -= decreasingSanityInArea;
        }
    }
}
