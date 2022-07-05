using UnityEngine;
using System.Collections;

public class PlayerAttackPoint : MonoBehaviour
{
    private const string traceAnimationName = "Show";

    [SerializeField] private GameObject tracePrefab;
    [SerializeField] private float traceZPosition = -6f;

    public void PlayTraceAnimation()
    {
        var trace = Instantiate(tracePrefab, new Vector3(transform.position.x, transform.position.y, traceZPosition), Quaternion.identity);
        trace.transform.localScale = transform.parent.localScale;

        var animation = trace.GetComponent<Animation>();
        animation.Play(traceAnimationName);
        StartCoroutine(DestroyObject(trace, animation.GetClip(traceAnimationName).length));
    }

    private IEnumerator DestroyObject(GameObject obj, float afterTime)
    {
        yield return new WaitForSeconds(afterTime);
        Destroy(obj);
    }
}
