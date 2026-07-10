using UnityEngine;

public class TriggerDialogoEsc6 : MonoBehaviour
{
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