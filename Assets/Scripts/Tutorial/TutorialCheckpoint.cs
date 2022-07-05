using UnityEngine;
using System;
using UnityEngine.Events;

public class TutorialCheckpoint : MonoBehaviour
{
    [SerializeField] private string informationText = "";
    [SerializeField] private string playerWords = "";
    [SerializeField] private float timeHidingDialogWindow = 4f;
    [SerializeField] private InformationWindowHidingConditions condition = InformationWindowHidingConditions.None;
    [SerializeField] private UnityEvent onMakeCondition;
    [SerializeField] private UnityEvent onTriggered;

    private Func<bool> funcCondition;

    private bool wasTriggered = false;

    private void Awake()
    {
        funcCondition = GetFuncCondition();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (wasTriggered || collision.GetComponent<Player>() == null)
            return;

        onTriggered?.Invoke();
        ServiceInfo.CheckpointConditionDone = false;

        if (informationText != "" && InformationWindow.instance)
            InformationWindow.instance.Show(informationText, funcCondition);

        Player.Instanse.Checkpoint = transform.position;

        if (playerWords != "")
            Player.Instanse.Say(playerWords, timeHidingDialogWindow);

        if (funcCondition == null)
            Destroy(gameObject);
        else
            wasTriggered = true;
    }

    private Func<bool> GetFuncCondition()
    {
        switch (condition)
        {
            case InformationWindowHidingConditions.Press1:
                return new Func<bool>(() => { return Input.GetKey(KeyCode.Alpha1); });
            case InformationWindowHidingConditions.PressI:
                return new Func<bool>(() => { return Input.GetKey(KeyCode.Tab); });
            case InformationWindowHidingConditions.Press2:
                return new Func<bool>(() => { return Input.GetKey(KeyCode.Alpha2); });
            case InformationWindowHidingConditions.Press3:
                return new Func<bool>(() => { return Input.GetKey(KeyCode.Alpha3); });
            case InformationWindowHidingConditions.Press4:
                return new Func<bool>(() => { return Input.GetKey(KeyCode.Alpha4); });
            case InformationWindowHidingConditions.PressENearMinecart:
                return new Func<bool>(() => { return ServiceInfo.CheckpointConditionDone; });
            case InformationWindowHidingConditions.PressENearHay:
                return new Func<bool>(() => { return ServiceInfo.CheckpointConditionDone; });
            case InformationWindowHidingConditions.PressENearChest:
                return new Func<bool>(() => { return ServiceInfo.CheckpointConditionDone; });
            case InformationWindowHidingConditions.PressENearForge:
                return new Func<bool>(() => { return ServiceInfo.CheckpointConditionDone; });
            case InformationWindowHidingConditions.PressENearCave:
                return new Func<bool>(() => { return ServiceInfo.CheckpointConditionDone; });
            default: return null;
        }
    }

    private void Update()
    {
        if (wasTriggered && funcCondition != null && funcCondition())
        {
            onMakeCondition?.Invoke();
            Destroy(gameObject);
        }
    }
}
