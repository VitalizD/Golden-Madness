using UnityEngine;
using System;

public class TutorialCheckpoint : MonoBehaviour
{
    [SerializeField] private string informationText;
    [SerializeField] private InformationWindowHidingConditions informationWindowHidingCondition = InformationWindowHidingConditions.None;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        InformationWindow.instance?.Show(informationText, GetFuncCondition());
        Player.instanse.Checkpoint = transform.position;
        Destroy(gameObject);
    }

    private Func<bool> GetFuncCondition()
    {
        switch (informationWindowHidingCondition)
        {
            case InformationWindowHidingConditions.Press1:
                return new Func<bool>(() => { return Input.GetKey(KeyCode.Alpha1); });
            default: return null;
        }
    }
}
