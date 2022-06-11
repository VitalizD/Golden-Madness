using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [SerializeField] private RoomDirection type;

    public RoomDirection Type { get => type; }

    public void Remove() => Destroy(gameObject);
}
