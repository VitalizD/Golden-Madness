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
    [SerializeField] private bool stalagmitesCanThrowAway=false;
    private float repulsiveForce = 3f;

    private readonly float delayInSeconds = 1f;
    private float timer;

    private Color normalColor;
    private bool attacked = false;
    private int directionValue = 1; // 1 - положительный X (право);   -1 - отрицательный X (лево)

    private Animator animator;
    private Rigidbody2D rigidbody2D_;
    private SpriteRenderer sprite;
    private ICreature child;

    public float Speed { get => speed; set => speed = value >= 0 ? value : speed; }

    public bool Repulsiable { get => repulsiable; }

    public bool Attacked { get => attacked; }

    public int DirectionValue { get => directionValue; }

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
                attacked = true;
                ChangeDirectionTowards(Player.Instanse.transform.position);
                sprite.color = damageColor;
                child?.ReactToAttack();
                StartCoroutine(SetNormalColor());
            }
        }
    }

    public States State
    {
        get => (States)animator.GetInteger("State");
        set
        {
            if (Array.Exists(states, v => v == value))
                animator.SetInteger("State", (int)value);
        }
    }

    public void SetChild(ICreature value) => child = value;

    private void Awake()
    {
        timer = delayInSeconds;
        animator = GetComponent<Animator>();
        rigidbody2D_ = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        normalColor = sprite.color;
    }

    private void OnDestroy()
    {
        var terrible = GetComponent<Terrible>();
        if (terrible != null)
            terrible.Cancel();
    }

    public void Throw(Vector2 startPoint, float force)
    {
        var position = transform.position;
        rigidbody2D_.AddForce(new Vector2(position.x - startPoint.x, 1.5f) * force, ForceMode2D.Impulse);
    }

    public void ChangeDirection()
    {
        sprite.flipX = !sprite.flipX;
        directionValue = sprite.flipX ? -1 : 1;
    }

    public void ChangeDirectionTowards(Vector2 position)
    {
        sprite.flipX = position.x <= transform.position.x;
        directionValue = sprite.flipX ? -1 : 1;
    }

    public void AddForce(Vector2 force) => rigidbody2D_.AddForce(force, ForceMode2D.Impulse);

    private IEnumerator SetNormalColor()
    {
        yield return new WaitForSeconds(damageColorTime);
        sprite.color = normalColor;
        attacked = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
      
        var danger = collision.collider.GetComponent<Danger>();
        if (danger) {
            timer += Time.deltaTime;
            if (timer >= delayInSeconds) 
            {
                Debug.Log(timer);
                GetDamage(danger.Damage);
                if (stalagmitesCanThrowAway) this.Throw(transform.position, repulsiveForce);
                timer = 0f;
            }
        } 
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var danger = collision.collider.GetComponent<Danger>();
        if (danger)
            timer = delayInSeconds;
    }

    private void GetDamage(int damage)
    {
        this.Health -= damage;
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSecondsRealtime(time);
    }
}
