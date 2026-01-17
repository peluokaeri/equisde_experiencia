using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTriggerIntro : MonoBehaviour
{
    [Header("Dialogue")]
    public SubtitleController subtitleController;
    public DialogueData introDialogue;
    public FirstPlayer player;

    [Header("White Fade")]
    public Image whiteFadeImage;
    public float fadeDuration = 6f; // tiempo total del desvanecimiento

    private bool triggered = false;

    private void Start()
    {
        if (whiteFadeImage != null)
        {
            Color c = whiteFadeImage.color;
            c.a = 1f;
            whiteFadeImage.color = c;
            whiteFadeImage.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 🔒 Bloquear movimiento
        if (player != null)
            player.canMove = false;

        subtitleController.PlayDialogue(introDialogue);

        // ▶ Fade blanco por tiempo
        if (whiteFadeImage != null)
            StartCoroutine(FadeWhiteOverTime());

        // ⏳ Esperar fin del diálogo
        StartCoroutine(WaitForDialogueEnd());
    }

    IEnumerator FadeWhiteOverTime()
    {
        float t = 0f;
        Color c = whiteFadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            whiteFadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        whiteFadeImage.color = c;
    }

    IEnumerator WaitForDialogueEnd()
    {
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        // 🔓 Volver a habilitar movimiento
        if (player != null)
            player.canMove = true;
    }
}
