using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SubtitleController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private RectTransform subtitlePanel;
    [SerializeField] private Image clickHintImage;

    [Header("Textos")]
    [TextArea(3, 5)]
    [SerializeField] private string[] subtitles;

    [Header("Escritura")]
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private Vector2 panelPadding = new Vector2(40, 25);

    [Header("Click Hint")]
    [SerializeField] private float idleTimeToHint = 5f;
    [SerializeField] private float hintBlinkSpeed = 0.5f;

    private int currentIndex = 0;
    private bool isTyping = false;
    private float idleTimer = 0f;

    private Coroutine typingCoroutine;
    private Coroutine blinkCoroutine;

    void Start()
    {

         RectTransform rt = subtitlePanel;

    rt.anchorMin = new Vector2(0f, 0.5f);
    rt.anchorMax = new Vector2(0f, 0.5f);
    rt.pivot     = new Vector2(0f, 0.5f);
    
        clickHintImage.gameObject.SetActive(false);
        ShowSubtitle();
    }

    void Update()
    {
        HandleInput();
        HandleIdleTimer();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            idleTimer = 0f;
            StopClickHint();

            if (isTyping)
            {
                CompleteText();
            }
            else
            {
                NextSubtitle();
            }
        }
    }

    void HandleIdleTimer()
    {
        if (!isTyping)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTimeToHint && blinkCoroutine == null)
            {
                blinkCoroutine = StartCoroutine(BlinkHint());
            }
        }
    }

    void ShowSubtitle()
    {
        if (currentIndex >= subtitles.Length) return;

        subtitleText.text = "";
        typingCoroutine = StartCoroutine(TypeText(subtitles[currentIndex]));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;

        foreach (char c in text)
        {
            subtitleText.text += c;
            AdjustPanelSize();
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        AdjustPanelSize();
    }

    void CompleteText()
    {
        StopCoroutine(typingCoroutine);
        subtitleText.text = subtitles[currentIndex];
        isTyping = false;
        AdjustPanelSize();
    }

    void NextSubtitle()
    {
        currentIndex++;
        idleTimer = 0f;

        if (currentIndex < subtitles.Length)
        {
            ShowSubtitle();
        }
        else
        {
            subtitleText.text = "";
            subtitlePanel.gameObject.SetActive(false);
        }
    }

void AdjustPanelSize()
{
    subtitleText.ForceMeshUpdate();

    float width = subtitleText.preferredWidth + panelPadding.x;
    float height = subtitleText.preferredHeight + panelPadding.y;

    subtitlePanel.sizeDelta = new Vector2(width, height);
}


    IEnumerator BlinkHint()
    {
        clickHintImage.gameObject.SetActive(true);
        Color c = clickHintImage.color;

        while (true)
        {
            c.a = Mathf.PingPong(Time.time * (1f / hintBlinkSpeed), 1f);
            clickHintImage.color = c;
            yield return null;
        }
    }

    void StopClickHint()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
            clickHintImage.gameObject.SetActive(false);
        }
    }
}
