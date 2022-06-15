using UnityEngine;
using System.Collections;

public class Stalactite : MonoBehaviour
{
    [SerializeField] private float rangeToActivate;
    [SerializeField] private float shakingRange;
    [SerializeField] private float shakingRate = 10f;
    [SerializeField] private float timeBeforeFallingMin = 0.25f;
    [SerializeField] private float timeBeforeFallingMax = 2f;
    [SerializeField] private int extraFallingDamage = 5;
    [SerializeField] private float repulsiveForce;
    [SerializeField] private Transform raycastStartPoint;

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
            StartCoroutine(Fall(timeBeforeFalling, timeBeforeFalling));
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

        if (PlayerDetected())
            StartCoroutine(Fall(timeBeforeFallingMin, timeBeforeFallingMax));
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

    private bool PlayerDetected()
    {
        var playerNear = !isActive &&
            player.position.x >= leftBorderTrigger &&
            player.position.x <= rightBorderTrigger &&
            player.position.y < transform.position.y;

        if (!playerNear)
            return false;

        var layer = 1 << 3 | 1 << 7; // 3 - Ground; 7 - Player
        var raycastHit = Physics2D.Raycast(raycastStartPoint.position, -transform.up, Mathf.Infinity, layer);
        var playerDetection = raycastHit.collider.GetComponent<Player>();

        return playerDetection != null;
    }

    private IEnumerator Fall(float timeBeforeFallingMin, float timeBeforeFallingMax)
    {
        isActive = true;
        isShaking = true;

        yield return new WaitForSeconds(Random.Range(timeBeforeFallingMin, timeBeforeFallingMax));

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
