using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTriggerPerro : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData dialoguePerro;
    public AudioSource barkAudio;

    [Header("UI")]
    public GameObject instruccionCanvas; // Canvas compartido
    public Image eImage;                 // Imagen "E"

    private bool playerInside = false;
    private bool used = false;

    void Start()
    {
        // Asegurar que el canvas esté activo
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        // Ocultar la E al iniciar
        eImage.enabled = false;
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

            // Ocultar E
            eImage.enabled = false;

            // 🔊 Sonido del perro
            if (barkAudio != null)
                barkAudio.Play();

            // 💬 Diálogo del perro
            subtitleController.PlayDialogue(dialoguePerro);
        }
    }
}
