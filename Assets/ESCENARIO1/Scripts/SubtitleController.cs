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
    public float horizontalPadding = 40f;
    public float verticalPadding = 20f;

    [Header("Hint")]
    public float idleTimeToShowHint = 2f;
    public float hintFadeSpeed = 2f;

    public bool IsDialogueActive => currentDialogue != null;

    private DialogueData currentDialogue;
    private int currentLineIndex;
    private Coroutine typingCoroutine;
    private Coroutine hintCoroutine;
    private float idleTimer;

    private float panelStartPosX;


   void Start()
{
    subtitleText.text = "";

    // 🔒 FORZAMOS ANCHORS Y PIVOT (CLAVE)
    subtitlePanel.anchorMin = new Vector2(0f, 0.5f);
    subtitlePanel.anchorMax = new Vector2(0f, 0.5f);
    subtitlePanel.pivot = new Vector2(0f, 0.5f);

    panelStartPosX = subtitlePanel.anchoredPosition.x;

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
        foreach (char c in line)
        {
            subtitleText.text += c;
            UpdatePanelSize();
            yield return new WaitForSeconds(typingSpeed);
        }

        UpdatePanelSize();
    }

    void UpdatePanelSize()
{
    subtitleText.ForceMeshUpdate();
    Vector2 size = subtitleText.GetRenderedValues(false);

    subtitlePanel.sizeDelta = new Vector2(
        size.x + horizontalPadding,
        size.y + verticalPadding
    );

    // 🔒 FIJA LA POSICIÓN → NUNCA SE RECENTRA
    subtitlePanel.anchoredPosition = new Vector2(
        panelStartPosX,
        subtitlePanel.anchoredPosition.y
    );
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

        subtitlePanel.gameObject.SetActive(false);
    }
}
