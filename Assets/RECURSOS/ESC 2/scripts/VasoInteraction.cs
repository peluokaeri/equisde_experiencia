using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VasoInteraction : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData vasoDialogue;
    public Animator vasoAnimator;

    [Header("Audio")]
    public AudioSource audioSource;   // AudioSource del vaso
    public AudioClip enterSound;      // Sonido al entrar al collider

    private bool triggered = false;

    private void Start()
    {
        // 🚫 Evitar que la animación se reproduzca sola
        if (vasoAnimator != null)
            vasoAnimator.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        // ❌ No disparar si hay otro diálogo activo
        if (subtitleController.IsDialogueActive)
            return;

        triggered = true;

        // 🔊 Reproducir sonido al entrar
        if (audioSource != null && enterSound != null)
        {
            audioSource.PlayOneShot(enterSound);
        }

        // ▶ Activar animación del vaso
        if (vasoAnimator != null)
        {
            vasoAnimator.enabled = true;
            vasoAnimator.Play(0, 0, 0f);
        }

        // 🗣 Reproducir diálogo
        subtitleController.PlayDialogue(vasoDialogue);
    }
}
