using UnityEngine;
using System.Collections;

public class Stalactite : MonoBehaviour
{
    [SerializeField] private float rangeToActivate;
    [SerializeField] private float shakingRange;
    [SerializeField] private float shakingRate = 10f;
    [SerializeField] private float timeBeforeFalling;
    [SerializeField] private int extraFallingDamage = 5;
    [SerializeField] private float repulsiveForce;

    private Rigidbody2D rb;
    private Transform player;

    private float leftBorderTrigger;
    private float rightBorderTrigger;
    private float leftBorderShaking;
    private float rightBorderShaking;
    private string dangerPointName = "Danger Point";

    private bool isActive = false;
    private bool isShaking = false;

    public void Active(float timeBeforeFalling)
    {
        if (LevelGeneration.Instanse.IsGenerated)
            StartCoroutine(Fall(timeBeforeFalling));
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        leftBorderTrigger = transform.position.x - rangeToActivate;
        rightBorderTrigger = transform.position.x + rangeToActivate;
        leftBorderShaking = transform.position.x - shakingRange;
        rightBorderShaking = transform.position.x + shakingRange;
    }

    private void Start()
    {
        player = Player.Instanse.transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(leftBorderTrigger, transform.position.y), new Vector2(rightBorderTrigger, transform.position.y));
        Gizmos.DrawLine(new Vector2(leftBorderShaking, transform.position.y), new Vector2(rightBorderShaking, transform.position.y + 0.1f));
    }

    private void FixedUpdate()
    {
        if (isShaking)
            transform.position = new Vector2(Mathf.Lerp(leftBorderShaking, rightBorderShaking, Mathf.PingPong(Time.time * shakingRate, 1)), transform.position.y);

        if (!isActive && 
            player.position.x >= leftBorderTrigger && 
            player.position.x <= rightBorderTrigger && 
            player.position.y < transform.position.y)
            StartCoroutine(Fall(timeBeforeFalling));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isShaking || rb.bodyType == RigidbodyType2D.Static)
            return;

        var player = collision.gameObject.GetComponent<Player>();
        if (player)
        {
            player.AddForce(transform.up * repulsiveForce);
            player.Health -= extraFallingDamage;
        }

        var creature = collision.gameObject.GetComponent<Creature>();
        if (creature)
        {
            creature.AddForce(transform.up * repulsiveForce);
            creature.Health -= extraFallingDamage;
        }

        StartCoroutine(Destroy());
    }

    private IEnumerator Fall(float timeBeforeFalling)
    {
        isActive = true;
        isShaking = true;

        yield return new WaitForSeconds(timeBeforeFalling);

        var dangerPoint = transform.Find(dangerPointName);
        if (dangerPoint) Destroy(dangerPoint.gameObject);
        rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(0.1f);

        isShaking = false;
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.01f);
        Destroy(gameObject);
    }
}
