using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CartaInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject instruccionCanvas;
    public Image eImage;

    [Header("Carta")]
    public CanvasGroup cartaCanvas;
    public TextMeshProUGUI textoCarta;
    public ScrollRect scrollRect;           // Arrastra el ScrollView
    public float velocidadFade = 3f;
    public float velocidadScroll = 150f;

    [Header("Contenido")]
    [TextArea(5, 15)]
    public string contenidoCarta;

    [Header("Dialogo requerido (opcional)")]
    public SubtitleController subtitleController;

    [Header("Jugador")]
    public GameObject player;

    private FirstPlayer firstPlayer;
    private bool playerInside = false;
    private bool used = false;
    [HideInInspector] public bool cartaAbierta = false;
    [HideInInspector] public bool fueLeida = false;
    private Coroutine fadeCoroutine;

    void Start()
    {
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        if (eImage != null)
            eImage.enabled = false;

        // Oculta la carta sin desactivar el GameObject
        if (cartaCanvas != null)
        {
            cartaCanvas.alpha = 0f;
            cartaCanvas.interactable = false;
            cartaCanvas.blocksRaycasts = false;
        }

        if (player != null)
        {
            firstPlayer = player.GetComponent<FirstPlayer>();
            if (firstPlayer == null)
                firstPlayer = player.GetComponentInChildren<FirstPlayer>();
        }

        if (textoCarta != null && !string.IsNullOrEmpty(contenidoCarta))
            textoCarta.text = contenidoCarta;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;
        playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        if (eImage != null) eImage.enabled = false;
    }

    void Update()
    {
        // Cerrar carta con E o Escape
        if (cartaAbierta)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
                CerrarCarta();

            // Scroll manual con la ruedita
            if (scrollRect != null)
            {
                float scroll = Input.mouseScrollDelta.y;
                if (scroll != 0f)
                {
                    scrollRect.verticalNormalizedPosition =
                        Mathf.Clamp01(scrollRect.verticalNormalizedPosition + scroll * 0.15f);
                }
            }

            return;
        }

        if (used) return;

        bool dialogoActivo = subtitleController != null && subtitleController.IsDialogueActive;

        if (eImage != null)
            eImage.enabled = playerInside && !dialogoActivo;

        if (!playerInside || dialogoActivo) return;

        if (Input.GetKeyDown(KeyCode.E))
            AbrirCarta();
    }

    private void AbrirCarta()
    {
        cartaAbierta = true;

        if (eImage != null)
            eImage.enabled = false;

        if (firstPlayer != null)
            firstPlayer.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeIn());
    }

    public void CerrarCarta()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        if (cartaCanvas == null) yield break;

        // Habilita interaccion ANTES del fade
        cartaCanvas.interactable = true;
        cartaCanvas.blocksRaycasts = true;

        while (cartaCanvas.alpha < 1f)
        {
            cartaCanvas.alpha += Time.deltaTime * velocidadFade;
            yield return null;
        }

        cartaCanvas.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        if (cartaCanvas == null) yield break;

        cartaCanvas.interactable = false;
        cartaCanvas.blocksRaycasts = false;

        while (cartaCanvas.alpha > 0f)
        {
            cartaCanvas.alpha -= Time.deltaTime * velocidadFade;
            yield return null;
        }

        cartaCanvas.alpha = 0f;

        if (firstPlayer != null)
            firstPlayer.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Resetea el scroll al tope para la proxima vez
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;

        fueLeida = true;
        cartaAbierta = false;
    }
}