using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public SubtitleController subtitleController;
    public DialogueData dialogue;
    public FirstPlayer player;

    [Header("Luz")]
    public GameObject luz;
    public float luzFadeTime = 2f;
    public float maxEmission = 3f;

    private bool triggered = false;

    private Renderer luzRenderer;
    private Material luzMaterial;
    private Animator luzAnimator;

    private void Awake()
    {
        // Setup luz
        if (luz != null)
        {
            luzRenderer = luz.GetComponent<Renderer>();
            luzAnimator = luz.GetComponent<Animator>();

            if (luzRenderer != null)
            {
                // Instanciamos material para no afectar otros objetos
                luzMaterial = luzRenderer.material;

                Color c = luzMaterial.color;
                c.a = 0f;
                luzMaterial.color = c;

                luzMaterial.SetColor("_EmissionColor", Color.black);
            }

            if (luzAnimator != null)
                luzAnimator.enabled = false;

            luz.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 🔒 Bloquear movimiento durante el dialogo
        if (player != null)
            player.canMove = false;

        subtitleController.PlayDialogue(dialogue);

        StartCoroutine(WaitForDialogueEnd());
    }

    IEnumerator WaitForDialogueEnd()
    {
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        // ✅ Libera el movimiento apenas termina el dialogo
        if (player != null)
            player.canMove = true;

        // 🌕 Activar aparición de luz
        if (luz != null)
            StartCoroutine(FadeInLuz());
    }

    IEnumerator FadeInLuz()
    {
        luz.SetActive(true);

        float t = 0f;

        while (t < luzFadeTime)
        {
            t += Time.deltaTime;
            float k = t / luzFadeTime;

            // Alpha
            Color c = luzMaterial.color;
            c.a = Mathf.Lerp(0f, 1f, k);
            luzMaterial.color = c;

            // Emission
            Color emission = Color.white * Mathf.Lerp(0f, maxEmission, k);
            luzMaterial.SetColor("_EmissionColor", emission);

            yield return null;
        }

        // 🔥 Reproducir animación
        if (luzAnimator != null)
            luzAnimator.enabled = true;
    }
}