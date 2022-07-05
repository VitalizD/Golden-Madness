using UnityEngine;

public class FallDamage : MonoBehaviour
{
    [SerializeField] private float velocityForDamage;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private float repulsiveForceMultiplier = 1.3f;
    [SerializeField] private Rigidbody2D rigidBody;

    private LayerMask groundMask;

    private void Awake()
    {
        groundMask = LayerMask.NameToLayer(ServiceInfo.GroundLayerName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == groundMask && rigidBody.velocity.y < -velocityForDamage)
        {
            var damage = -rigidBody.velocity.y - velocityForDamage;
            rigidBody.AddForce(-rigidBody.velocity.y * repulsiveForceMultiplier * transform.up, ForceMode2D.Impulse);
            Player.Instanse.Health -= (int)(damage * damageMultiplier);
        }
    }
}
