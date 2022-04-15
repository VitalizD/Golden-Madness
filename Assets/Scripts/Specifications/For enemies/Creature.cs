using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Creature : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float speed;
    [SerializeField] private float damageColorTime = 0.25f;
    [SerializeField] private Color damageColor = Color.white;
    [SerializeField] private bool repulsiable;
    [SerializeField] private States[] states;

    private UnityEvent OnGetDamage;

    private Color normalColor;
    private bool attacked = false;

    private Animator animator;
    private Rigidbody2D rigidbody2D_;
    private SpriteRenderer sprite;

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
                OnGetDamage?.Invoke();
                attacked = true;
                ChangeDirectionTowards(Player.instanse.transform.position);
                sprite.color = damageColor;
                StartCoroutine(SetNormalColor());
            }
        }
    }

    public float Speed { get => speed; set => speed = value >= 0 ? value : speed; }

    public bool Repulsiable { get => repulsiable; }

    public bool Attacked { get => attacked; }

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
        sprite = GetComponent<SpriteRenderer>();
        normalColor = sprite.color;
    }

    public void Throw(Vector2 startPoint, float force)
    {
        var position = transform.position;
        rigidbody2D_.AddForce(new Vector2(position.x - startPoint.x, 1.5f) * force, ForceMode2D.Impulse);
    }

    public void ChangeDirection()
    {
        sprite.flipX = !sprite.flipX;
    }

    public void ChangeDirectionTowards(Vector2 position)
    {
        sprite.flipX = position.x <= transform.position.x;
    }

    private IEnumerator SetNormalColor()
    {
        yield return new WaitForSeconds(damageColorTime);
        sprite.color = normalColor;
        attacked = false;
    }
}
