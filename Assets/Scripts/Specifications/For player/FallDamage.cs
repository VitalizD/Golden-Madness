using UnityEngine;

public class FallDamage : MonoBehaviour
{
    [SerializeField] private float velocityForDamage;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private float repulsiveForceMultiplier = 1.3f;

    private Rigidbody2D rb;
    private Player player;

    private LayerMask groundMask;

    private void Awake()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
        groundMask = LayerMask.NameToLayer(ServiceInfo.GroundLayerName);
        player = transform.parent.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == groundMask && rb.velocity.y < -velocityForDamage)
        {
            var damage = -rb.velocity.y - velocityForDamage;
            rb.AddForce(-rb.velocity.y * repulsiveForceMultiplier * transform.up, ForceMode2D.Impulse);
            player.Health -= (int)(damage * damageMultiplier);
        }
    }
}
