using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDialogueChangeScene : MonoBehaviour
{
    [Header("Dialogue")]
    public SubtitleController subtitleController;
    public DialogueData finalDialogue;

    [Header("Scene")]
    public string sceneToLoad = "Escenario2";

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // ▶️ Reproducir diálogo final
        subtitleController.PlayDialogue(finalDialogue);

        // ▶️ Esperar fin del diálogo
        StartCoroutine(WaitForDialogueEnd());
    }

    IEnumerator WaitForDialogueEnd()
    {
        while (subtitleController.IsDialogueActive)
        {
            yield return null;
        }

        // 🎬 Cambiar de escena
        SceneManager.LoadScene(sceneToLoad);
    }
}
