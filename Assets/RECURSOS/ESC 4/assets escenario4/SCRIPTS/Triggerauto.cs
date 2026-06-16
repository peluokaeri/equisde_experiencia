using UnityEngine;

public class TriggerAuto : MonoBehaviour
{
    [Header("Auto")]
    public Animator autoAnimator;
    public AudioSource autoAudio;

    [Header("Dialogo")]
    public SubtitleController subtitleController;
    public DialogueData dialogueAuto;

    [Header("Habilitar TriggerLuz al terminar dialogo")]
    public TriggerLuz triggerLuz;

    private bool activado = false;

    void Start()
    {
        if (autoAnimator != null)
            autoAnimator.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (activado) return;
        if (!other.CompareTag("Player")) return;

        activado = true;

        if (autoAnimator != null)
            autoAnimator.enabled = true;

        if (autoAudio != null)
            autoAudio.Play();

        if (subtitleController != null && dialogueAuto != null)
        {
            subtitleController.PlayDialogue(dialogueAuto);
            StartCoroutine(EsperarDialogoYHabilitar());
        }
    }

    System.Collections.IEnumerator EsperarDialogoYHabilitar()
    {
        yield return new WaitUntil(() => subtitleController.IsDialogueActive);
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        if (triggerLuz != null)
            triggerLuz.Habilitar();
    }
}