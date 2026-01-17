using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTriggerAvion : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData dialogueAvion;
    public Animator avionAnimator;
    public AudioSource audioSource;

    [Header("UI")]
    public GameObject instruccionCanvas;
    public Image eImage;

    private bool playerInside = false;
    private bool used = false;

    void Start()
    {
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        eImage.enabled = false;

        // Animación apagada al inicio
        if (avionAnimator != null)
            avionAnimator.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        if (!subtitleController.IsDialogueActive)
            eImage.enabled = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        eImage.enabled = false;
    }

    void Update()
    {
        if (!playerInside) return;
        if (used) return;
        if (subtitleController.IsDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            used = true;
            eImage.enabled = false;

            // ▶️ Animación
            if (avionAnimator != null)
            {
                avionAnimator.enabled = true;
                avionAnimator.SetTrigger("Play");
            }

            // 🔊 Sonido
            if (audioSource != null)
                audioSource.Play();

            // 💬 Diálogo
            subtitleController.PlayDialogue(dialogueAvion);
        }
    }
}
