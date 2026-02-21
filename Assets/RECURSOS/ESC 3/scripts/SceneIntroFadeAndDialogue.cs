using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneIntroFadeAndDialogue : MonoBehaviour
{
    [Header("Fade")]
    public Image whiteImage;
    public float fadeDuration = 3f;

    [Header("Dialogue")]
    public SubtitleController subtitleController;
    public DialogueData introDialogue;

    IEnumerator Start()
    {
        // Esperar a que el SubtitleController exista y esté listo
        yield return new WaitUntil(() => subtitleController != null);

        // Esperar 1 frame extra para asegurar inicialización interna
        yield return null;

        // 🎙 Iniciar diálogo
        subtitleController.PlayDialogue(introDialogue);

        // 🌫 Iniciar fade en paralelo
        StartCoroutine(FadeOutWhite());
    }

    IEnumerator FadeOutWhite()
    {
        Color color = whiteImage.color;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - (t / fadeDuration);
            color.a = alpha;
            whiteImage.color = color;
            yield return null;
        }

        color.a = 0f;
        whiteImage.color = color;
    }
}