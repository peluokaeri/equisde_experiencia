using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalTrigger : MonoBehaviour
{
    [Header("References")]
    public SubtitleController subtitleController;
    public DialogueData finalDialogue;

    [Header("Scene")]
    public string nextSceneName = "escenario2";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // ▶️ Reproducir diálogo final
        subtitleController.PlayDialogue(finalDialogue);

        // ⏳ Esperar a que termine
        StartCoroutine(WaitForDialogueAndChangeScene());
    }

    private System.Collections.IEnumerator WaitForDialogueAndChangeScene()
    {
        // Esperamos a que arranque
        yield return new WaitUntil(() => subtitleController.IsDialogueActive);

        // Esperamos a que termine
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        // 🔁 Cambiar de escena
        SceneManager.LoadScene(nextSceneName);
    }
}
