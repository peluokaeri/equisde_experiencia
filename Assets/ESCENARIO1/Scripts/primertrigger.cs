using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData dialogue;
    public FirstPlayer player;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 🔒 Bloquear SOLO movimiento (la cámara sigue)
        if (player != null)
            player.canMove = false;

        subtitleController.PlayDialogue(dialogue);

        // ⏳ Esperar a que termine el diálogo
        StartCoroutine(WaitForDialogueEnd());
    }

    IEnumerator WaitForDialogueEnd()
    {
        // Espera hasta que el diálogo termine
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        // 🔓 Habilitar movimiento
        if (player != null)
            player.canMove = true;
    }
}
