using UnityEngine;

public class Repulsive : MonoBehaviour
{
    [SerializeField] private float forceX = 5f;
    [SerializeField] private float forceY = 5f;

    public float ForceX { get => forceX; }

    public float ForceY { get => forceY; }
}
