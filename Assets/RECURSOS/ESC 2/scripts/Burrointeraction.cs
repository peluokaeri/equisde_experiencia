using UnityEngine;
using UnityEngine.UI;

public class BurroInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject instruccionCanvas;
    public Image eImage;

    [Header("Dialogo")]
    public SubtitleController subtitleController;
    public DialogueData dialogueBurro;

    [Header("Animacion")]
    public Animator animator;

    [Header("Sonido")]
    public AudioSource audioSource;

    private bool playerInside = false;
    private bool used = false;

    void Start()
    {
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        // No tocamos eImage en Start para no interferir con otros scripts

        if (animator != null)
            animator.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        // Solo mostramos E si no hay dialogo activo
        if (!subtitleController.IsDialogueActive)
            eImage.enabled = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        // Solo ocultamos E si somos nosotros quien la mostramos
        if (!used)
            eImage.enabled = false;
    }

    void Update()
    {
        if (!playerInside || used) return;

        // Actualiza visibilidad si el dialogo cambia mientras estamos dentro
        eImage.enabled = !subtitleController.IsDialogueActive;

        if (subtitleController.IsDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            used = true;
            eImage.enabled = false;

            if (animator != null)
            {
                animator.enabled = true;
                animator.SetTrigger("Play");
            }

            if (subtitleController != null && dialogueBurro != null)
                subtitleController.PlayDialogue(dialogueBurro);

            if (audioSource != null)
                audioSource.Play();
        }
    }
}