using UnityEngine;

public class SpritesStorage : MonoBehaviour
{
    public static SpritesStorage instanse = null;

    [SerializeField] private Sprite goldIcon;
    [SerializeField] private Sprite coalIcon;
    [SerializeField] private Sprite quartzIcon;
    [SerializeField] private Sprite ironIcon;

    public Sprite GoldIcon { get => goldIcon; }

    public Sprite CoalIcon { get => coalIcon; }

    public Sprite QuartzIcon { get => quartzIcon; }

    public Sprite IronIcon { get => ironIcon; }

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
