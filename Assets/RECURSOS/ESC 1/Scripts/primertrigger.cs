using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public SubtitleController subtitleController;
    public DialogueData dialogue;
    public FirstPlayer player;

    [Header("Confirmar Menu")]
    public GameObject confirmarMenu;
    public Graphic confirmText;
    public Button confirmButton;

    [Header("Confirm Timings")]
    public float textFadeTime = 1.5f;
    public float buttonFadeTime = 1f;
    public float buttonBlinkSpeed = 0.25f;

    [Header("Luz")]
    public GameObject luz;
    public float luzFadeTime = 2f;
    public float maxEmission = 3f;

    private bool triggered = false;
    private Coroutine blinkRoutine;
    private Graphic buttonGraphic;

    private Renderer luzRenderer;
    private Material luzMaterial;
    private Animator luzAnimator;

    private void Awake()
    {
        confirmarMenu.SetActive(false);

        buttonGraphic = confirmButton.GetComponent<Graphic>();

        SetAlpha(confirmText, 0f);
        SetAlpha(buttonGraphic, 0f);

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

        // 🔒 Bloquear movimiento
        if (player != null)
            player.canMove = false;

        subtitleController.PlayDialogue(dialogue);

        StartCoroutine(WaitForDialogueEnd());
    }

    IEnumerator WaitForDialogueEnd()
    {
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        // Mouse libre
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Bloquear rotación
        if (player != null)
            player.enabled = false;

        confirmarMenu.SetActive(true);

        yield return StartCoroutine(FadeGraphic(confirmText, 0f, 1f, textFadeTime));
        yield return StartCoroutine(FadeGraphic(buttonGraphic, 0f, 1f, buttonFadeTime));

        blinkRoutine = StartCoroutine(BlinkButton());
    }

    IEnumerator FadeGraphic(Graphic graphic, float from, float to, float duration)
    {
        float t = 0f;
        SetAlpha(graphic, from);

        while (t < duration)
        {
            t += Time.deltaTime;
            SetAlpha(graphic, Mathf.Lerp(from, to, t / duration));
            yield return null;
        }

        SetAlpha(graphic, to);
    }

    IEnumerator BlinkButton()
    {
        while (true)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * buttonBlinkSpeed;
                SetAlpha(buttonGraphic, Mathf.Lerp(1f, 0.25f, t));
                yield return null;
            }

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * buttonBlinkSpeed;
                SetAlpha(buttonGraphic, Mathf.Lerp(0.25f, 1f, t));
                yield return null;
            }
        }
    }

    void SetAlpha(Graphic g, float alpha)
    {
        Color c = g.color;
        c.a = alpha;
        g.color = c;
    }

    // 🔘 Botón Confirmar
    public void OnConfirmPressed()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        confirmarMenu.SetActive(false);

        // Volver a gameplay
        if (player != null)
        {
            player.enabled = true;
        
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
