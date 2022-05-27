using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instanse = null;

    [Header("Base")]
    [SerializeField] [Range(0, 100)] private int health = 100;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 5f;
    public HealthBar healthBar;
    public SanityBar sanityBar;
    public Text healthPacks;

    [Header("Fight")]
    [SerializeField] private int enemyDamage = 10;
    [SerializeField] private int maxEnemyDamage = 10;
    [SerializeField] private int minEnemyDamageInPercents = 60;
    [SerializeField] private float repulsiveForce;
    [SerializeField] private float jerkForce = 3f;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackTime = 0.3f;
    [SerializeField] private float reloadAttackTime = 0.5f;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float stunTime = 0.5f;
    [SerializeField] private RedFilter displayFilter;
    [SerializeField] private UnityEvent<string> OnChangeHealth;

    [Header("Pickaxe")]
    [SerializeField] [Range(0, 100f)] private float pickaxeStrength = 100f;
    [SerializeField] private float hitDamageToPickaxe = 1f;
    [SerializeField] private float tileDamage = 1f;
    [SerializeField] private float maxTileDamage = 1f;
    [SerializeField] [Range(0, 100f)] private float minTileDamageInPercents = 20;
    [SerializeField] private float touchingDistance;
    public PickaxeStrengthBar pickaxeStrengthBar;
    public Text grindstones;

    [Header("Sleeping Bag")]
    [SerializeField] [Range(0, 100)] private int healthRecovery = 20;
    [SerializeField] [Range(0, 100f)] private float sanityRecovery = 50f;

    private Rigidbody2D rigidBody2d;
    private SpriteRenderer sprite;
    private Animator animator;
    private SanityController sanity;
    private Consumables consumables;

    private Tile selectedTile;
    private LayerMask groundMask;
    private LayerMask enemiesMask;
    private Vector2 checkpoint;
    private float fixedZPosition;

    private Coroutine reloadAttack;

    private readonly float jumpCheckRadius = 0.01f;
    private readonly float yOffsetToGround = -0.5f;

    private bool invulnerability = false;
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
                if (displayFilter) displayFilter.ChangeColor(health - value);
            }
            health = value > 100 ? 100 : value;
            healthBar.SetHealth(health);
            if (health <= 0)
            {
                health = 100;
                healthBar.SetHealth(health);
                transform.position = checkpoint;
                if (displayFilter) displayFilter.RemoveFilter();
            }
            OnChangeHealth?.Invoke(health.ToString());
        }
    }

    private float PickaxeStrength
    {
        get => pickaxeStrength;
        set
        {
            if (value < 0)
            {
                pickaxeStrength = 0;
                pickaxeStrengthBar.SetStrength(0);
            }
            else if (value > 100)
            {
                pickaxeStrength = 100f;
                pickaxeStrengthBar.SetStrength(100f);
            }
            else
            { 
                pickaxeStrength = value;
                pickaxeStrengthBar.SetStrength(value);
            }

            tileDamage = Mathf.Lerp(maxTileDamage * minTileDamageInPercents / 100, maxTileDamage, pickaxeStrength / 100);
            enemyDamage = (int)Mathf.Ceil(Mathf.Lerp(maxEnemyDamage * minEnemyDamageInPercents / 100, maxEnemyDamage, pickaxeStrength / 100));
        }
    }

    public void SetSelectedTile(Tile value) => selectedTile = value;

    public void RemoveSelectedTile() => selectedTile = null;

    public void AddForce(Vector2 force) => rigidBody2d.AddForce(force, ForceMode2D.Impulse);

    public void Sleep()
    {
        Health += healthRecovery;
        healthBar.SetHealth(Health);
        sanity.Sanity += sanityRecovery;
        sanityBar.SetSanity(sanity.Sanity);
    }

    public void SetStun()
    {
        isStunned = true;
        StartCoroutine(DisableStun(stunTime));
    }

    public void SetStun(float time)
    {
        isStunned = true;
        StartCoroutine(DisableStun(time));
    }

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else if (instanse == this)
            Destroy(gameObject);

        rigidBody2d = GetComponent<Rigidbody2D>();
        sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        sanity = GetComponent<SanityController>();
        consumables = GetComponent<Consumables>();

        groundMask = LayerMask.GetMask(ServiceInfo.GroundLayerName);
        enemiesMask = LayerMask.GetMask(ServiceInfo.EnemiesLayerName);
        fixedZPosition = transform.position.z;
    }

    private void Start()
    {
        checkpoint = transform.position;
        PickaxeStrength = pickaxeStrength;
    }

    private void Update()
    {
        if (isGrounded && !isStunned && Input.GetButtonDown("Jump"))
            Jump();

        if (canAttack && isGrounded && !isDigging && !isStunned && Input.GetButtonDown("Fire1"))
            Attack();
        //точильный камень
        if (Input.GetKeyDown(KeyCode.Alpha2) && consumables.GrindstonesCount > 0)
        {
            PickaxeStrength += Consumables.GrindstoneRecovery;
            pickaxeStrengthBar.SetStrength(PickaxeStrength);
            --consumables.GrindstonesCount;
            grindstones.text = "" + consumables.GrindstonesCount;
        }
        //Нажатие хилки
        if (Input.GetKeyDown(KeyCode.Alpha3) && consumables.HealthPacksCount > 0)
        {
            Health += Consumables.HealthPacksRecovery;
            healthBar.SetHealth(Health);
            --consumables.HealthPacksCount;
            healthPacks.text = "" + consumables.HealthPacksCount;
        }

    }

    private void FixedUpdate()
    {
        var position = transform.position;
        transform.position = new Vector3(position.x, position.y, fixedZPosition);

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

        Throw(collision.collider);
        GetDamage(collision.collider);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, touchingDistance);
    }

    // Назначен на ключ в анимации "Attack"
    private void OnAttack()
    {
        var raycastHits = Physics2D.RaycastAll(transform.position, transform.right * (sprite.flipX ? -1 : 1), attackDistance, enemiesMask);

        foreach (var raycastHit in raycastHits)
        {
            var creature = raycastHit.collider.GetComponent<Creature>();
            if (creature)
            {
                creature.Health -= enemyDamage;
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

        var damage = selectedTile.DiggingDifficulty < tileDamage ? tileDamage / selectedTile.DiggingDifficulty : tileDamage;
        selectedTile.Health -= damage;
        PickaxeStrength -= hitDamageToPickaxe;
        pickaxeStrengthBar.SetStrength(PickaxeStrength);
        canAttack = false;

        if (reloadAttack != null) StopCoroutine(reloadAttack);
        reloadAttack = StartCoroutine(ReloadAttack());
    }

    private void GetDamage(Collider2D collision)
    {
        var danger = collision.GetComponent<Danger>();
        if (danger)
        {
            Health -= danger.Damage;
            healthBar.SetHealth(Health);
            State = States.Pain;
            SetStun();

            var terrible = collision.GetComponent<Terrible>();
            if (terrible)
            { 
                sanity.Sanity -= terrible.DecreasingSanityAfterAttack;
                sanityBar.SetSanity(sanity.Sanity);
            }
                
        }
    }

    private void Throw(Collider2D collision)
    {
        var repulsion = collision.GetComponent<Repulsive>();
        if (repulsion)
        {
            var colPosition = collision.transform.position;
            var playerPosition = transform.position;

            rigidBody2d.AddForce(new Vector2((playerPosition.x - colPosition.x) * repulsion.ForceX, repulsion.ForceY), ForceMode2D.Impulse);
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

    //private void ChangeDirectionTowards(Vector2 position)
    //{
    //    sprite.flipX = position.x <= transform.position.x;
    //}

    private IEnumerator DisableInvulnerability()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        invulnerability = false;
    }

    private IEnumerator DisableStun(float afterTime)
    {
        yield return new WaitForSeconds(afterTime);
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