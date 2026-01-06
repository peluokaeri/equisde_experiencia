using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerIntro : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData introDialogue;
    public FirstPlayer player;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 🔒 Bloquear movimiento
        if (player != null)
            player.canMove = false;

        subtitleController.PlayDialogue(introDialogue);

        // ⏳ Esperar fin del diálogo
        StartCoroutine(WaitForDialogueEnd());
    }

    IEnumerator WaitForDialogueEnd()
    {
        // Espera hasta que el SubtitleController termine
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        // 🔓 Volver a habilitar movimiento
        if (player != null)
            player.canMove = true;
    }
}
