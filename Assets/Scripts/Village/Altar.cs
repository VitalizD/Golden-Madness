using UnityEngine;

public class Altar : MonoBehaviour
{
    private PressActionKey pressActionKey;
    private Building building;
    private TriggerZone triggerZone;
    private AltarComponents altarComponents;

    private bool builded = false;

    public void Build()
    {
        builded = true;
        CheckArtifactPart();
    }

    public void CheckArtifactPart()
    {
        //var artifactPartFounded = true;
        var artifactPartFounded = PlayerPrefs.GetInt(PlayerPrefsKeys.ArtifactPartFounded, 0) != 0;
        if (artifactPartFounded)
        {
            pressActionKey.SetActive(true);
            if (triggerZone.IsTriggered)
                pressActionKey.Show();
        }
    }

    private void Awake()
    {
        pressActionKey = GetComponent<PressActionKey>();
        building = GetComponent<Building>();
        triggerZone = GetComponent<TriggerZone>();
        altarComponents = GetComponent<AltarComponents>();
    }

    private void Start()
    {
        //PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentChapter, 1);
        if (building.Level > 0)
            Build();
    }

    private void Update()
    {
        if (builded && triggerZone.IsTriggered && Input.GetKeyDown(KeyCode.E))
        {
            //var artifactPartFounded = true;
            var artifactPartFounded = PlayerPrefs.GetInt(PlayerPrefsKeys.ArtifactPartFounded, 0) != 0;
            if (artifactPartFounded)
                NextChapter();
        }
    }

    private void NextChapter()
    {
        var currentChapter = PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentChapter, 1);
        altarComponents.ActivatePart(currentChapter);
        PlayerPrefs.SetInt(PlayerPrefsKeys.ArtifactPartFounded, 0);
        pressActionKey.SetActive(false);

        TextMessagesQueue.Instanse.Add($"<color=green>{currentChapter}</color>/3 частей установлено", null, 2.5f);
        TextMessagesQueue.Instanse.Add($"Пещера обновлена", null, 1.5f);
        TextMessagesQueue.Instanse.Add("Благодарим Вас за прохождение демо версии GOLDEN MADNESS!", null, 5f);

        //if (currentChapter < 3)
        //    PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentChapter, ++currentChapter);
    }
}
