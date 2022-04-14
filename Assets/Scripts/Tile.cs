using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    [SerializeField] private float health = 10;
    [SerializeField] private float diggingDifficulty = 1;
    [SerializeField] private bool isBedrock = false;
    [SerializeField] private Sprite[] destructionDegrees;

    public UnityEvent OnCannotDig;

    private SpriteRenderer destructionSprite;

    private GameObject selection;
    private float zPosSelection;
    private float selectionMovingSpeed = 20;
    private float maxHealth;
    private Player player;

    public float Health
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0)
            {
                player.IsDigging = false;
                selection.SetActive(false);
                Destroy(gameObject);
            }
            else
                ChangeDestructionDegree();
        }
    }

    public float DiggingDifficulty { get => diggingDifficulty; }

    private void Awake()
    {
        selection = GameObject.Find("Selection");
        zPosSelection = selection.transform.position.z;
        maxHealth = health;
        player = FindObjectOfType<Player>();
        destructionSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        if (isBedrock)
            return;

        selection.SetActive(true);
        player.SetSelectedTile(this);
    }

    private void OnMouseExit()
    {
        selection.SetActive(false);
        player.IsDigging = false;
        player.RemoveSelectedTile();
    }

    private void OnMouseDown()
    {
        CheckDistance();
    }

    private void OnMouseOver()
    {
        selection.transform.position = Vector3.Lerp(
            selection.transform.position,
            new Vector3(transform.position.x, transform.position.y, zPosSelection),
            selectionMovingSpeed * Time.deltaTime);

        if (Input.GetMouseButton(0))
            CheckDistance();
    }

    private void OnMouseUp()
    {
        player.IsDigging = false;
    }

    private void CheckDistance()
    {
        if (isBedrock || !player.IsGrounded || player.State != States.Idle)
        {
            OnCannotDig?.Invoke();
            return;
        }

        var referencePoint = new Vector2(player.transform.position.x, player.transform.position.y);
        var distance = Vector2.Distance(transform.position, referencePoint);

        if (distance < player.TouchingDistance)
        {
            player.IsDigging = true;
            player.State = States.Dig;
        }
    }

    private void ChangeDestructionDegree()
    {
        var destructionSpriteIndex = (int)Mathf.Floor((maxHealth - Health) * (destructionDegrees.Length / maxHealth));
        destructionSprite.sprite = destructionDegrees[destructionSpriteIndex];
    }
}
