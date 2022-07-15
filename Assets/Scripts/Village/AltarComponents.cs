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
            case 1: firstPart.SetActive(true); break;
            case 2: secondPart.SetActive(true); break;
            case 3: thirdPart.SetActive(true); break;
        }
    }

}
