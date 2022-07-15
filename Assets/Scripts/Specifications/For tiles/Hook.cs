using UnityEngine;
using System.Collections.Generic;

public class Hook : MonoBehaviour
{
    private const float ropePartZPosition = 0.5f;
    private const float checkingRadius = 0.1f;

    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private GameObject ropePart;

    private readonly List<GameObject> ropeParts = new List<GameObject>();
    private Vector3 endPosition;
    private bool isFlying = false;

    public void BuildRope(int maxDistance)
    {
        for (var y = transform.position.y; y < transform.position.y + maxDistance; y += Tile.Size)
        {
            var checkpoint = new Vector3(transform.position.x, y, transform.position.z + ropePartZPosition);
            if (Physics2D.OverlapCircleAll(checkpoint, checkingRadius, groundMask).Length == 0)
            {
                var ropePart = Instantiate(this.ropePart, checkpoint, Quaternion.identity);
                ropeParts.Add(ropePart);
                ropePart.SetActive(false);
                if (y >= transform.position.y + maxDistance - Tile.Size)
                {
                    endPosition = checkpoint;
                    break;
                }
            }
            else
            {
                endPosition = new Vector3(checkpoint.x, checkpoint.y - Tile.Size, checkpoint.z);
                break;
            }
        }
        isFlying = true;
    }

    private void Update()
    {
        if (isFlying && endPosition != Vector3.zero)
        {
            transform.position = Vector3.Lerp(transform.position, endPosition, speed * Time.deltaTime);
            if (Mathf.Abs(transform.position.y - endPosition.y) < 0.02f)
            {
                isFlying = false;
                for (var i = 0; i < ropeParts.Count - 1; ++i)
                {
                    ropeParts[i].SetActive(true);
                    ropeParts[i].transform.parent = transform;
                }
            }
        }
    }
}
