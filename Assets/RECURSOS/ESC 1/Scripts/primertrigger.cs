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
    public Graphic confirmText;     // TMP_Text o Text
    public Button confirmButton;

    [Header("Timings")]
    public float textFadeTime = 1.5f;
    public float buttonFadeTime = 1f;
    public float buttonBlinkSpeed = 0.25f;

    private bool triggered = false;
    private Coroutine blinkRoutine;
    private Graphic buttonGraphic;

    private void Awake()
    {
        confirmarMenu.SetActive(false);

        buttonGraphic = confirmButton.GetComponent<Graphic>();

        SetAlpha(confirmText, 0f);
        SetAlpha(buttonGraphic, 0f);
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
        // ⏳ Esperar a que termine el diálogo
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        // 🖱️ Desbloquear mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 🚫 Bloquear rotación deshabilitando el player
        if (player != null)
            player.enabled = false;

        confirmarMenu.SetActive(true);

        // Fade texto
        yield return StartCoroutine(FadeGraphic(confirmText, 0f, 1f, textFadeTime));

        // Fade botón
        yield return StartCoroutine(FadeGraphic(buttonGraphic, 0f, 1f, buttonFadeTime));

        // Parpadeo lento
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

        // Fade OUT fuerte
        while (t < 1f)
        {
            t += Time.deltaTime * buttonBlinkSpeed;
            SetAlpha(buttonGraphic, Mathf.Lerp(1f, 0.25f, t));
            yield return null;
        }

        t = 0f;

        // Fade IN fuerte
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

        // 🔓 Rehabilitar player
        if (player != null)
        {
            player.enabled = true;
            player.canMove = true;
        }

        // 🖱️ Volver a bloquear mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
