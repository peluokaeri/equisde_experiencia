using UnityEngine;

public class TriggerAula2 : MonoBehaviour
{
    [Header("Dialogo")]
    public SubtitleController subtitleController;
    public DialogueData dialogueAula2;          // Dialogo al entrar al aula

    [Header("Luz y banco")]
    public GameObject luz2;                     // Luz que ilumina el banco (desactivada al inicio)
    public AudioSource sonidoLuz;               // Sonido al aparecer la luz

    [Header("Interaccion banco")]
    public SillaBanco2 sillaBanco2;             // Script del banco del 2do aula

    private bool triggered = false;

    void Start()
    {
        // La luz empieza apagada
        if (luz2 != null)
            luz2.SetActive(false);

        // El banco no se puede usar hasta que termine el dialogo
        if (sillaBanco2 != null)
            sillaBanco2.habilitado = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // Dispara dialogo de entrada
        if (subtitleController != null && dialogueAula2 != null)
            subtitleController.PlayDialogue(dialogueAula2);

        // Espera a que el dialogo termine para activar la luz
        StartCoroutine(EsperarDialogo());
    }

    private System.Collections.IEnumerator EsperarDialogo()
    {
        // Espera a que empiece el dialogo
        yield return new WaitUntil(() => subtitleController.IsDialogueActive);

        // Espera a que termine
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        // Activa la luz con sonido
        if (luz2 != null)
            luz2.SetActive(true);

        if (sonidoLuz != null)
            sonidoLuz.Play();

        // Habilita el banco
        if (sillaBanco2 != null)
            sillaBanco2.habilitado = true;
    }
}