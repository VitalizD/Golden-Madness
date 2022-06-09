using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class floorDoorToSZ : MonoBehaviour
{
    void Awake()
    {
        var colliders = new List<Collider2D>
        {
            Physics2D.OverlapPoint(new Vector2(transform.position.x - 1 , transform.position.y - 1.25f)),
            Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y - 1.25f)),
            Physics2D.OverlapPoint(new Vector2(transform.position.x + 1, transform.position.y - 1.25f)),
        }
        .Where(collaider => collaider && collaider.gameObject.layer == LayerMask.NameToLayer(ServiceInfo.GroundLayerName) &&
                !(collaider.gameObject.GetComponent<AttachedTile>()) )
        .ToList();
        foreach (var collider in colliders) 
        {
            var tile = collider.gameObject.GetComponent<Tile>();
            tile.IsBedrock = true;
        }

    }

   /* void OnCollisionEnter2D(Collision2D collaider)
    {
        if (collaider.gameObject.layer == LayerMask.NameToLayer(ServiceInfo.GroundLayerName)&& !(collaider.gameObject.GetComponent<AttachedTile>()))
        {
            var tile = collaider.gameObject.GetComponent<Tile>();
            tile.IsBedrock = true;
        }
    }*/
}
