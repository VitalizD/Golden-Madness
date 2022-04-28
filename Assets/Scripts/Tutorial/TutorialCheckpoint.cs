using UnityEngine;
using System;
using UnityEngine.Events;

public class TutorialCheckpoint : MonoBehaviour
{
    [SerializeField] private string informationText;
    [SerializeField] private InformationWindowHidingConditions condition = InformationWindowHidingConditions.None;
    [SerializeField] private UnityEvent onMakeCondition;

    private Func<bool> funcCondition;

    private bool wasTriggered = false;

    private void Awake()
    {
        funcCondition = GetFuncCondition();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (wasTriggered)
            return;

        InformationWindow.instance?.Show(informationText, funcCondition);
        Player.instanse.Checkpoint = transform.position;
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
                return new Func<bool>(() => { return Input.GetKey(KeyCode.I); });
            case InformationWindowHidingConditions.Press2:
                return new Func<bool>(() => { return Input.GetKey(KeyCode.Alpha2); });
            case InformationWindowHidingConditions.Press3:
                return new Func<bool>(() => { return Input.GetKey(KeyCode.Alpha3); });
            case InformationWindowHidingConditions.Press4:
                return new Func<bool>(() => { return Input.GetKey(KeyCode.Alpha4); });
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
