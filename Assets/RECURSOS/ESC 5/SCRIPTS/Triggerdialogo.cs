using UnityEngine;

public class TriggerDialogo : MonoBehaviour
{
    [Header("Dialogo")]
    public SubtitleController subtitleController;
    public DialogueData dialogue;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;
        if (subtitleController.IsDialogueActive) return;

        triggered = true;
        subtitleController.PlayDialogue(dialogue);
    }
}