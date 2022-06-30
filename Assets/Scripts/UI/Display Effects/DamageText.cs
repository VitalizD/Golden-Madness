using UnityEngine;
using System.Collections;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private GameObject damageTextPrefab;

    private readonly string[] animationNames = new[] { "Jump Left", "Jump Right", "Jump Top" };

    public void ShowDamage(int value, Vector2 position)
    {
        var damageText = Instantiate(damageTextPrefab, position, Quaternion.identity, transform);
        damageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
        var animation = damageText.GetComponent<Animation>();
        var randomClip = animationNames[Random.Range(0, animationNames.Length)];
        animation.Play(randomClip);
        StartCoroutine(Destroy(animation.GetClip(randomClip).length, damageText));
    }

    private IEnumerator Destroy(float afterTime, GameObject obj)
    {
        yield return new WaitForSeconds(afterTime);
        Destroy(obj);
    }
}
