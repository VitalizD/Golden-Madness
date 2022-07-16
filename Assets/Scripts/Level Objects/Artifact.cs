using UnityEngine;
using System.Collections;

public class Artifact : MonoBehaviour
{
    private const string takeAnimationName = "Take";

    [SerializeField] private string message = "Часть артефакта найдена\n<color=#e34534>Вы чувствуете ауру злобных сил...";
    [SerializeField] private float delayBeforeMessage = 1f;
    [SerializeField] private Light lighting;
    [SerializeField] private SFX artifactPickUpSFX;

    private Animation anim;
    private BoxCollider2D collider2d;

    private void Awake()
    {
        collider2d = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            collider2d.enabled = false;
            lighting.enabled = false;
            anim.Play(takeAnimationName);
            artifactPickUpSFX.Play();
            PlayerPrefs.SetInt(PlayerPrefsKeys.ArtifactPartFounded, 1);

            StartCoroutine(ShowMessage());
        }
    }

    private IEnumerator ShowMessage()
    {
        yield return new WaitForSeconds(delayBeforeMessage);
        TextMessagesQueue.Instanse.Add(message, null, 5f);
    }
}
