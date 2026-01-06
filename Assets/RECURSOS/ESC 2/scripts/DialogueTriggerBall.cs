using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerBall : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData ballDialogue;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        subtitleController.PlayDialogue(ballDialogue);
    }
}
