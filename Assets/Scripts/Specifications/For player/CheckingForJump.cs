using UnityEngine;
using System.Collections;

public class CheckingForJump : MonoBehaviour
{
    private LayerMask groundMask;
    private LayerMask enemiesMask;
    private BoxCollider2D boxCollider2D;
    private readonly Collider2D[] resultColliders = new Collider2D[10];

    private bool canJump = false;
    private bool jumped = false;

    public bool CanJump { get => canJump; }

    public IEnumerator Jump()
    {
        jumped = true;
        yield return new WaitForSeconds(0.1f);
        jumped = false;
    }

    private void Awake()
    {
        groundMask = LayerMask.NameToLayer(ServiceInfo.GroundLayerName);
        enemiesMask = LayerMask.NameToLayer(ServiceInfo.EnemiesLayerName);
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (Player.Instanse.IsClimbing)
        {
            canJump = true;
            return;
        }
        else if (jumped)
        {
            canJump = false;
            return;
        }

        var collisionsCount = boxCollider2D.OverlapCollider(new ContactFilter2D(), resultColliders);
        var finded = false;
        for (var i = 0; i < collisionsCount; ++i)
        {
            if (resultColliders[i] != null && (resultColliders[i].gameObject.layer == groundMask || resultColliders[i].gameObject.layer == enemiesMask))
            {
                canJump = true;
                finded = true;
                break;
            }
        }
        if (!finded)
            canJump = false;
    }
}
