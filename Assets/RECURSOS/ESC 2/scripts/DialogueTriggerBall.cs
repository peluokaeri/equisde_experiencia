using UnityEngine;

public class DialogueTriggerBall : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData ballDialogue;

    [Header("Audio")]
    public AudioSource pelotaAudio;

    [Header("Force")]
    public float pushForce = 3f;

    private bool triggered = false;

    private void Start()
    {
        // Asegurarnos que el audio no suene solo
        if (pelotaAudio != null)
            pelotaAudio.playOnAwake = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // 🔊 Sonido DESDE la pelota (la sigue siempre)
        if (pelotaAudio != null)
            pelotaAudio.Play();

        // 🧍‍♂️ Empujón leve en dirección a donde mira el player
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 pushDir = collision.transform.forward;
            rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
        }

        // 🗣 Diálogo (solo la primera vez)
        if (!triggered && !subtitleController.IsDialogueActive)
        {
            triggered = true;
            subtitleController.PlayDialogue(ballDialogue);
        }
    }
}
