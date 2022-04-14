using System;
using UnityEngine;
using UnityEngine.Events;

public class Creature : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float speed;
    [SerializeField] private bool repulsiable;
    [SerializeField] private States[] states;

    private Animator animator;
    private Rigidbody2D rigidbody2D_;

    public int Health
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0)
                Destroy(gameObject);
            else
            {
                var rat = GetComponent<Rat>();
                if (rat)
                    rat.ChangeDirectionTowards(Player.instanse.transform.position);
            }
        }
    }

    public float Speed { get => speed; set => speed = value >= 0 ? value : speed; }

    public bool Repulsiable { get => repulsiable; }

    public States State
    {
        get => (States)animator.GetInteger("State");
        set
        {
            if (Array.Exists(states, v => v == value))
                animator.SetInteger("State", (int)value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody2D_ = GetComponent<Rigidbody2D>();
    }

    public void Throw(Vector2 startPoint, float force)
    {
        var position = transform.position;
        rigidbody2D_.AddForce(new Vector2(position.x - startPoint.x, 1.5f) * force, ForceMode2D.Impulse);
    }
}
