using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    private const string shakeAnimationName = "Shake";

    [SerializeField] private float health = 10;
    [SerializeField] private float diggingDifficulty = 1;
    [SerializeField] private float shakingTime = 0.24f;
    [SerializeField] private bool isBedrock = false;
    [SerializeField] private bool destroyAttachedTiles = true;
    [SerializeField] private ResourceTypes resourceType = ResourceTypes.None;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Sprite[] destructionDegrees;
    [SerializeField] private Sprite[] variants;

    [Space]

    [SerializeField] private SpriteRenderer tileSprite;
    [SerializeField] private SpriteRenderer destructionSprite;

    private Selection selection;
    private Player player;
    private Animation animation_;

    private float maxHealth;
    private readonly float checkingDistanceToDestroyAttachedTiles = 0.505f;

    public float DiggingDifficulty { get => diggingDifficulty; }

    public ResourceTypes ResourceType { get => resourceType; }

    public float Health
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0)
            {
                AddResourceToBackpack(resourceType);
                if (destroyAttachedTiles)
                    DestroyAttachedTiles();
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(Shake());
                if (destructionDegrees.Length > 0)
                    ChangeDestructionDegree();
            }
        }
    }

    public bool IsBedrock { get => isBedrock; set => isBedrock = value; }

    private void Awake()
    {
        selection = GameObject.FindGameObjectWithTag(ServiceInfo.SelectionTag).GetComponent<Selection>();
        maxHealth = health;
        animation_ = GetComponent<Animation>();

        if (variants.Length > 0)
            tileSprite.sprite = variants[Random.Range(0, variants.Length)];
    }

    private void Start()
    {
        player = Player.Instanse;

        //var tileDetection = Physics2D.OverlapCircle(transform.position, 0.001f, groundMask);
        //if (tileDetection != null)
        //    Destroy(gameObject);
    }

    /*private void OnMouseEnter()
    {
        *//*if (isBedrock)
            return;*/

        /*if (selection != null)
            selection.gameObject.SetActive(true);
        if (player != null)
            player.SetSelectedTile(this);*//*
    }*/

    private void OnMouseExit()
    {
        StopDigging();
        if (selection != null)
            selection.SetActive(false);
        if (player != null)
            player.RemoveSelectedTile();
    }

    private void OnMouseDown()
    {
        CheckDistance();
    }

    private void OnMouseOver()
    {
        if (selection == null || player == null)
            return;

        if (!isBedrock && IsSelectionInTouchingDistance())
        {
            selection.Move(transform.position);
            selection.SetActive(true);
            player.SetSelectedTile(this);
        }
        else
        {
            selection.SetActive(false);
            player.RemoveSelectedTile();
        }

        if (Input.GetButton("Fire1"))
        {
            if (player.State != States.Dig)
                selection.SetNormalColor();
            CheckDistance();
        }
    }

    private void OnMouseUp()
    {
        StopDigging();
    }

    private void OnDestroy()
    {
        StopDigging();
        if (selection != null)
            selection.SetActive(false);
    }

    private void StopDigging()
    {
        if (player != null)
            player.IsDigging = false;
        if (selection != null)
            selection.SetNormalColor();
    }

    private bool IsSelectionInTouchingDistance()
    {
        if (isBedrock)
            return false;

        var referencePoint = new Vector2(player.transform.position.x, player.transform.position.y);
        var distance = Vector2.Distance(transform.position, referencePoint);
        return distance < player.TouchingDistance;
    }

    private void CheckDistance()
    {
        if (isBedrock || player.State == States.Dig || player.State == States.Attack || player.State == States.Pain)
            return;

        var referencePoint = new Vector2(player.transform.position.x, player.transform.position.y);
        var distance = Vector2.Distance(transform.position, referencePoint);

        if (distance < player.TouchingDistance)
        {
            player.IsDigging = true;
            player.State = States.Dig;
            selection.SetActiveColor();
        }
    }

    private void ChangeDestructionDegree()
    {
        var destructionSpriteIndex = (int)Mathf.Floor((maxHealth - Health) * (destructionDegrees.Length / maxHealth));
        destructionSprite.sprite = destructionDegrees[destructionSpriteIndex];
    }

    private void DestroyAttachedTiles()
    {
        var colliders = new List<Collider2D>
        {
            Physics2D.OverlapPoint(new Vector2(transform.position.x + checkingDistanceToDestroyAttachedTiles, transform.position.y)),
            Physics2D.OverlapPoint(new Vector2(transform.position.x - checkingDistanceToDestroyAttachedTiles, transform.position.y)),
            Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y + checkingDistanceToDestroyAttachedTiles)),
            Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y - checkingDistanceToDestroyAttachedTiles))
        };

        foreach (var collider in colliders)
        {
            if (collider == null)
                continue;

            var attachedComponent = collider.GetComponent<AttachedTile>();
            if (attachedComponent != null)
            {
                var stalactite = attachedComponent.GetComponent<Stalactite>();
                var tile = attachedComponent.GetComponent<Tile>();

                if (tile != null && (LevelGeneration.Instanse == null || LevelGeneration.Instanse.IsGenerated))
                    AddResourceToBackpack(tile.ResourceType);

                if (stalactite != null)
                {
                    stalactite.transform.SetParent(null);
                    stalactite.Active(0);
                }
                else
                    Destroy(attachedComponent.gameObject);

            }
        }
    }

    private void AddResourceToBackpack(ResourceTypes resourceType)
    {
        if (resourceType != ResourceTypes.None && player)
            player.GetComponent<Backpack>().Add(resourceType);
    }

    private IEnumerator Shake()
    {
        animation_.Play(shakeAnimationName);
        yield return new WaitForSeconds(shakingTime);
        animation_.Stop(shakeAnimationName);
        tileSprite.transform.localPosition = new Vector3(0, 0, 0);
    }
}
