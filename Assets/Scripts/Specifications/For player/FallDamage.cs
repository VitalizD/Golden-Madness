using UnityEngine;

public class FallDamage : MonoBehaviour
{
    [SerializeField] private float velocityForDamage;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private float repulsiveForceMultiplier = 1.3f;

    private Rigidbody2D rb;

    private LayerMask groundMask;

    private void Awake()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
        groundMask = LayerMask.NameToLayer(ServiceInfo.GroundLayerName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == groundMask && rb.velocity.y < -velocityForDamage)
        {
            var damage = -rb.velocity.y - velocityForDamage;
            var player = transform.parent.GetComponent<Player>();
            if (player)
            {
                rb.AddForce(transform.up * (-rb.velocity.y) * repulsiveForceMultiplier, ForceMode2D.Impulse);
                player.Health -= (int)(damage * damageMultiplier);
            }
        }
    }
}
