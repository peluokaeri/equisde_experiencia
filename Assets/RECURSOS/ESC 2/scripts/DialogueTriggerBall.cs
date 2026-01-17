using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerBall : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData ballDialogue;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Push Settings")]
    public float pushForce = 3f; // Fuerza leve del empujón

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 🔊 Sonido SIEMPRE
        if (audioSource != null)
            audioSource.Play();

        // 🚀 Empujar al jugador en la dirección que mira
        Rigidbody playerRb = other.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 pushDirection = other.transform.forward;
            playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }

        // 🗣 Diálogo SOLO una vez
        if (triggered) return;

        triggered = true;
        subtitleController.PlayDialogue(ballDialogue);
    }
}
