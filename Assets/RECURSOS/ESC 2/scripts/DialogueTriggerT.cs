using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerTobogan : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData dialogueTobogan;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        // ❌ No disparar si hay otro diálogo activo
        if (subtitleController.IsDialogueActive)
            return;

        triggered = true;

        // 🗣 Reproducir diálogo
        subtitleController.PlayDialogue(dialogueTobogan);
    }
}
