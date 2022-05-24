using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour, IStorage
{
    public static Player instanse = null;

    [Header("Loading Parameters")]
    [SerializeField] private bool loadParameters = true;

    [Header("Base")]
    [SerializeField] [Range(0, 100)] private int health = 100;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 5f;

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
    [SerializeField] private UnityEvent<string> OnChangeHealth;

    [Header("Pickaxe")]
    [SerializeField] [Range(0, 100f)] private float pickaxeStrength = 100f;
    [SerializeField] private float hitDamageToPickaxe = 1f;
    [SerializeField] private float tileDamage = 1f;
    [SerializeField] private float maxTileDamage = 1f;
    [SerializeField] [Range(0, 100f)] private float minTileDamageInPercents = 20;
    [SerializeField] private float touchingDistance;

    [Header("Sleeping Bag")]
    [SerializeField] [Range(0, 100)] private int healthRecovery = 20;
    [SerializeField] [Range(0, 100f)] private float sanityRecovery = 50f;

    private Rigidbody2D rigidBody2d;
    private SpriteRenderer sprite;
    private Animator animator;
    private SanityController sanity;
    private Consumables consumables;
    private Backpack backpack;
    private Lamp lamp;
    private PlayerDialogWindow dialogWindow;
    private GameOver gameOver;
    private RedFilter displayFilter;

    private Tile selectedTile;
    private LayerMask groundMask;
    private LayerMask enemiesMask;
    private Vector2 checkpoint;
    private float fixedZPosition;

    private Coroutine reloadAttack;

    private readonly float jumpCheckRadius = 0.07f;
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
            if (health <= 0)
            {
                if (PlayerPrefs.HasKey(PlayerPrefsKeys.TutorialDone) && bool.Parse(PlayerPrefs.GetString(PlayerPrefsKeys.TutorialDone)))
                {
                    StopAllCoroutines();
                    gameObject.SetActive(false);
                    //sprite.enabled = false;
                    //isStunned = true;
                    //rigidBody2d.bodyType = RigidbodyType2D.Static;
                    gameOver.ShowAndReturnToVillage();
                }
                else
                {
                    health = 100;
                    transform.position = checkpoint;
                }
                if (displayFilter) displayFilter.RemoveFilter();
            }
            OnChangeHealth?.Invoke(health.ToString());
        }
    }

    public float PickaxeStrength
    {
        get => pickaxeStrength;
        set
        {
            if (value < 0) pickaxeStrength = 0;
            else if (value > 100) pickaxeStrength = 100f;
            else pickaxeStrength = value;

            tileDamage = Mathf.Lerp(maxTileDamage * minTileDamageInPercents / 100, maxTileDamage, pickaxeStrength / 100);
            enemyDamage = (int)Mathf.Ceil(Mathf.Lerp(maxEnemyDamage * minEnemyDamageInPercents / 100, maxEnemyDamage, pickaxeStrength / 100));
        }
    }

    public void Save()
    {
        backpack.Save();
        consumables.Save();
        lamp.Save();

        PlayerPrefs.SetFloat(PlayerPrefsKeys.HitDamageToPickaxe, hitDamageToPickaxe);
        PlayerPrefs.SetInt(PlayerPrefsKeys.MaxEnemyDamage, maxEnemyDamage);
        PlayerPrefs.SetFloat(PlayerPrefsKeys.MaxTileDamage, maxTileDamage);
        PlayerPrefs.SetInt(PlayerPrefsKeys.SleepingBagHealthRecovery, healthRecovery);
        PlayerPrefs.SetFloat(PlayerPrefsKeys.SleepingBagSanityRecovery, sanityRecovery);
        PlayerPrefs.SetFloat(PlayerPrefsKeys.PickaxeStrength, pickaxeStrength);
    }

    public void Load()
    {
        if (!loadParameters)
            return;

        backpack.Load();
        consumables.Load();
        lamp.Load();

        hitDamageToPickaxe = PlayerPrefs.GetFloat(PlayerPrefsKeys.HitDamageToPickaxe, hitDamageToPickaxe);
        maxEnemyDamage = PlayerPrefs.GetInt(PlayerPrefsKeys.MaxEnemyDamage, enemyDamage);
        maxTileDamage = PlayerPrefs.GetFloat(PlayerPrefsKeys.MaxTileDamage, tileDamage);
        healthRecovery = PlayerPrefs.GetInt(PlayerPrefsKeys.SleepingBagHealthRecovery, healthRecovery);
        sanityRecovery = PlayerPrefs.GetFloat(PlayerPrefsKeys.SleepingBagSanityRecovery, sanityRecovery);
        PickaxeStrength = PlayerPrefs.GetFloat(PlayerPrefsKeys.PickaxeStrength, pickaxeStrength);
    }

    public void SetSelectedTile(Tile value) => selectedTile = value;

    public void RemoveSelectedTile() => selectedTile = null;

    public void AddForce(Vector2 force) => rigidBody2d.AddForce(force, ForceMode2D.Impulse);

    public void AddMaxEnemyDamage(int valueInPercents)
    {
        maxEnemyDamage += (int)(maxEnemyDamage * (valueInPercents / 100f));
        enemyDamage = maxEnemyDamage;
        Save();
    }

    public void AddMaxTileDamage(float valueInPercents)
    {
        maxTileDamage += maxTileDamage * (valueInPercents / 100f);
        tileDamage = maxTileDamage;
        Save();
    }

    public void AddHitDamageToPickaxe(float valueInPercents)
    {
        hitDamageToPickaxe += hitDamageToPickaxe * (valueInPercents / 100f);
        Save();
    }

    public void AddFuelDecreaseValue(float valueInPercents)
    {
        lamp.FuelDecreaseValue += lamp.FuelDecreaseValue * (valueInPercents / 100f);
        Save();
    }

    public void AddSleepingBagHealthRecovery(int valueInPercents)
    {
        healthRecovery += (int)(healthRecovery * (valueInPercents / 100f));
        Save();
    }

    public void AddSleepingBagSanityRecovery(float valueInPercents)
    {
        sanityRecovery += (int)(sanityRecovery * (valueInPercents / 100f));
        Save();
    }

    public void AddBackpackCapacity(int value)
    {
        backpack.MaxCapacity += value;
        Save();
    }

    public void Sleep()
    {
        Health += healthRecovery;
        sanity.Sanity += sanityRecovery;
    }

    public void SetStun() => SetStun(stunTime);

    public void SetStun(float time)
    {
        isStunned = true;
        if (gameObject.activeSelf)
            StartCoroutine(DisableStun(time));
    }

    public void Say(string text, float sayingTime)
    {
        dialogWindow.gameObject.SetActive(true);
        dialogWindow.Show(text, sayingTime);
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
        backpack = GetComponent<Backpack>();
        lamp = transform.GetChild(ServiceInfo.ChildIndexOfLamp).GetComponent<Lamp>();
        dialogWindow = transform.GetChild(ServiceInfo.ChildIndexOfDialogWindow).GetComponent<PlayerDialogWindow>();
        gameOver = GameObject.FindGameObjectWithTag(ServiceInfo.GameOverTag).GetComponent<GameOver>();
        displayFilter = GameObject.FindGameObjectWithTag(ServiceInfo.RedFilterTag).GetComponent<RedFilter>();

        groundMask = LayerMask.GetMask(ServiceInfo.GroundLayerName);
        enemiesMask = LayerMask.GetMask(ServiceInfo.EnemiesLayerName);
        fixedZPosition = transform.position.z;
    }

    private void Start()
    {
        checkpoint = transform.position;
        PickaxeStrength = pickaxeStrength;

        if (loadParameters)
            Load();
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
            --consumables.GrindstonesCount;
        }
        //Нажатие хилки
        if (Input.GetKeyDown(KeyCode.Alpha3) && consumables.HealthPacksCount > 0)
        {
            Health += Consumables.HealthPacksRecovery;
            --consumables.HealthPacksCount;
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
        canAttack = false;

        if (reloadAttack != null) StopCoroutine(reloadAttack);
        reloadAttack = StartCoroutine(ReloadAttack());
    }

    private void GetDamage(Collider2D collision)
    {
        var danger = collision.GetComponent<Danger>();
        if (danger)
        {
            State = States.Pain;
            SetStun();
            Health -= danger.Damage;

            var terrible = collision.GetComponent<Terrible>();
            if (terrible)
                sanity.Sanity -= terrible.DecreasingSanityAfterAttack;
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