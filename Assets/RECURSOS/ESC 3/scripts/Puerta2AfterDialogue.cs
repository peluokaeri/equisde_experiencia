using System.Collections;
using UnityEngine;

public class Puerta2AfterDialogue : MonoBehaviour
{
    public SubtitleController subtitleController;
    public Animator animator;
    public AudioSource audioSource;

    private bool used = false;

    void Start()
    {
        if (animator != null)
            animator.enabled = false;
    }

    // Llamado por ExamenManager cuando termina el examen
    public void IniciarEspera()
    {
        StartCoroutine(WaitForDialogueProperly());
    }

    IEnumerator WaitForDialogueProperly()
    {
        // 1 — Esperar a que el dialogo realmente empiece
        yield return new WaitUntil(() => subtitleController.IsDialogueActive);

        // 2 — Esperar a que termine
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        if (used) yield break;
        used = true;

        // 3 — Activar animacion
        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger("Examen2puert");
        }

        // 4 — Reproducir sonido
        if (audioSource != null)
            audioSource.Play();
    }
}