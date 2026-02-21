using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examen1Trigger : MonoBehaviour
{
    [Header("Dialogue")]
    public SubtitleController subtitleController;
    public DialogueData examen1Dialogue;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        // Evita que se active dos veces
        triggered = true;

        // Evita superponer diálogos
        if (!subtitleController.IsDialogueActive)
        {
            subtitleController.PlayDialogue(examen1Dialogue);
        }
    }
}