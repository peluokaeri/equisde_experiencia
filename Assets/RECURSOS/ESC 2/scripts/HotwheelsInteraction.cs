using UnityEngine;
using UnityEngine.UI;

public class DialogueTriggerHotWheels : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData dialogueHotWheels;
    public Animator hotWheelsAnimator;

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
        if (hotWheelsAnimator != null)
            hotWheelsAnimator.enabled = false;
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

            // ▶ Animación SOLO al tocar la E
            if (hotWheelsAnimator != null)
            {
                hotWheelsAnimator.enabled = true;
                hotWheelsAnimator.SetTrigger("Play");
            }

            // 🗣 Diálogo
            subtitleController.PlayDialogue(dialogueHotWheels);
        }
    }
}
