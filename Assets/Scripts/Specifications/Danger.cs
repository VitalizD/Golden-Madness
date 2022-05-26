using UnityEngine;

public class Danger : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    public int Damage { get => damage; }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    var player = collision.GetComponent<Player>();
    //    if (player)
    //        player.Health -= damage;
    //}
}
