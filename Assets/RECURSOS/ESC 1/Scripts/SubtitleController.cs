using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SubtitleController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text subtitleText;
    public RectTransform subtitlePanel;
    public Image clickHintImage;

    [Header("Typing")]
    public float typingSpeed = 0.03f;
    public float verticalPadding = 20f;

    [Header("Hint")]
    public float idleTimeToShowHint = 2f;
    public float hintFadeSpeed = 2f;

    public bool IsDialogueActive => currentDialogue != null;
    public System.Action OnDialogueEnd; // Evento que se dispara al terminar

    private DialogueData currentDialogue;
    private int currentLineIndex;
    private Coroutine typingCoroutine;
    private Coroutine hintCoroutine;
    private float idleTimer;

    private Vector2 panelStartPos;

    // Ancho del canvas (referencia). El panel ocupa todo el ancho
    // asi el texto centrado por alignment queda SIEMPRE en el medio.
    private const float PANEL_WIDTH = 1920f;


    void Start()
    {
        subtitleText.text = "";

        // 🔒 ANCLAJE Y PIVOT CENTRADOS
        subtitlePanel.anchorMin = new Vector2(0.5f, 0.5f);
        subtitlePanel.anchorMax = new Vector2(0.5f, 0.5f);
        subtitlePanel.pivot = new Vector2(0.5f, 0.5f);

        // Panel de ancho fijo = ancho del canvas (no se sale nunca)
        subtitlePanel.sizeDelta = new Vector2(PANEL_WIDTH, subtitlePanel.sizeDelta.y);

        // Guardamos la posicion para mantenerla fija
        panelStartPos = subtitlePanel.anchoredPosition;
        // Forzamos X centrada (Pos X = 0). La Y la respeta como este en el editor.
        panelStartPos.x = 0f;
        subtitlePanel.anchoredPosition = panelStartPos;

        // Texto centrado dentro del panel
        subtitleText.alignment = TextAlignmentOptions.Center;

        // El texto ocupa TODO el panel (stretch), asi el centro coincide
        RectTransform textRT = subtitleText.rectTransform;
        textRT.anchorMin = new Vector2(0f, 0f);
        textRT.anchorMax = new Vector2(1f, 1f);
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        subtitlePanel.gameObject.SetActive(false);
        SetHintAlpha(0f);
    }

    void Update()
    {
        if (currentDialogue == null)
            return;

        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTimeToShowHint && hintCoroutine == null)
            hintCoroutine = StartCoroutine(SmoothHint());

        if (Input.GetMouseButtonDown(1)) // Click derecho
        {
            ResetIdle();
            NextLine();
        }

        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            ResetIdle();
            PreviousLine();
        }
    }

    // 🔹 Llamado desde triggers / eventos
    public void PlayDialogue(DialogueData dialogue)
    {
        if (dialogue == null) return;

        currentDialogue = dialogue;
        currentLineIndex = 0;

        subtitlePanel.gameObject.SetActive(true);
        ResetIdle();
        ShowLine();
    }

    void NextLine()
    {
        if (currentLineIndex >= currentDialogue.lines.Length - 1)
        {
            ClearDialogue();
            return;
        }

        currentLineIndex++;
        ShowLine();
    }

    void PreviousLine()
    {
        if (currentLineIndex <= 0)
            return;

        currentLineIndex--;
        ShowLine();
    }

    void ShowLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        subtitleText.text = "";
        typingCoroutine = StartCoroutine(TypeText(currentDialogue.lines[currentLineIndex]));
    }

    IEnumerator TypeText(string line)
    {
        // Escribe letra por letra. Como el panel es de ancho fijo y el texto
        // esta centrado, cada letra reacomoda el texto al centro solo.
        foreach (char c in line)
        {
            subtitleText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void ResetIdle()
    {
        idleTimer = 0f;

        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            hintCoroutine = null;
        }

        SetHintAlpha(0f);
    }

    IEnumerator SmoothHint()
    {
        Color c = clickHintImage.color;

        while (true)
        {
            // Fade in
            while (c.a < 1f)
            {
                c.a += Time.deltaTime * hintFadeSpeed;
                clickHintImage.color = c;
                yield return null;
            }

            // Fade out
            while (c.a > 0f)
            {
                c.a -= Time.deltaTime * hintFadeSpeed;
                clickHintImage.color = c;
                yield return null;
            }
        }
    }

    void SetHintAlpha(float alpha)
    {
        Color c = clickHintImage.color;
        c.a = alpha;
        clickHintImage.color = c;
    }

    void ClearDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (hintCoroutine != null)
            StopCoroutine(hintCoroutine);

        subtitleText.text = "";
        SetHintAlpha(0f);
        currentDialogue = null;
        OnDialogueEnd?.Invoke();

        subtitlePanel.gameObject.SetActive(false);
    }
}