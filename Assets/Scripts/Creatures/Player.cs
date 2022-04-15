using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Player : MonoBehaviour
{
    public static Player instanse = null;
    
    [SerializeField] private int health = 10;
    [SerializeField] private int hitDamage;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float repulsiveForce;
    [SerializeField] private float jerkForce = 3f;
    [SerializeField] private float jumpCheckRadius = 0.45f;
    [SerializeField] private float touchingDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float stunTime = 0.5f;
    [SerializeField] private float attackTime = 0.3f;
    [SerializeField] private float reloadAttackTime = 1f;
    [SerializeField] private float yOffsetToGround = -0.5f;

    [SerializeField] private UnityEvent<string> OnGetDamage;

    private Rigidbody2D rigidBody2d;
    private SpriteRenderer sprite;
    private Animator animator;
    private Tile selectedTile;
    private Vector3 oldPos;

    private LayerMask groundMask;
    private LayerMask enemiesMask;

    private Coroutine reloadAttack;

    private bool invulnerability = false;
    private bool isMoving = false;
    private bool isStunned = false;
    private bool isAttacking = false;
    private bool canAttack = true;

    public bool IsDigging { get; set; } = false;

    public bool IsGrounded { get; private set; } = false;

    public float TouchingDistance { get => touchingDistance; }

    public int HitDamage { get => hitDamage; }

    public States State
    {
        get => (States)animator.GetInteger("State");
        set
        {
            if (isStunned)
                return;
            animator.SetInteger("State", (int)value);
        }
    }

    public int Health
    {
        get => health;
        set
        {
            health = value;
            //OnGetDamage?.Invoke(health.ToString());
            if (health <= 0)
                Destroy(gameObject);
        }
    }

    private bool IsStunned
    {
        get => isStunned;
        set
        {
            if (value) State = States.Pain;
            isStunned = value;
            if (!value) State = States.Idle;
        }
    }

    public void SetSelectedTile(Tile value) => selectedTile = value;

    public void RemoveSelectedTile() => selectedTile = null;

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else if (instanse == this)
            Destroy(gameObject);

        rigidBody2d = GetComponent<Rigidbody2D>();
        sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        groundMask = LayerMask.GetMask("Ground");
        enemiesMask = LayerMask.GetMask("Enemies");
    }

    private void Update()
    {
        if (IsGrounded && !IsStunned && Input.GetButtonDown("Jump"))
            Jump();

        if (canAttack && IsGrounded && !IsDigging && !IsStunned && Input.GetButtonDown("Fire1"))
            Attack();
    }

    private void FixedUpdate()
    {
        isMoving = transform.position != oldPos;
        oldPos = transform.position;

        CheckGrounded();

        if (IsGrounded && !isAttacking && !IsStunned && !IsDigging)
            State = States.Idle;

        if (!IsStunned && !isAttacking && Input.GetButton("Horizontal"))
            Run();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (invulnerability)
            return;

        Throw(collision);
        GetDamage(collision);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    // Назначен на ключ в анимации "Attack"
    private void OnAttack()
    {
        var attackPoint = new Vector2(transform.position.x + (sprite.flipX ? -attackDistance : attackDistance), transform.position.y);
        var colliders = Physics2D.OverlapCircleAll(attackPoint, attackDistance, enemiesMask);

        foreach (var collider in colliders)
        {
            var creature = collider.GetComponent<Creature>();
            if (creature)
            {
                creature.Health -= hitDamage;
                if (creature.Repulsiable)
                    creature.Throw(transform.position, repulsiveForce);
            }
        }
    }

    // Назначен на ключ в анимации "Dig"
    private void HitTile()
    {
        if (!selectedTile)
            return;

        var damage = selectedTile.DiggingDifficulty < HitDamage ? HitDamage / selectedTile.DiggingDifficulty : HitDamage;
        selectedTile.Health -= damage;
        canAttack = false;
        if (reloadAttack != null) StopCoroutine(reloadAttack);
        reloadAttack = StartCoroutine(ReloadAttack());
    }

    private void GetDamage(Collision2D collision)
    {
        var danger = collision.gameObject.GetComponent<Danger>();
        if (danger)
        {
            Health -= danger.Damage;
            IsStunned = true;
            invulnerability = true;
            StartCoroutine(DisableStun());
            StartCoroutine(DisableInvulnerability());
        }
    }

    private void Throw(Collision2D collision)
    {
        var repulsion = collision.gameObject.GetComponent<Repulsive>();
        if (repulsion)
        {
            var colPos = collision.transform.position;
            var playerPos = transform.position;

            rigidBody2d.AddForce(new Vector2(playerPos.x - colPos.x, 1) * repulsion.Force, ForceMode2D.Impulse);
        }
    }

    private void Run()
    {
        IsDigging = false;
        if (IsGrounded)
            State = States.Walk;

        var dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0;
    }

    private void Jump()
    {
        IsDigging = false;
        rigidBody2d.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Attack()
    {
        State = States.Attack;
        canAttack = false;
        isAttacking = true;
        rigidBody2d.AddForce(transform.right * (sprite.flipX ? -jerkForce : jerkForce), ForceMode2D.Impulse);

        StartCoroutine(FinishAttack());
        reloadAttack = StartCoroutine(ReloadAttack());
    }

    private void CheckGrounded()
    {
        if (!IsGrounded)
        {
            if (rigidBody2d.velocity.y > 0)
                State = States.Jump;
            else if (rigidBody2d.velocity.y < 0)
                State = States.Fall;
        }

        var collaiders = Physics2D.OverlapCircleAll(
            new Vector2(transform.position.x, transform.position.y + yOffsetToGround), jumpCheckRadius, groundMask);
        IsGrounded = collaiders.Length > 0;
    }

    private IEnumerator DisableInvulnerability()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        invulnerability = false;
    }

    private IEnumerator DisableStun()
    {
        yield return new WaitForSeconds(stunTime);
        IsStunned = false;
    }

    private IEnumerator FinishAttack()
    {
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
    }

    private IEnumerator ReloadAttack()
    {
        yield return new WaitForSeconds(reloadAttackTime);
        canAttack = true;
    }
}