using System.Collections;
using UnityEngine;

public class Puerta1AfterDialogue : MonoBehaviour
{
    public SubtitleController subtitleController;
    public Animator animator;
    public AudioSource audioSource;

    private bool used = false;

    void Start()
    {
        if (animator != null)
            animator.enabled = false;

        StartCoroutine(WaitForDialogueProperly());
    }

    IEnumerator WaitForDialogueProperly()
    {
        // 1️⃣ Esperar a que el diálogo realmente empiece
        yield return new WaitUntil(() => subtitleController.IsDialogueActive);

        // 2️⃣ Esperar a que termine
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        if (used) yield break;
        used = true;

        // 3️⃣ Activar animación
        animator.enabled = true;
        animator.SetTrigger("Play");

        // 4️⃣ Reproducir sonido
        if (audioSource != null)
            audioSource.Play();
    }
}