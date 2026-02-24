using System.Collections;
using UnityEngine;

public class Examen1Trigger : MonoBehaviour
{
    [Header("Dialogue")]
    public SubtitleController subtitleController;
    public DialogueData examen1Dialogue;

    [Header("After Dialogue")]
    public GameObject LUZ1;

    [Header("Sound")]
    public AudioSource audioSource;

    private bool triggered = false;

    private void Start()
    {
        // Luz apagada al inicio
        if (LUZ1 != null)
            LUZ1.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (!subtitleController.IsDialogueActive)
        {
            subtitleController.PlayDialogue(examen1Dialogue);
            StartCoroutine(WaitDialogueEnd());
        }
    }

    IEnumerator WaitDialogueEnd()
    {
        // Esperar a que termine el diálogo
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        // 💡 Activar luz
        if (LUZ1 != null)
            LUZ1.SetActive(true);

        // 🔊 Reproducir sonido
        if (audioSource != null)
            audioSource.Play();
    }
}