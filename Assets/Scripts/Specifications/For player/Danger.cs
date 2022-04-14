using UnityEngine;

public class Danger : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    public int Damage { get => damage; }
}
