using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Rat : MonoBehaviour, ICreature
{
    private const float obstacleCheckRadius = 0.1f;
    private const float obstacleCheckOffsetX = 0.5f;
    private const float obstacleCheckOffsetY = 1;

    [SerializeField] private float obstacleCheckBetweenTime = 0.2f;
    [SerializeField] private float stayBetweenTimeMin = 1;
    [SerializeField] private float stayBetweenTimeMax = 5;
    [SerializeField] private float stayTimeMin = 1;
    [SerializeField] private float stayTimeMax = 5;
    [SerializeField] private float playerCheckBetweenTime = 0.5f;
    [SerializeField] private float aggressiveModeTime = 5f;
    [SerializeField] private float aggressiveSpeed = 1.5f;
  

    [SerializeField] private UnityEvent onDestroy;

    private Creature creature;
    private SpriteRenderer sprite;

    private Coroutine temporarilyStop;
    private Coroutine activateAggressiveMode;

    private float normalSpeed;
    private bool angry = false;
    private bool isMoving = true;

    public void ReactToAttack()
    {
        activateAggressiveMode = StartCoroutine(ActivateAggressiveMode());
    }

    private void Awake()
    {
        creature = GetComponent<Creature>();
        creature.SetChild(this);
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
            creature.ChangeDirectionTowards(player.transform.position);
    }

    private void OnDestroy()
    {
        onDestroy?.Invoke();
    }

    private void Run()
    {
        creature.State = States.Walk;
        var direction = transform.right * creature.DirectionValue;
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
            if (isMoving)
            {
                var layer = 1 << 3 | 1 << 6; // 3 - Ground; 6 - Enemies

                var checkingPoint1 = new Vector2(transform.position.x + obstacleCheckOffsetX * creature.DirectionValue, transform.position.y - 0.1f);
                var checkingPoint2 = new Vector2(checkingPoint1.x, checkingPoint1.y - obstacleCheckOffsetY);

                if (Physics2D.OverlapCircleAll(checkingPoint1, obstacleCheckRadius, layer).Length > 0 ||
                    (Physics2D.OverlapCircleAll(checkingPoint2, obstacleCheckRadius, layer).Length == 0 && !angry && !creature.Attacked))
                    creature.ChangeDirection();
            }
            yield return new WaitForSeconds(obstacleCheckBetweenTime);
        }
    }

    private IEnumerator CheckPlayer()
    {
        while (!angry)
        {
            yield return new WaitForSeconds(playerCheckBetweenTime);
            var startPoint = new Vector2(transform.position.x + obstacleCheckOffsetX * creature.DirectionValue, transform.position.y);
            var layer = 1 << 3 | 1 << 7; // 3 - Ground; 7 - Player
            var raycastHit = Physics2D.Raycast(startPoint, transform.right * creature.DirectionValue, Mathf.Infinity, layer);
            if (raycastHit)
            {
                var player = raycastHit.collider.GetComponent<Player>();
                if (player != null)
                    activateAggressiveMode = StartCoroutine(ActivateAggressiveMode());
            }
        }
    }

    private IEnumerator TemporarilyStop()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(stayBetweenTimeMin, stayBetweenTimeMax));
        isMoving = false;
        Stop();

        yield return new WaitForSeconds(UnityEngine.Random.Range(stayTimeMin, stayTimeMax));
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
