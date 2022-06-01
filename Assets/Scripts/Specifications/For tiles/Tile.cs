using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    [SerializeField] private float health = 10;
    [SerializeField] private float diggingDifficulty = 1;
    [SerializeField] private bool isBedrock = false;
    [SerializeField] private bool destroyAttachedTiles = true;
    [SerializeField] private ResourceTypes resourceType = ResourceTypes.None;
    [SerializeField] private Sprite[] destructionDegrees;

    private SpriteRenderer destructionSprite;
    private Selection selection;
    private Player player;

    private float maxHealth;
    private readonly float checkingDistanceToDestroyAttachedTiles = 0.50f;

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
                Destroy(gameObject);
            }
            else if (destructionDegrees.Length > 0)
                ChangeDestructionDegree();
        }
    }

    private void Awake()
    {
        selection = GameObject.Find("Selection").GetComponent<Selection>();
        maxHealth = health;
        destructionSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        player = Player.instanse;
    }

    private void OnMouseEnter()
    {
        if (isBedrock)
            return;

        if (selection != null)
            selection.gameObject.SetActive(true);
        if (player != null)
            player.SetSelectedTile(this);
    }

    private void OnMouseExit()
    {
        StopDigging();
        if (selection != null)
            selection.gameObject.SetActive(false);
        if (player != null)
            player.RemoveSelectedTile();
    }

    private void OnMouseDown()
    {
        CheckDistance();
    }

    private void OnMouseOver()
    {
        if (!isBedrock)
            selection.Move(transform.position);

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
        if (destroyAttachedTiles)
            DestroyAttachedTiles();

        StopDigging();
        if (selection != null)
            selection.gameObject.SetActive(false);
    }

    private void StopDigging()
    {
        if (player != null)
            player.IsDigging = false;
        if (selection != null)
            selection.SetNormalColor();
    }

    private void CheckDistance()
    {
        if (isBedrock || player.State != States.Idle)
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

                if (tile != null)
                    AddResourceToBackpack(tile.ResourceType);

                if (stalactite != null)
                    stalactite.Active(0);
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
}
