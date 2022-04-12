using System;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float speed;
    [SerializeField] private States[] states;
    private Animator animator;

    public int Health
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0)
                Destroy();
        }
    }

    public float Speed { get => speed; set => speed = value >= 0 ? value : speed; }

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
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
