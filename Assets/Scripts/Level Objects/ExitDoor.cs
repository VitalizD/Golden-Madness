using UnityEngine;
using Agava.YandexGames;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private float fadeSpeed = 0.7f;
    [SerializeField] private string nameLevelPrefix = "Level";
    [SerializeField] private string nextScene = "";

    private Teleporter teleporter;
    private SceneChanger sceneChanger;
    private TriggerZone trigger;
    private PressActionKey pressActionKey;

    public bool CanBeUsed
    {
        get => canBeUsed;
        set
        {
            canBeUsed = value;
            pressActionKey.SetActive(value);
        }
    }

    private void Awake()
    {
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.BlackFilterTag).GetComponent<Teleporter>();
        sceneChanger = GetComponent<SceneChanger>();
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && trigger.IsTriggered && canBeUsed && teleporter.State == Teleporter.States.Stayed) 
        {
            var currentLevel = PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentLevelNumber, 1);
            var currentChapter = PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentChapter, 1);
            ++currentLevel;
            PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentLevelNumber, currentLevel);

            void action()
            {
                if (nextScene == "")
                {
                    if (currentLevel < 6)
                    {
                        var backpack = Player.Instanse.GetComponent<Backpack>();
                        ResourcesSaver.AddInVillage(backpack.GetAll());
                        backpack.Clear();
                        //sceneChanger.ChangeScene($"{nameLevelPrefix} {currentChapter}.{currentLevel}");
                        sceneChanger.ChangeScene($"{nameLevelPrefix} 1.{currentLevel}");
                    }
                    else
                        sceneChanger.ChangeScene(ServiceInfo.VillageScene);
                }
                else
                    sceneChanger.ChangeScene(nextScene);
            }

            CanBeUsed = false;
            ServiceInfo.CheckpointConditionDone = true; // Для обучения
            Player.Instanse.Save();
            teleporter.Go(action, fadeSpeed);

            InterestialAd.Show();
        }
    }
}
