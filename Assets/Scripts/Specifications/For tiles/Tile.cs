using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public const float Size = 1f;

    private const string shakeAnimationName = "Shake";
    private const float checkingDistanceToAttachedTiles = 0.505f;

    [SerializeField] private float health = 10;
    [SerializeField] private float diggingDifficulty = 1;
    [SerializeField] private float shakingTime = 0.24f;
    [SerializeField] private bool isBedrock = false;
    [SerializeField] private bool destroyAttachedTiles = true;
    [SerializeField] private bool drawFrames = true;
    [SerializeField] private ResourceType resourceType = ResourceType.None;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Sprite[] destructionDegrees;
    [SerializeField] private Sprite[] variants;

    [Space]

    [SerializeField] private SpriteRenderer tileSprite;
    [SerializeField] private SpriteRenderer destructionSprite;

    [Header("Frame Sprites")]
    [SerializeField] private GameObject frameTop;
    [SerializeField] private GameObject frameBottom;
    [SerializeField] private GameObject frameLeft;
    [SerializeField] private GameObject frameRight;

    [Space]

    [SerializeField] private SFX digSFX;
    [SerializeField] private SFX destroySFX;

    private Selection selection;
    private Player player;
    private Animation animation_;

    private float maxHealth;
    private bool framesDrawed = false;

    public float DiggingDifficulty { get => diggingDifficulty; }

    public ResourceType ResourceType { get => resourceType; }

    public float Health
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0)
            {
                digSFX.Play();
                destroySFX.Play();
                AddResourceToBackpack(resourceType);
                DrawFramesOnNeighboringTiles();
                if (destroyAttachedTiles)
                    DestroyAttachedTiles();
                Destroy(gameObject);
            }
            else
            {
                digSFX.Play();
                StartCoroutine(Shake());
                if (destructionDegrees.Length > 0)
                    ChangeDestructionDegree();
            }
        }
    }

    public bool IsBedrock { get => isBedrock; set => isBedrock = value; }

    public static Vector3 GetCenterPositionOfNearestTile(Vector3 position)
    {
        var deltaX = position.x - Mathf.Round(position.x);
        var deltaY = position.y - Mathf.Round(position.y);
        var x = Mathf.Round(position.x) + (deltaX < 0 ? -Tile.Size / 2 : Tile.Size / 2);
        var y = Mathf.Round(position.y) + (deltaY < 0 ? -Tile.Size / 2 : Tile.Size / 2);
        return new Vector3(x, y, 0);
    }

    public void DrawFrames()
    {
        framesDrawed = true;
        if (!drawFrames || GetComponent<AttachedTile>() != null)
            return;
        StartCoroutine(Draw());

        IEnumerator Draw()
        {
            yield return new WaitForSeconds(0.001f);

            var colliders = GetNearestTiles();
            foreach (var collider in colliders)
            {
                if (collider.Value == null || collider.Value.GetComponent<AttachedTile>() != null)
                {
                    switch (collider.Key)
                    {
                        case Direction.Top: frameTop.SetActive(true); break;
                        case Direction.Bottom: frameBottom.SetActive(true); break;
                        case Direction.Left: frameLeft.SetActive(true); break;
                        case Direction.Right: frameRight.SetActive(true); break;
                    }
                }
            }
        }
    }

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
    }

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

    private void FixedUpdate()
    {
        if (!framesDrawed && (LevelGeneration.Instanse == null || LevelGeneration.Instanse.IsGenerated))
            DrawFrames();
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
        var colliders = GetNearestTiles().Values;

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

    private void AddResourceToBackpack(ResourceType resourceType)
    {
        if (resourceType != ResourceType.None && player)
            player.GetComponent<Backpack>().Add(resourceType);
    }

    private void DrawFramesOnNeighboringTiles()
    {
        var neighboringTiles = GetNearestTiles().Values;
        foreach (var neighboringTile in neighboringTiles)
        {
            if (neighboringTile == null)
                continue;

            var tile = neighboringTile.GetComponent<Tile>();

            if (tile == null)
                continue;

            tile.DrawFrames();
        }
    }

    private Dictionary<Direction, Collider2D> GetNearestTiles()
    {
        return new Dictionary<Direction, Collider2D>
        {
            [Direction.Right] = Physics2D.OverlapPoint(new Vector2(transform.position.x + checkingDistanceToAttachedTiles, transform.position.y), groundMask),
            [Direction.Left] = Physics2D.OverlapPoint(new Vector2(transform.position.x - checkingDistanceToAttachedTiles, transform.position.y), groundMask),
            [Direction.Top] = Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y + checkingDistanceToAttachedTiles), groundMask),
            [Direction.Bottom] = Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y - checkingDistanceToAttachedTiles), groundMask)
        };
    }

    private IEnumerator Shake()
    {
        animation_.Play(shakeAnimationName);
        yield return new WaitForSeconds(shakingTime);
        animation_.Stop(shakeAnimationName);
        tileSprite.transform.localPosition = new Vector3(0, 0, 0);
    }
}
