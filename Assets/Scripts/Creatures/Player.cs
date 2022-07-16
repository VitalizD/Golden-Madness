using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IStorage
{
    public static Player Instanse { get; private set; } = null;

    [Header("Loading Parameters")]
    [SerializeField] private bool loadParameters = true;

    [Header("Base")]
    [SerializeField] [Range(0, 100)] private int health = 100;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float climbingSpeed = 1f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float reloadClimbTime = 0.5f;
    [SerializeField] private LayerMask ropeMask;
    [SerializeField] private Transform character;
    [SerializeField] private CheckingForJump jumpCheckingPoint;
    [SerializeField] private Lamp lamp;
    [SerializeField] private PlayerDialogWindow dialogWindow;

    [Header("Fight")]
    [SerializeField] private int enemyDamage = 10;
    [SerializeField] private int maxEnemyDamage = 10;
    [SerializeField] private int minEnemyDamageInPercents = 60;
    [SerializeField] private float repulsiveForce;
    [SerializeField] private float attackTime = 0.5f;
    [SerializeField] private float reloadAttackTime = 0.1f;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float stunTime = 0.5f;
    [SerializeField] private PlayerAttackPoint attackPoint;

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

    [Header("SFX")]
    //[SerializeField] private AudioSource digSFX;
    [SerializeField] private SFX hurtSFX;
    //[SerializeField] private SFX digSFX;
    [SerializeField] private SFX swingSFX;
    [SerializeField] private SFX jumpSFX;
    [SerializeField] private SFX walkSFX;

    private Rigidbody2D rigidBody2d;
    private Animator animator;
    private SanityController sanity;
    private Consumables consumables;
    private Backpack backpack;
    private GameOver gameOver;
    private RedFilter displayFilter;

    private Tile selectedTile;
    private LayerMask enemiesMask;
    private Vector2 checkpoint;
    private Vector2 attackDistanse;
    private float defaultSpeed;

    private float fixedZPosition;
    private float scaleXValue;
    private float scaleX;

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
    private bool isClimbing = false;
    private bool canClimb = true;
    private bool adViewed = false;

    public bool IsDigging { get => isDigging; set => isDigging = value; }

    public bool CanJump { get => jumpCheckingPoint.CanJump; }

    public Vector2 Checkpoint { get => checkpoint; set => checkpoint = value; }

    public bool IsClimbing { get => isClimbing; }

    public float TouchingDistance { get => touchingDistance; }

    public float DefaultSpeed { get => defaultSpeed; }

    public float Speed { get => movementSpeed; set => movementSpeed = value; }

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
                hurtSFX.Play();
                //AudioManager.Instance.PlaySound(AudioManager.SoundName.PlayerHit);
                State = States.Pain;
                invulnerability = true;
                StartCoroutine(DisableInvulnerability());
                if (displayFilter != null) displayFilter.ChangeColor(health - value);
            }
            health = value > 100 ? 100 : value;

            if (HotbarController.Instanse != null)
                HotbarController.Instanse.SetBarValue(BarType.Health, health);

            if (health <= 0)
            {
                if (PlayerPrefs.HasKey(PlayerPrefsKeys.TutorialDone) && bool.Parse(PlayerPrefs.GetString(PlayerPrefsKeys.TutorialDone)))
                {
                    gameObject.SetActive(false);
                    if (adViewed)
                        gameOver.ShowAndReturnToVillage();
                    else
                        gameOver.ShowGameOverAd();
                }
                else
                {
                    Health = 100;
                    transform.position = checkpoint;
                }
                if (displayFilter != null) displayFilter.RemoveFilter();
            }
        }
    }

    public float PickaxeStrength
    {
        get => pickaxeStrength;
        set
        {
            pickaxeStrength = Mathf.Clamp(value, 0f, 100f);

            if (HotbarController.Instanse != null)
                HotbarController.Instanse.SetBarValue(BarType.Pickaxe, pickaxeStrength);

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

    public void SetFuelCount(float value) => lamp.FuelCount = value;

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

    public void ViewedAd()
    {
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

    public void SetCheckpoint() => checkpoint = transform.position;

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);

        rigidBody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sanity = GetComponent<SanityController>();
        consumables = GetComponent<Consumables>();
        backpack = GetComponent<Backpack>();
        gameOver = GameObject.FindGameObjectWithTag(ServiceInfo.GameOverTag).GetComponent<GameOver>();
        displayFilter = GameObject.FindGameObjectWithTag(ServiceInfo.RedFilterTag).GetComponent<RedFilter>();

        enemiesMask = LayerMask.GetMask(ServiceInfo.EnemiesLayerName);
        fixedZPosition = transform.position.z;
        scaleXValue = transform.localScale.x;
        character.localScale = transform.localScale;
        defaultSpeed = movementSpeed;

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
        if (Paused.Instanse != null && Paused.Instanse.IsPaused)
            return;

        if (jumpCheckingPoint.CanJump && !isAttacking && !isStunned && !isDigging && !Input.GetButton("Horizontal"))
        {
            if (isClimbing)
            {
                if (Input.GetButton("Vertical"))
                    State = States.ClimbMove;
                else
                    State = States.ClimbIdle;
            }
            else
                State = States.Idle;
        }

        if (jumpCheckingPoint.CanJump && !isStunned && Input.GetButtonDown("Jump"))
            Jump();

        if (canAttack && !isDigging && !isStunned && Input.GetButtonDown("Fire1"))
            Attack();

        if (Input.GetKeyDown(KeyCode.Alpha2) && consumables.GetCount(ConsumableType.Grindstone) > 0)
        {
            PickaxeStrength += consumables.GetRecovery(ConsumableType.Grindstone);
            consumables.Add(ConsumableType.Grindstone, -1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && consumables.GetCount(ConsumableType.HealthPack) > 0)
        {
            Health += (int)consumables.GetRecovery(ConsumableType.HealthPack);
            consumables.Add(ConsumableType.HealthPack, -1);
        }
    }

    private void FixedUpdate()
    {
        if (Paused.Instanse != null && Paused.Instanse.IsPaused)
            return;

        var position = transform.position;
        transform.position = new Vector3(position.x, position.y, fixedZPosition);

        if (Input.GetButton("Horizontal") && !isStunned && !isClimbing)
            Run();

        if (Input.GetButton("Vertical") && canClimb && Physics2D.OverlapCircleAll(transform.position, 0.1f, ropeMask).Length > 0)
            Climb();
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, touchingDistance);
    }


    private void OnPlayerStep() 
    {
        if(State==States.Walk&&jumpCheckingPoint.CanJump)
            walkSFX.Play();
    }

    // Назначен на ключ в анимации "Attack"
    private void OnAttack()
    {
        attackPoint.PlayTraceAnimation();

        var hits = Physics2D.OverlapCapsuleAll(attackPoint.transform.position, attackDistanse, CapsuleDirection2D.Horizontal, 0, enemiesMask);
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

    // Назначен на ключ в анимации "Dig"
    private void HitTile()
    {
        if (!selectedTile)
            return;
        //digSFX.Play();
        //AudioManager.Instance.PlaySound(AudioManager.SoundName.PlayerDig);
        var damage = selectedTile.DiggingDifficulty < tileDamage ? tileDamage / selectedTile.DiggingDifficulty : tileDamage;
        selectedTile.Health -= damage;
        PickaxeStrength -= hitDamageToPickaxe;
        canAttack = false;

        if (reloadAttack != null) StopCoroutine(reloadAttack);
        reloadAttack = StartCoroutine(ReloadAttack(reloadAttackTime - 0.5f));
    }

    private void GetDamage(Collider2D collision)
    {
        var danger = collision.GetComponent<Danger>();
        if (danger != null)
        {
            StopClimb();

            Health -= danger.Damage;
            State = States.Pain;
            SetStun();

            var terrible = collision.GetComponent<Terrible>();
            if (terrible != null)
                sanity.Sanity -= terrible.DecreasingSanityAfterAttack;                
        }
    }

    private void Throw(Collider2D collision)
    {
        var repulsion = collision.GetComponent<Repulsive>();
        if (repulsion != null)
        {
            var colPosition = collision.transform.position;
            var playerPosition = transform.position;

            rigidBody2d.AddForce(new Vector2((playerPosition.x - colPosition.x) * repulsion.ForceX, repulsion.ForceY), ForceMode2D.Impulse);
        }
    }

    private void Run()
    {
        if (isClimbing)
            return;

        if (jumpCheckingPoint.CanJump && !isAttacking && !isDigging)
        {
            //AudioManager.Instance.PlaySound(AudioManager.SoundName.PlayerWalk);
            State = States.Walk;
        }
            
        var dir = transform.right * Input.GetAxis("Horizontal");
        //rigidBody2d.AddForce(speed * dir, ForceMode2D.Impulse);
        transform.Translate(movementSpeed * Time.fixedDeltaTime * dir);
        //transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        if (State != States.Attack)
            Mirror(dir.x < 0);
    }

    private void Jump()
    {
        isDigging = false;
        //Debug.Log(AudioManager.SoundName.PlayerJump);
        jumpSFX.Play();
        //AudioManager.Instance.PlaySound(AudioManager.SoundName.PlayerJump);
        State = States.Jump;
        if (isClimbing)
        {
            StopClimb();
            if (Input.GetButton("Vertical") && Input.GetAxis("Vertical") < 0)
                return;
        }
        StartCoroutine(jumpCheckingPoint.Jump());
        rigidBody2d.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Climb()
    {
        if (!isClimbing)
        {
            isClimbing = true;
            State = States.ClimbIdle;
            rigidBody2d.constraints = RigidbodyConstraints2D.FreezePositionX;
            rigidBody2d.gravityScale = 0f;
            rigidBody2d.velocity = Vector2.zero;
            transform.position = new Vector2(Tile.GetCenterPositionOfNearestTile(transform.position).x, transform.position.y);
        }
        transform.rotation = Quaternion.identity;

        var dir = transform.up * Input.GetAxis("Vertical");
        rigidBody2d.MovePosition(rigidBody2d.position + climbingSpeed * Time.fixedDeltaTime * (Vector2)dir);
    }

    private void StopClimb()
    {
        isClimbing = false;
        rigidBody2d.gravityScale = 2.5f;
        rigidBody2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        canClimb = false;
        StartCoroutine(ReloadClimb());
    }

    private void Attack()
    {
        Mirror(Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x);

        State = States.Attack;
        canAttack = false;
        isAttacking = true;
        swingSFX.Play();
        //AudioManager.Instance.PlaySound(AudioManager.SoundName.PlayerSwing);
        StartCoroutine(FinishAttack());
        reloadAttack = StartCoroutine(ReloadAttack(reloadAttackTime));
    }

    private void Mirror(bool condition)
    {
        scaleX = condition ? -scaleXValue : scaleXValue;
        character.localScale = new Vector3(scaleX, character.localScale.y, character.localScale.z);
    }

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

    private IEnumerator ReloadAttack(float afterTime)
    {
        yield return new WaitForSeconds(afterTime);
        canAttack = true;
    }

    private IEnumerator ReloadClimb()
    {
        yield return new WaitForSeconds(reloadClimbTime);
        canClimb = true;
    }
}