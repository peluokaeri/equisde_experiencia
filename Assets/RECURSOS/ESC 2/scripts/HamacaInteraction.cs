using UnityEngine;
using UnityEngine.UI;

public class DialogueTriggerHamaca : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData dialogueHamaca;
    public Animator hamacaAnimator;

    [Header("UI")]
    public GameObject instruccionCanvas;
    public Image eImage;

    [Header("Audio")]
    public AudioSource audioSource; // 🔊 Sonido al tocar la E

    private bool playerInside = false;
    private bool used = false;

    void Start()
    {
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        eImage.enabled = false;

        // 🔒 Evitar que la animación arranque sola
        if (hamacaAnimator != null)
            hamacaAnimator.enabled = false;
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

            // 🔹 Ocultar la E
            eImage.enabled = false;

            // 🔊 Sonido al interactuar
            if (audioSource != null)
                audioSource.Play();

            // ▶️ Activar animator SOLO al tocar la E
            if (hamacaAnimator != null)
            {
                hamacaAnimator.enabled = true;
                hamacaAnimator.SetTrigger("Play");
            }

            // 🗣 Diálogo
            subtitleController.PlayDialogue(dialogueHamaca);
        }
    }
}
