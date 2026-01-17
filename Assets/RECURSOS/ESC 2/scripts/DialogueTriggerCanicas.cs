using UnityEngine;
using UnityEngine.UI;

public class DialogueTriggerCanicas : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData dialogueCanicas;
    public Animator canicasAnimator;

    [Header("UI")]
    public GameObject instruccionCanvas; // Canvas compartido
    public Image eImage;                 // Imagen "E"

    [Header("Audio")]
    public AudioSource audioSource;      // Sonido al tocar la E

    private bool playerInside = false;
    private bool used = false;

    void Start()
    {
        // ❗ Asegurarse de que el canvas esté activo
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        eImage.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        // 🔑 Forzar canvas activo
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

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

            // ▶ Animación SOLO al interactuar
            if (canicasAnimator != null)
                canicasAnimator.SetTrigger("Play");

            // ▶ Diálogo
            subtitleController.PlayDialogue(dialogueCanicas);
        }
    }
}
