using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Player : MonoBehaviour
{
    public static Player instanse = null;
    
    [SerializeField] private int health = 10;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float hitForce;
    [SerializeField] private float jumpCheckRadius = 0.45f;
    [SerializeField] private float touchingDistance;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float stunTime = 0.5f;
    [SerializeField] private float yOffsetToGround = -0.5f;

    [SerializeField] private UnityEvent<string> OnGetDamage;

    private Rigidbody2D rigidBody2d;
    private SpriteRenderer sprite;
    private Animator animator;
    private Animation animation_;
    private LayerMask groundMask;

    private Tile selectedTile;
    private Vector3 oldPos;
    private bool invulnerability;

    public States State
    {
        get => (States)animator.GetInteger("State");
        set => animator.SetInteger("State", (int)value);
    }

    private bool Invulnerability
    {
        get => animation_.IsPlaying("Flash");
        set
        {
            if (value)
                animation_.Play("Flash");
            else
                animation_.Stop("Flash");
        }
    }

    public bool IsDigging { get; set; } = false;

    public bool IsStunned { get; private set; } = false;

    public bool IsGrounded { get; private set; } = false;

    public bool IsMoving { get; private set; } = false;

    public float TouchingDistance { get => touchingDistance; }

    public float HitDamage { get => hitForce; }

    public int Health
    {
        get => health;
        set
        {
            health = value;
            OnGetDamage?.Invoke(health.ToString());
            if (health <= 0)
                Destroy(gameObject);
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
        animation_ = GetComponent<Animation>();
        groundMask = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        if (IsGrounded && !IsStunned && Input.GetButtonDown("Jump"))
            Jump();
    }

    private void FixedUpdate()
    {
        IsMoving = transform.position != oldPos;
        oldPos = transform.position;

        CheckGrounded();

        if (IsDigging)
            State = States.Dig;
        else if (IsGrounded)
            State = States.Idle;

        if (!IsStunned && Input.GetButton("Horizontal"))
            Run();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Invulnerability)
            return;

        Throw(collision);
        GetDamage(collision);
    }

    private void HitTile()
    {
        if (!selectedTile)
            return;

        var damage = selectedTile.DiggingDifficulty < HitDamage ? HitDamage / selectedTile.DiggingDifficulty : HitDamage;
        selectedTile.Health -= damage;
    }

    private void GetDamage(Collision2D collision)
    {
        var danger = collision.gameObject.GetComponent<Danger>();
        if (danger)
        {
            Health -= danger.Damage;

            Invulnerability = true;
            IsStunned = true;

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

            rigidBody2d.AddForce(new Vector2(playerPos.x - colPos.x, playerPos.y - colPos.y + yOffsetToGround + 1)
                * repulsion.Force, ForceMode2D.Impulse);
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
        Invulnerability = false;
    }

    private IEnumerator DisableStun()
    {
        yield return new WaitForSeconds(stunTime);
        IsStunned = false;
    }
}