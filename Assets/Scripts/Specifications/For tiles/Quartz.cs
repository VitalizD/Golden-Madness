using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Quartz : MonoBehaviour
{
    private Vector3 initialPosition;
    private readonly float distance = 1f;
    private readonly float distanceToMove = 0.25f;
    private readonly Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);

    public Vector3 InitialPosition { get => initialPosition; set => initialPosition = value; }

    void Awake()
    {
        transform.localScale -= scale;
        InitialPosition = transform.position;
        var colliders = new List<Collider2D>
        {
            Physics2D.OverlapPoint(new Vector2(transform.position.x + distance, transform.position.y)),
            Physics2D.OverlapPoint(new Vector2(transform.position.x - distance, transform.position.y)),
            Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y + distance)),
            Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y - distance))
        }
        .Where(collaider => collaider && collaider.gameObject.layer == LayerMask.NameToLayer(ServiceInfo.GroundLayerName) &&
                !(collaider.gameObject.GetComponent<AttachedTile>()) &&
                collaider.gameObject.GetComponent<Tile>().ResourceType == ResourceTypes.None)
        .ToList();

        //если рядом нет блоков, то удали кварц
        if (colliders.Count == 0)
        {
            Destroy(gameObject);
        }

        else {
            var random = new System.Random();
            /*for (int i = colliders.Count - 1; i >= 1; i--)
            {
                int j = random.Next(i + 1);
                var temp = colliders[j];
                colliders[j] = colliders[i];
                colliders[i] = temp;
            }*/
            /*foreach (var collaider in colliders) {
                Debug.Log(collaider);
            }*/

            transform.position = Vector3.MoveTowards(
                transform.position,
                colliders[random.Next(colliders.Count)].transform.position,
                distanceToMove);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
