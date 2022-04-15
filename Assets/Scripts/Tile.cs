using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    [SerializeField] private float health = 10;
    [SerializeField] private float diggingDifficulty = 1;
    [SerializeField] private bool isBedrock = false;
    [SerializeField] private Sprite[] destructionDegrees;

    private SpriteRenderer destructionSprite;
    private Selection selection;

    private float maxHealth;
    private Player player;

    public float DiggingDifficulty { get => diggingDifficulty; }

    public float Health
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0)
            {
                StopDigging();
                selection.gameObject.SetActive(false);
                Destroy(gameObject);
            }
            else
                ChangeDestructionDegree();
        }
    }

    private void Awake()
    {
        selection = GameObject.Find("Selection").GetComponent<Selection>();
        maxHealth = health;
        player = FindObjectOfType<Player>();
        destructionSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        if (isBedrock)
            return;

        selection.gameObject.SetActive(true);
        player.SetSelectedTile(this);
    }

    private void OnMouseExit()
    {
        StopDigging();
        selection.gameObject.SetActive(false);
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

    private void StopDigging()
    {
        player.IsDigging = false;
        selection.SetNormalColor();
    }

    private void CheckDistance()
    {
        if (isBedrock || !player.IsGrounded || player.State != States.Idle)
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
}
