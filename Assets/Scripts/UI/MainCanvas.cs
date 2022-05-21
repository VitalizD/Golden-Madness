using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas instanse = null;

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
