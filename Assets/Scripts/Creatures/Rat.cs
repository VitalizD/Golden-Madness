using System.Collections;
using UnityEngine;

public class Rat : MonoBehaviour
{
    [SerializeField] private float obstacleCheckBetweenTime = 0.2f;
    [SerializeField] private float obstacleCheckRadius = 0.01f;
    [SerializeField] private float obstacleCheckOffsetX = 0.4f;
    [SerializeField] private float obstacleCheckOffsetY = 1;
    [SerializeField] private float stayBetweenTimeMin = 1;
    [SerializeField] private float stayBetweenTimeMax = 5;
    [SerializeField] private float stayTimeMin = 1;
    [SerializeField] private float stayTimeMax = 5;
    [SerializeField] private float playerCheckBetweenTime = 0.5f;
    [SerializeField] private float aggressiveModeTime = 5f;
    [SerializeField] private float aggressiveSpeed = 1.5f;

    private Creature creature;
    private SpriteRenderer sprite;
    private Coroutine temporarilyStop;

    private int directionValue = 1; // 1 - положительный X (право);   -1 - отрицательный X (лево)
    private float normalSpeed;
    private bool angry = false;
    private bool isMoving = true;

    private void Awake()
    {
        creature = GetComponent<Creature>();
        sprite = GetComponent<SpriteRenderer>();
        normalSpeed = creature.Speed;
    }

    private void Start()
    {
        StartCoroutine(CheckObstacle());
        temporarilyStop = StartCoroutine(TemporarilyStop());
        StartCoroutine(CheckPlayer());
    }

    private void Update()
    {
        if (isMoving)
            Run();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.collider.GetComponent<Player>();
        if (player)
        {
            var collisionDirectionValue = transform.position.x > player.transform.position.x ? -1 : 1;
            if (collisionDirectionValue != directionValue)
                ChangeDirection();
        }
    }

    private void Run()
    {
        creature.State = States.Walk;
        var direction = transform.right * directionValue;
        transform.position = Vector2.MoveTowards(transform.position, transform.position + direction, creature.Speed * Time.deltaTime);
    }

    private void Stop()
    {
        creature.State = States.Idle;
    }

    private IEnumerator CheckObstacle()
    {
        while (true)
        {
            var groundMask = LayerMask.GetMask("Ground");
            var checkingPoint1 = new Vector2(transform.position.x + obstacleCheckOffsetX * directionValue, transform.position.y);
            var checkingPoint2 = new Vector2(checkingPoint1.x, checkingPoint1.y - obstacleCheckOffsetY);
            if (Physics2D.OverlapCircleAll(checkingPoint1, obstacleCheckRadius, groundMask).Length > 0 ||
                (Physics2D.OverlapCircleAll(checkingPoint2, obstacleCheckRadius, groundMask).Length == 0 && !angry))
                ChangeDirection();
            yield return new WaitForSeconds(obstacleCheckBetweenTime);
        }
    }

    private IEnumerator CheckPlayer()
    {
        while (!angry)
        {
            yield return new WaitForSeconds(playerCheckBetweenTime);
            var startPoint = new Vector2(transform.position.x + obstacleCheckOffsetX * directionValue, transform.position.y);
            var raycastObject = Physics2D.Raycast(startPoint, transform.right * directionValue).collider.gameObject;
            var player = raycastObject.GetComponent<Player>();
            if (player)
                StartCoroutine(ActivateAggressiveMode());
        }
    }

    private void ChangeDirection()
    {
        sprite.flipX = !sprite.flipX;
        directionValue = sprite.flipX ? -1 : 1;
    }

    private IEnumerator TemporarilyStop()
    {
        yield return new WaitForSeconds(Random.Range(stayBetweenTimeMin, stayBetweenTimeMax));
        isMoving = false;
        Stop();

        yield return new WaitForSeconds(Random.Range(stayTimeMin, stayTimeMax));
        isMoving = true;
        temporarilyStop = StartCoroutine(TemporarilyStop());
    }

    private IEnumerator ActivateAggressiveMode()
    {
        StopCoroutine(temporarilyStop);
        angry = true;
        isMoving = true;
        creature.Speed = aggressiveSpeed;

        yield return new WaitForSeconds(aggressiveModeTime);

        angry = false;
        creature.Speed = normalSpeed;
        temporarilyStop = StartCoroutine(TemporarilyStop());
        StartCoroutine(CheckPlayer());
    }
}
