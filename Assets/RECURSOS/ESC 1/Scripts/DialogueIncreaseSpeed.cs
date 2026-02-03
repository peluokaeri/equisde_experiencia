using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DialogueIncreaseSpeed : MonoBehaviour
{
    [Header("Dialogue")]
    public SubtitleController subtitleController;
    public DialogueData dialogueDI2;

    [Header("Movement")]
    public ForwardMovementOnLight movementScript; // el script que mueve al player
    public float newSpeed = 1.2f;                  // velocidad aumentada

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

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

        // 🚀 Aumentar velocidad cuando termina
        if (movementScript != null)
        {
            movementScript.speed = newSpeed;
        }
    }
}