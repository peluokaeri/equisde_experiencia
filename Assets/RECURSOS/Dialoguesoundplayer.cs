using UnityEngine;
using System.Collections;

public class DialogueSoundPlayer : MonoBehaviour
{
    [Header("Referencias")]
    public SubtitleController subtitleController;
    public AudioSource audioSource;

    [Header("Configuracion")]
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.5f;

    private bool dialogoActivoAnterior = false;
    private Coroutine fadeCoroutine;
    private float volumenOriginal;

    void Start()
    {
        if (audioSource != null)
        {
            volumenOriginal = audioSource.volume;
            audioSource.volume = 0f;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (subtitleController == null || audioSource == null) return;

        bool dialogoActivo = subtitleController.IsDialogueActive;

        if (dialogoActivo && !dialogoActivoAnterior)
        {
            // Dialogo empezo — fade in
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeVolumen(volumenOriginal, fadeInDuration));
        }
        else if (!dialogoActivo && dialogoActivoAnterior)
        {
            // Dialogo termino — fade out
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeVolumen(0f, fadeOutDuration));
        }

        dialogoActivoAnterior = dialogoActivo;
    }

    private IEnumerator FadeVolumen(float destino, float duracion)
    {
        float origen = audioSource.volume;
        float t = 0f;

        while (t < duracion)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(origen, destino, t / duracion);
            yield return null;
        }

        audioSource.volume = destino;
    }
}