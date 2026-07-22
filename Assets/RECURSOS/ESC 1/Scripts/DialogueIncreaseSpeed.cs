using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueIncreaseSpeed : MonoBehaviour
{
    [Header("Dialogue")]
    public SubtitleController subtitleController;
    public DialogueData dialogueDI2;

    [Header("Velocidad de lectura (typing)")]
    // Mas alto = las letras aparecen mas lento = mas tiempo para leer
    public float typingSpeedLento = 0.06f;

    [Header("Movement")]
    public ForwardMovementOnLight movementScript; // el script que mueve al player
    public float newSpeed = 1.2f;                  // velocidad al reanudar

    [Header("Cordon")]
    public CordonController cordonController;      // el cordon que se mueve

    private bool triggered = false;
    private float typingSpeedOriginal;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // ⏸️ Pausa el avance del jugador mientras lee
        if (movementScript != null)
            movementScript.Pausar();

        // 🐢 Baja la velocidad de tipeo (mas tiempo para leer)
        if (subtitleController != null)
        {
            typingSpeedOriginal = subtitleController.typingSpeed;
            subtitleController.typingSpeed = typingSpeedLento;
        }

        // 🪢 Arranca el cordon junto con el dialogo 2
        if (cordonController != null)
            cordonController.PlayCordon();

        // ▶️ Reproducir diálogo
        subtitleController.PlayDialogue(dialogueDI2);

        // ▶️ Esperar a que termine
        StartCoroutine(WaitForDialogueEnd());
    }

    IEnumerator WaitForDialogueEnd()
    {
        // Espera mientras el diálogo esté activo
        while (subtitleController.IsDialogueActive)
        {
            yield return null;
        }

        // 🔄 Restaura la velocidad de tipeo original
        if (subtitleController != null)
            subtitleController.typingSpeed = typingSpeedOriginal;

        // 🚀 Reanuda el avance con la nueva velocidad
        if (movementScript != null)
        {
            movementScript.speed = newSpeed;
            movementScript.Reanudar();
        }
    }
}