using UnityEngine;
using System.Collections.Generic;

public class Rope : MonoBehaviour
{
    [Tooltip("Максимальная дистанция вверх, на которую способен подняться канат, в тайлах")]
    [SerializeField] private int maxDistance = 5;
    [SerializeField] private GameObject hook;

    private Consumables consumables;

    private void Awake()
    {
        consumables = GetComponent<Consumables>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5) && consumables.GetCount(ConsumableType.Rope) > 0)
        {
            consumables.Add(ConsumableType.Rope, -1);
            var hook = Instantiate(this.hook, Tile.GetCenterPositionOfNearestTile(transform.position), Quaternion.identity).transform;
            hook.GetComponent<Hook>().BuildRope(maxDistance);
        }
    }
}
