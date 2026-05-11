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

    [Header("Menu codigo")]
    public CanvasGroup menuCodigoCanvas;    // CanvasGroup con el menu, alpha 0 al inicio
    public TMP_InputField inputCodigo;      // Campo de texto para escribir
    public Button botonConfirmar;           // Boton confirmar
    public TextMeshProUGUI textoError;      // Texto de error si el codigo es incorrecto
    public float velocidadFadeMenu = 1.5f;

    [Header("Flash blanco y cambio de escena")]
    public CanvasGroup flashBlanco;         // CanvasGroup con imagen blanca, alpha 0
    public float velocidadFlash = 2f;
    public string nombreEscena;             // Nombre de la escena destino

    [Header("Jugador")]
    public FirstPlayer firstPlayer;

    private const string CODIGO_CORRECTO = "REALIDAD";
    private bool usado = false;

    void Start()
    {
        if (menuCodigoCanvas != null)
        {
            menuCodigoCanvas.alpha = 0f;
            menuCodigoCanvas.gameObject.SetActive(false);
            menuCodigoCanvas.interactable = false;
        }

        if (textoError != null)
            textoError.gameObject.SetActive(false);

        if (flashBlanco != null)
        {
            flashBlanco.alpha = 0f;
            flashBlanco.gameObject.SetActive(false);
        }

        if (botonConfirmar != null)
            botonConfirmar.onClick.AddListener(VerificarCodigo);
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
        // Desbloquea mouse para escribir
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Desactiva el componente FirstPlayer para bloquear camara
        if (firstPlayer != null)
            firstPlayer.enabled = false;

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

            // Foca el input automaticamente
            if (inputCodigo != null)
                inputCodigo.Select();
        }
    }

    public void VerificarCodigo()
    {
        if (inputCodigo == null) return;

        string codigo = inputCodigo.text.Trim().ToUpper();

        if (codigo == CODIGO_CORRECTO)
        {
            StartCoroutine(FlashYCambioEscena());
        }
        else
        {
            if (textoError != null)
            {
                textoError.gameObject.SetActive(true);
                textoError.text = "Código incorrecto";
                StartCoroutine(OcultarError());
            }

            inputCodigo.text = "";
            inputCodigo.Select();
        }
    }

    private IEnumerator OcultarError()
    {
        yield return new WaitForSeconds(2f);
        if (textoError != null)
            textoError.gameObject.SetActive(false);
    }

    private IEnumerator FlashYCambioEscena()
    {
        // Deshabilita el menu
        if (menuCodigoCanvas != null)
            menuCodigoCanvas.interactable = false;

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

        // Cambia de escena
        if (!string.IsNullOrEmpty(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
    }
}