using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeWindow : MonoBehaviour
{
    private const float offsetToolIconY = 0.05f;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private TextMeshProUGUI action;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private Image toolIcon;
    [SerializeField] private Image upgradeIcon;

    private Animation animation_;

    private bool canUpgrade = true;

    private void Awake()
    {
        animation_ = GetComponent<Animation>();
        level.text = "";

        //var canvas = transform.GetChild(0);
        //title = canvas.GetChild(0).GetComponent<TextMeshProUGUI>();
        //description = canvas.GetChild(1).GetComponent<TextMeshProUGUI>();
        //resourceIcon = canvas.GetChild(2).GetComponent<Image>();
        //count = canvas.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        //action = canvas.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        animation_.Play("Show");
    }

    public void Hide()
    {
        animation_.Play("Hide");
    }

    public void SetTitle(string value)
    {
        title.text = value;
    }

    public void SetDescription(string value)
    {
        description.text = value;
    }

    public void SetAction(string value)
    {
        action.text = value;
    }

    public void SetLevel(int value) => level.text = $"��. {value}";

    public void SetRequiredResource(ResourceTypes type, int requiredCount)
    {
        resourceIcon.sprite = VillageController.instanse.GetResourceSprite(type);
        var currentResourcesCount = VillageController.instanse.GetResourcesCount(type);

        string color;
        if (currentResourcesCount >= requiredCount)
            color = "green";
        else
            color = "red";

        count.text = $"<color={color}>{currentResourcesCount}</color> / {requiredCount}";
    }

    public void SetToolIcon(Sprite value) => toolIcon.sprite = value;

    public void SetUpgradeActive(bool value)
    {
        if (value)
        {
            if (!canUpgrade)
            {
                canUpgrade = true;
                toolIcon.transform.position = new Vector2(toolIcon.transform.position.x, toolIcon.transform.position.y + offsetToolIconY);
            }
        }
        else if (canUpgrade)
        {
            canUpgrade = false;
            toolIcon.transform.position = new Vector2(toolIcon.transform.position.x, toolIcon.transform.position.y - offsetToolIconY);
        }
        upgradeIcon.enabled = value;
    }

    public void SetDescriptionAlignFlush() => description.alignment = TextAlignmentOptions.Flush;

    public void SetDescriptionAlignLeft() => description.alignment = TextAlignmentOptions.Left;

    // �������� �� ���� � �������� "Hide"
    private void OnEndAnimation()
    {
        gameObject.SetActive(false);
    }
}
