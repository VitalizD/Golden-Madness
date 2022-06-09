using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Player : MonoBehaviour, IStorage
{
    public static Player instanse = null;

    [Header("Loading Parameters")]
    [SerializeField] private bool loadParameters = true;

    [Header("Base")]
    [SerializeField] [Range(0, 100)] private int health = 100;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private CheckingForJump jumpCheckingPoint;
    public HealthBar healthBar;
    public SanityBar sanityBar;
    public Text healthPacks;

    [Header("Fight")]
    [SerializeField] private int enemyDamage = 10;
    [SerializeField] private int maxEnemyDamage = 10;
    [SerializeField] private int minEnemyDamageInPercents = 60;
    [SerializeField] private float repulsiveForce;
    [SerializeField] private float jerkForce = 3f;
    [SerializeField] private float attackTime = 0.3f;
    [SerializeField] private float reloadAttackTime = 0.5f;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float stunTime = 0.5f;
    [SerializeField] private Transform attackPoint;

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
    private Backpack backpack;
    private Lamp lamp;
    private PlayerDialogWindow dialogWindow;
    private GameOver gameOver;
    private RedFilter displayFilter;
    private Paused paused;

    private Tile selectedTile;
    private LayerMask groundMask;
    private LayerMask enemiesMask;
    private Vector2 checkpoint;
    private Vector2 attackDistanse;
    private float fixedZPosition;
    private float xAttackPoint;

    private float initialEnemyDamage;
    private float initialTileDamage;
    private float initialHitDamageToPickaxe;

    private Coroutine reloadAttack;

    private bool invulnerability = false;
    private bool isStunned = false;
    private bool feelPain = false;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool isDigging = false;
    private bool adViewed = false;

    public bool IsDigging { get => isDigging; set => isDigging = value; }

    public Vector2 Checkpoint { get => checkpoint; set => checkpoint = value; }

    public bool CanJump { get => jumpCheckingPoint.CanJump; }

    public float TouchingDistance { get => touchingDistance; }

    public void ViewedAd()
    {
        //paused.Pause();
        transform.position = checkpoint;
        gameObject.SetActive(true);
        Health = 100;
        sanity.Sanity = 100;
        invulnerability = false;
        isStunned = false;
        feelPain = false;
        isAttacking = false;
        canAttack = true;
        isDigging = false;
        adViewed = true;
    }

    public void NonViewedAd()
    {
        StopAllCoroutines();
    }

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
            if (value < health && !loadParameters)
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
                if (PlayerPrefs.HasKey(PlayerPrefsKeys.TutorialDone) && bool.Parse(PlayerPrefs.GetString(PlayerPrefsKeys.TutorialDone)))
                {
                    gameObject.SetActive(false);
                    if (adViewed)
                    {
                        gameOver.ShowAndReturnToVillage();
                    }
                    else
                    {
                        gameOver.ShowGameOverAd();
                    }
                }
                else
                {
                    health = 100;
                    transform.position = checkpoint;
                }
                if (displayFilter) displayFilter.RemoveFilter();
            }
        }
    }

    public float PickaxeStrength
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

    public void Save()
    {
        backpack.Save();
        consumables.Save();
        lamp.Save();
        sanity.Save();

        PlayerPrefs.SetFloat(PlayerPrefsKeys.HitDamageToPickaxe, hitDamageToPickaxe);
        PlayerPrefs.SetInt(PlayerPrefsKeys.MaxEnemyDamage, maxEnemyDamage);
        PlayerPrefs.SetFloat(PlayerPrefsKeys.MaxTileDamage, maxTileDamage);
        PlayerPrefs.SetInt(PlayerPrefsKeys.SleepingBagHealthRecovery, healthRecovery);
        PlayerPrefs.SetFloat(PlayerPrefsKeys.SleepingBagSanityRecovery, sanityRecovery);
        PlayerPrefs.SetFloat(PlayerPrefsKeys.PickaxeStrength, pickaxeStrength);
        PlayerPrefs.SetInt(PlayerPrefsKeys.Health, Health);
    }

    public void Load()
    {
        if (!loadParameters)
            return;

        backpack.Load();
        consumables.Load();
        lamp.Load();
        sanity.Load();

        hitDamageToPickaxe = PlayerPrefs.GetFloat(PlayerPrefsKeys.HitDamageToPickaxe, hitDamageToPickaxe);
        maxEnemyDamage = PlayerPrefs.GetInt(PlayerPrefsKeys.MaxEnemyDamage, enemyDamage);
        maxTileDamage = PlayerPrefs.GetFloat(PlayerPrefsKeys.MaxTileDamage, tileDamage);
        healthRecovery = PlayerPrefs.GetInt(PlayerPrefsKeys.SleepingBagHealthRecovery, healthRecovery);
        sanityRecovery = PlayerPrefs.GetFloat(PlayerPrefsKeys.SleepingBagSanityRecovery, sanityRecovery);
        PickaxeStrength = PlayerPrefs.GetFloat(PlayerPrefsKeys.PickaxeStrength, pickaxeStrength);
        Health = PlayerPrefs.GetInt(PlayerPrefsKeys.Health, Health);

        loadParameters = false;
    }

    public void SetSelectedTile(Tile value) => selectedTile = value;

    public void RemoveSelectedTile() => selectedTile = null;

    public void AddForce(Vector2 force) => rigidBody2d.AddForce(force, ForceMode2D.Impulse);

    public void AddMaxEnemyDamage(int valueInPercents)
    {
        maxEnemyDamage += (int)(initialEnemyDamage * (valueInPercents / 100f));
        enemyDamage = maxEnemyDamage;
        Save();
    }

    public void AddMaxTileDamage(float valueInPercents)
    {
        maxTileDamage += initialTileDamage * (valueInPercents / 100f);
        tileDamage = maxTileDamage;
        Save();
    }

    public void AddHitDamageToPickaxe(float valueInPercents)
    {
        hitDamageToPickaxe += initialHitDamageToPickaxe * (valueInPercents / 100f);
        Save();
    }

    public void AddTimeDecreaseValue(float value)
    {
        lamp.TimeFuelDecrease += value;
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
        healthBar.SetHealth(Health);
        sanity.Sanity += sanityRecovery;
        sanityBar.SetSanity(sanity.Sanity);
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
        xAttackPoint = attackPoint.localPosition.x;

        attackDistanse = attackPoint.GetComponent<CapsuleCollider2D>().size;

        initialEnemyDamage = maxEnemyDamage;
        initialHitDamageToPickaxe = hitDamageToPickaxe;
        initialTileDamage = maxTileDamage;
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
        if (jumpCheckingPoint.CanJump && !isStunned && Input.GetButtonDown("Jump"))
            Jump();

        if (canAttack && !isDigging && !isStunned && Input.GetButtonDown("Fire1"))
            Attack();
        //��������� ������
        if (Input.GetKeyDown(KeyCode.Alpha2) && consumables.GrindstonesCount > 0)
        {
            PickaxeStrength += Consumables.GrindstoneRecovery;
            pickaxeStrengthBar.SetStrength(PickaxeStrength);
            --consumables.GrindstonesCount;
            grindstones.text = "" + consumables.GrindstonesCount;
        }
        //������� �����
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

        speed = backpack.isFull() ? 3f*0.85f : 3f;

        //CheckGrounded();

        if (jumpCheckingPoint.CanJump && !isAttacking && !isStunned && !isDigging)
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
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(attackPoint.position, attackDistanceX);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, touchingDistance);

    }

    // �������� �� ���� � �������� "Attack"
    private void OnAttack()
    {
        //var raycastHits = Physics2D.RaycastAll(transform.position, transform.right * (sprite.flipX ? -1 : 1), attackDistance, enemiesMask);
        var hits = Physics2D.OverlapCapsuleAll(attackPoint.position, attackDistanse, CapsuleDirection2D.Vertical, 0, enemiesMask);

        foreach (var raycastHit in hits)
        {
            var creature = raycastHit.GetComponent<Creature>();
            if (creature != null)
            {
                creature.Health -= enemyDamage;
                if (creature.Repulsiable)
                    creature.Throw(transform.position, repulsiveForce);
            }
        }
    }

    // �������� �� ���� � �������� "Dig"
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
        if (jumpCheckingPoint.CanJump && !isAttacking)
            State = States.Walk;

        var dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0;
        attackPoint.localPosition = sprite.flipX ? new Vector3(-xAttackPoint, attackPoint.localPosition.y, 5) : new Vector3(xAttackPoint, attackPoint.localPosition.y, 5);
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

    //private void CheckGrounded()
    //{
    //    if (!jumpCheckingPoint.CanJump)
    //    {
    //        if (rigidBody2d.velocity.y > 0)
    //            State = States.Jump;
    //        else if (rigidBody2d.velocity.y < 0)
    //            State = States.Fall;
    //    }

    //    var collaiders = Physics2D.OverlapCircleAll(
    //        new Vector2(transform.position.x, transform.position.y + yOffsetToGround), jumpCheckRadius, groundMask);
    //    isGrounded = collaiders.Length > 0;
    //}

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