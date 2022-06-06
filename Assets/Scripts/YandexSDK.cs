using Agava.YandexGames;
using UnityEngine;

public class YandexSDK : MonoBehaviour
{
    void Start()
    {
        VideoAd.Show();
    }
    void Update()
    {
        InterestialAd.Show();
    }
}
