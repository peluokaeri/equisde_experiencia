using UnityEngine;
using System.Collections;

public class PostDialogoEspacio : MonoBehaviour
{
    [Header("Dialogo")]
    public SubtitleController subtitleController;
    public DialogueData dialogueEspacio;

    public DialogueData GetDialogue() => dialogueEspacio;

    [Header("Triggers de pantallas")]
    public GameObject[] triggersPantallas;      // Desactivados al inicio

    [Header("Atraccion final")]
    public GameObject triggerAtraccionFinal;    // Desactivado al inicio

    private bool usado = false;

    // Llamado por TriggerNegro DESPUES de llamar PlayDialogue
    // Mismo patron exacto que Puerta2AfterDialogue
    public void IniciarEspera()
    {
        if (usado) return;
        usado = true;
        StartCoroutine(WaitForDialogueProperly());
    }

    IEnumerator WaitForDialogueProperly()
    {
        // 1 — Esperar a que el dialogo realmente empiece
        yield return new WaitUntil(() => subtitleController.IsDialogueActive);

        // 2 — Esperar a que termine
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        // 3 — Activar triggers de pantallas y atraccion
        foreach (var trigger in triggersPantallas)
        {
            if (trigger != null)
                trigger.SetActive(true);
        }

        if (triggerAtraccionFinal != null)
            triggerAtraccionFinal.SetActive(true);
    }
}