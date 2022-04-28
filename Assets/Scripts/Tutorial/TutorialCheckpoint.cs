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
            default: return null;
        }
    }

    private void Update()
    {
        if (funcCondition != null && funcCondition())
        {
            onMakeCondition?.Invoke();
            Destroy(gameObject);
        }
    }
}
