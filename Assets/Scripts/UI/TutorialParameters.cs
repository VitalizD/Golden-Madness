using UnityEngine;

public class TutorialParameters : MonoBehaviour
{
    [SerializeField] private bool forcedTutorial = false;

    public bool ForcedTutorial { get => forcedTutorial; }
}
