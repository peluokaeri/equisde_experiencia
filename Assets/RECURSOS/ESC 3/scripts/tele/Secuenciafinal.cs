using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SecuenciaFinal : MonoBehaviour
{
    [Header("Teles finales")]
    public GameObject[] telesFinales;
    public float intervaloTeles = 0.2f;

    [Header("Dialogo final")]
    public SubtitleController subtitleController;
    public DialogueData dialogueFinal;

    [Header("Menu codigo (minimalista)")]
    public CanvasGroup menuCodigoCanvas;    // CanvasGroup con el menu, alpha 0 al inicio
    public TextMeshProUGUI textoInput;      // Donde se muestra lo que escribe el jugador
    public RectTransform cursor;            // Cursor parpadeante
    public float velocidadFadeMenu = 1.5f;

    [Header("Flash blanco y cambio de escena")]
    public CanvasGroup flashBlanco;         // CanvasGroup con imagen blanca, alpha 0
    public float velocidadFlash = 2f;
    public string nombreEscena;             // Nombre de la escena destino

    [Header("Jugador")]
    public FirstPlayer firstPlayer;

    private const string CODIGO_CORRECTO = "REALIDAD";
    private bool usado = false;
    private bool inputActivo = false;
    private string textoActual = "";

    void Start()
    {
        if (menuCodigoCanvas != null)
        {
            menuCodigoCanvas.alpha = 0f;
            menuCodigoCanvas.gameObject.SetActive(false);
            menuCodigoCanvas.interactable = false;
        }

        if (textoInput != null)
            textoInput.text = "";

        if (flashBlanco != null)
        {
            flashBlanco.alpha = 0f;
            flashBlanco.gameObject.SetActive(false);
        }
    }

    // Llamado por PuntoAtraccionFinal
    public void IniciarSecuenciaFinal()
    {
        if (usado) return;
        usado = true;
        StartCoroutine(Secuencia());
    }

    private IEnumerator Secuencia()
    {
        // 1 — Teles aparecen de a poco
        foreach (var tele in telesFinales)
        {
            if (tele != null)
                tele.SetActive(true);
            yield return new WaitForSeconds(intervaloTeles);
        }

        yield return new WaitForSeconds(0.5f);

        // 2 — Inicia dialogo final
        if (subtitleController != null && dialogueFinal != null)
        {
            subtitleController.PlayDialogue(dialogueFinal);

            // Patron Puerta2AfterDialogue
            yield return new WaitUntil(() => subtitleController.IsDialogueActive);
            yield return new WaitUntil(() => !subtitleController.IsDialogueActive);
        }

        // 3 — Aparece el menu de codigo con fade
        yield return StartCoroutine(MostrarMenuCodigo());
    }

    private IEnumerator MostrarMenuCodigo()
    {
        // Bloquea movimiento y camara
        if (firstPlayer != null)
            firstPlayer.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false; // Solo teclado, sin mouse

        if (menuCodigoCanvas != null)
        {
            menuCodigoCanvas.gameObject.SetActive(true);

            while (menuCodigoCanvas.alpha < 1f)
            {
                menuCodigoCanvas.alpha += Time.deltaTime * velocidadFadeMenu;
                yield return null;
            }

            menuCodigoCanvas.alpha = 1f;
            menuCodigoCanvas.interactable = true;
        }

        inputActivo = true;
        StartCoroutine(ParpadeoCursor());
    }

    void Update()
    {
        if (!inputActivo) return;

        // Captura las letras tipeadas
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // Backspace
            {
                if (textoActual.Length > 0)
                    textoActual = textoActual.Substring(0, textoActual.Length - 1);
            }
            else if (c == '\n' || c == '\r') // Enter
            {
                VerificarCodigo();
            }
            else if (char.IsLetter(c))
            {
                textoActual += char.ToUpper(c);
            }
        }

        if (textoInput != null)
            textoInput.text = textoActual;

        // Mueve el cursor al final del texto
        if (cursor != null && textoInput != null)
        {
            float anchoTexto = textoInput.preferredWidth;
            cursor.anchoredPosition = new Vector2(anchoTexto / 2f + 10f, cursor.anchoredPosition.y);
        }
    }

    private IEnumerator ParpadeoCursor()
    {
        while (inputActivo)
        {
            if (cursor != null)
                cursor.gameObject.SetActive(!cursor.gameObject.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void VerificarCodigo()
    {
        if (textoActual.Trim().ToUpper() == CODIGO_CORRECTO)
        {
            inputActivo = false;
            StartCoroutine(FlashYCambioEscena());
        }
        else
        {
            // Codigo incorrecto: limpia el texto
            textoActual = "";
            if (textoInput != null)
                textoInput.text = "";
        }
    }

    private IEnumerator FlashYCambioEscena()
    {
        if (menuCodigoCanvas != null)
            menuCodigoCanvas.interactable = false;

        if (cursor != null)
            cursor.gameObject.SetActive(false);

        // Flash blanco
        if (flashBlanco != null)
        {
            flashBlanco.gameObject.SetActive(true);

            while (flashBlanco.alpha < 1f)
            {
                flashBlanco.alpha += Time.deltaTime * velocidadFlash;
                yield return null;
            }

            flashBlanco.alpha = 1f;
        }

        yield return new WaitForSeconds(0.5f);

        if (!string.IsNullOrEmpty(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
    }
}