using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuOpciones : MonoBehaviour
{
    [Header("Canvas del menu")]
    public CanvasGroup menuCanvas;
    public float velocidadFade = 2f;

    [Header("Botones")]
    public Button botonBaston;
    public Button botonSombrero;
    public Button botonBigote;

    [Header("Mensaje incorrecto")]
    public GameObject mensajeIncorrecto; // TextMeshPro desactivado al inicio

    [Header("Dialogo correcto")]
    public SubtitleController subtitleController;
    public DialogueData dialogueBigote;

    [Header("Activacion automatica")]
    public bool esperarPrimerDialogo = true;

    [Header("Jugador")]
    public GameObject player;

    private FirstPlayer firstPlayer;
    private Button ultimoBotonPresionado;
    private bool respondioCorrectamente = false;

    void Start()
    {
        if (menuCanvas != null)
        {
            menuCanvas.alpha = 0f;
            menuCanvas.gameObject.SetActive(false);
            menuCanvas.interactable = false;
        }

        if (mensajeIncorrecto != null)
            mensajeIncorrecto.SetActive(false);

        if (player != null)
        {
            firstPlayer = player.GetComponent<FirstPlayer>();
            if (firstPlayer == null)
                firstPlayer = player.GetComponentInChildren<FirstPlayer>();
        }

        botonBaston.onClick.AddListener(() => OnOpcionIncorrecta(botonBaston));
        botonSombrero.onClick.AddListener(() => OnOpcionIncorrecta(botonSombrero));
        botonBigote.onClick.AddListener(OnBigote);

        if (esperarPrimerDialogo)
            StartCoroutine(EsperarPrimerDialogo());
    }

    IEnumerator EsperarPrimerDialogo()
    {
        yield return new WaitUntil(() => subtitleController.IsDialogueActive);
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);
        StartCoroutine(MostrarMenu());
    }

    public void Activar()
    {
        StartCoroutine(MostrarMenu());
    }

    private IEnumerator MostrarMenu()
    {
        // Bloquea movimiento y rotacion
        if (firstPlayer != null)
            firstPlayer.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(true);

            while (menuCanvas.alpha < 1f)
            {
                menuCanvas.alpha += Time.deltaTime * velocidadFade;
                yield return null;
            }

            menuCanvas.alpha = 1f;
            menuCanvas.interactable = true;
        }
    }

    private void OnOpcionIncorrecta(Button boton)
    {
        if (respondioCorrectamente) return;
        StartCoroutine(MostrarMensajeIncorrecto());
    }

    private IEnumerator MostrarMensajeIncorrecto()
    {
        // Muestra el mensaje
        if (mensajeIncorrecto != null)
        {
            mensajeIncorrecto.SetActive(true);
            // Espera 2.5 segundos y lo oculta
            yield return new WaitForSeconds(2.5f);
            mensajeIncorrecto.SetActive(false);
        }
    }

    private void OnBigote()
    {
        if (respondioCorrectamente) return;
        respondioCorrectamente = true;

        StartCoroutine(OcultarYContinuar());
    }

    private IEnumerator OcultarYContinuar()
    {
        // Oculta el menu con fade
        if (menuCanvas != null)
        {
            menuCanvas.interactable = false;

            while (menuCanvas.alpha > 0f)
            {
                menuCanvas.alpha -= Time.deltaTime * velocidadFade;
                yield return null;
            }

            menuCanvas.alpha = 0f;
            menuCanvas.gameObject.SetActive(false);
        }

        // Reactiva movimiento y bloquea mouse
        if (firstPlayer != null)
            firstPlayer.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Dispara el dialogo de continuacion
        if (subtitleController != null && dialogueBigote != null)
            subtitleController.PlayDialogue(dialogueBigote);
    }
}