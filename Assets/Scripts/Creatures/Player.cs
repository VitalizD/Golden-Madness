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
    [SerializeField] private float touchingDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float stunTime = 0.5f;
    [SerializeField] private float attackTime = 0.3f;
    [SerializeField] private float reloadAttackTime = 0.5f;
    [SerializeField] private VectorValue initialPosition;

    [SerializeField] private UnityEvent<string> OnGetDamage;

    private Rigidbody2D rigidBody2d;
    private SpriteRenderer sprite;
    private Animator animator;

    private readonly float jumpCheckRadius = 0.01f;
    private readonly float yOffsetToGround = -0.5f;
    private Tile selectedTile;
    private Vector3 oldPos;
    private LayerMask groundMask;
    private LayerMask enemiesMask;
    private Vector2 checkpoint;

    private Coroutine reloadAttack;

    private bool invulnerability = false;
    private bool isMoving = false;
    private bool isStunned = false;
    private bool feelPain = false;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool isDigging = false;
    private bool isGrounded = false;

    public bool IsDigging { get => isDigging; set => isDigging = value; }

    public Vector2 Checkpoint { get => checkpoint; set => checkpoint = value; }

    public bool IsGrounded { get => isGrounded; }

    public float TouchingDistance { get => touchingDistance; }

    public int HitDamage { get => hitDamage; }

    public States State
    {
        get => (States)animator.GetInteger("State");
        set
        {
            if (feelPain)
                return;

            animator.SetInteger("State", (int)value);

            if (value == States.Pain)
            {
                feelPain = true;
                StartCoroutine(DisablePainAnimation());
            }
        }
    }

    public int Health
    {
        get => health;
        set
        {
            if (value < health)
            {
                State = States.Pain;
                invulnerability = true;
                StartCoroutine(DisableInvulnerability());
            }
            health = value;
            //OnGetDamage?.Invoke(health.ToString());
            if (health <= 0)
            {
                health = 100;
                transform.position = checkpoint;
            }
        }
    }

    public void SetSelectedTile(Tile value) => selectedTile = value;

    public void RemoveSelectedTile() => selectedTile = null;

    public void AddForce(Vector2 force) => rigidBody2d.AddForce(force, ForceMode2D.Impulse);

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else if (instanse == this)
            Destroy(gameObject);

        rigidBody2d = GetComponent<Rigidbody2D>();
        sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        groundMask = LayerMask.GetMask(ServiceInfo.GroundLayerName);
        enemiesMask = LayerMask.GetMask(ServiceInfo.EnemiesLayerName);
    }

    private void Start()
    {
        if (initialPosition != null)
            transform.position = initialPosition.initialValue;
        checkpoint = transform.position;
    }

    private void Update()
    {
        if (isGrounded && !isStunned && Input.GetButtonDown("Jump"))
            Jump();

        if (canAttack && isGrounded && !isDigging && !isStunned && Input.GetButtonDown("Fire1"))
            Attack();
    }

    private void FixedUpdate()
    {
        isMoving = transform.position != oldPos;
        oldPos = transform.position;

        CheckGrounded();

        if (isGrounded && !isAttacking && !isStunned && !isDigging)
            State = States.Idle;

        if (!isStunned && Input.GetButton("Horizontal"))
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
        //var attackPoint = new Vector2(transform.position.x + (sprite.flipX ? -attackDistance : attackDistance), transform.position.y);
        var raycastHits = Physics2D.RaycastAll(transform.position, transform.right * (sprite.flipX ? -1 : 1), attackDistance, enemiesMask);

        foreach (var raycastHit in raycastHits)
        {
            var creature = raycastHit.collider.GetComponent<Creature>();
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
            SetStun();
        }
    }

    private void SetStun()
    {
        State = States.Pain;
        isStunned = true;
        StartCoroutine(DisableStun());
    }

    private void Throw(Collision2D collision)
    {
        var repulsion = collision.gameObject.GetComponent<Repulsive>();
        if (repulsion)
        {
            var colPos = collision.transform.position;
            var playerPos = transform.position;

            rigidBody2d.AddForce(new Vector2((playerPos.x - colPos.x) * repulsion.Force, repulsion.Force / 5), ForceMode2D.Impulse);
        }
    }

    private void Run()
    {
        isDigging = false;
        if (isGrounded && !isAttacking)
            State = States.Walk;

        var dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0;
    }

    private void Jump()
    {
        isDigging = false;
        rigidBody2d.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Attack()
    {
        //ChangeDirectionTowards(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        State = States.Attack;
        canAttack = false;
        isAttacking = true;
        rigidBody2d.AddForce(transform.right * (sprite.flipX ? -jerkForce : jerkForce), ForceMode2D.Impulse);

        StartCoroutine(FinishAttack());
        reloadAttack = StartCoroutine(ReloadAttack());
    }

    private void CheckGrounded()
    {
        if (!isGrounded)
        {
            if (rigidBody2d.velocity.y > 0)
                State = States.Jump;
            else if (rigidBody2d.velocity.y < 0)
                State = States.Fall;
        }

        var collaiders = Physics2D.OverlapCircleAll(
            new Vector2(transform.position.x, transform.position.y + yOffsetToGround), jumpCheckRadius, groundMask);
        isGrounded = collaiders.Length > 0;
    }

    private void ChangeDirectionTowards(Vector2 position)
    {
        sprite.flipX = position.x <= transform.position.x;
    }

    private IEnumerator DisableInvulnerability()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        invulnerability = false;
    }

    private IEnumerator DisableStun()
    {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        feelPain = false;
    }

    private IEnumerator DisablePainAnimation()
    {
        yield return new WaitForSeconds(stunTime);
        feelPain = false;
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