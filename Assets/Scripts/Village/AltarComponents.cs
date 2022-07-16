using UnityEngine;

public class AltarComponents : MonoBehaviour
{
    [SerializeField] private GameObject firstPart;
    [SerializeField] private GameObject secondPart;
    [SerializeField] private GameObject thirdPart;

    public void ActivatePart(int number)
    {
        switch (number)
        {
            case 1: if (firstPart != null) firstPart.SetActive(true); break;
            case 2: if (secondPart != null) secondPart.SetActive(true); break;
            case 3: if (thirdPart != null) thirdPart.SetActive(true); break;
        }
    }

}
