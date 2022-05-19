using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeWindow : MonoBehaviour
{
    private Animation animation_;
    private TextMeshProUGUI title;
    private TextMeshProUGUI description;
    private TextMeshProUGUI count;
    private TextMeshProUGUI action;
    private Image resourceIcon;

    private void Awake()
    {
        animation_ = GetComponent<Animation>();

        var canvas = transform.GetChild(0);
        title = canvas.GetChild(0).GetComponent<TextMeshProUGUI>();
        description = canvas.GetChild(1).GetComponent<TextMeshProUGUI>();
        resourceIcon = canvas.GetChild(2).GetComponent<Image>();
        count = canvas.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        action = canvas.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
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

    // Назначен на ключ в анимации "Hide"
    private void OnEndAnimation()
    {
        gameObject.SetActive(false);
    }
}
