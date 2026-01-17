using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTriggerBarbie : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData dialogueBarbie;
    public Animator barbieAnimator;

    [Header("UI")]
    public GameObject instruccionCanvas; // Canvas compartido
    public Image eImage;                 // Imagen "E"

    [Header("Audio")]
    public AudioSource audioSource;      // 🔊 Sonido al tocar la E

    private bool playerInside = false;
    private bool used = false;

    void Start()
    {
        // Aseguramos canvas activo (por si otro script lo apagó)
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        // Ocultamos la E
        eImage.enabled = false;

        // 🔒 MUY IMPORTANTE: animación desactivada al inicio
        if (barbieAnimator != null)
            barbieAnimator.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        // Asegurar canvas activo
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        // Mostrar E solo si no hay diálogo activo
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

            // Ocultamos la E
            eImage.enabled = false;

            // 🔊 Sonido al interactuar
            if (audioSource != null)
                audioSource.Play();

            // ▶️ Activar animación SOLO al tocar la E
            if (barbieAnimator != null)
            {
                barbieAnimator.enabled = true;
                barbieAnimator.SetTrigger("Play");
            }

            // 🗣 Diálogo
            subtitleController.PlayDialogue(dialogueBarbie);
        }
    }
}
