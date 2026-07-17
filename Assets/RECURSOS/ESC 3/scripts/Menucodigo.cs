using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuCodigo : MonoBehaviour
{
    [Header("Canvas")]
    public CanvasGroup menuCanvas;
    public float velocidadFade = 1.5f;

    [Header("Texto")]
    public TextMeshProUGUI textoInput;      // Donde se muestra lo que escribe
    public RectTransform cursor;            // El cursor parpadeante
    public float cursorOffsetBase = 20f;    // Posicion X del cursor sin texto

    [Header("Codigo correcto")]
    public string codigoCorrecto = "REALIDAD";

    [Header("Flash y cambio de escena")]
    public GameObject flashBlanco;
    public float velocidadFlash = 2f;
    public string nombreEscena;

    [Header("Jugador")]
    public GameObject player;

    private FirstPlayer firstPlayer;
    private bool activo = false;
    private string textoActual = "";

    void Start()
    {
        if (menuCanvas != null)
        {
            menuCanvas.alpha = 0f;
            menuCanvas.gameObject.SetActive(false);
        }

        if (textoInput != null)
            textoInput.text = "";

        if (flashBlanco != null)
            flashBlanco.SetActive(false);

        if (player != null)
        {
            firstPlayer = player.GetComponent<FirstPlayer>();
            if (firstPlayer == null)
                firstPlayer = player.GetComponentInChildren<FirstPlayer>();
        }
    }

    // Llamado para mostrar el menu (desde otro script cuando corresponda)
    public void Activar()
    {
        StartCoroutine(MostrarMenu());
    }

    private IEnumerator MostrarMenu()
    {
        // Bloquea movimiento y camara
        if (firstPlayer != null)
            firstPlayer.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false; // No mostramos el mouse, es solo teclado

        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(true);
            while (menuCanvas.alpha < 1f)
            {
                menuCanvas.alpha += Time.deltaTime * velocidadFade;
                yield return null;
            }
            menuCanvas.alpha = 1f;
        }

        activo = true;
        StartCoroutine(ParpadeoCursor());
    }

    void Update()
    {
        if (!activo) return;

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

        // Actualiza el texto mostrado
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
        while (activo)
        {
            if (cursor != null)
                cursor.gameObject.SetActive(!cursor.gameObject.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void VerificarCodigo()
    {
        if (textoActual.Trim().ToUpper() == codigoCorrecto.ToUpper())
        {
            activo = false;
            StartCoroutine(FlashYCambiar());
        }
        else
        {
            // Codigo incorrecto: limpia el texto
            textoActual = "";
            if (textoInput != null)
                textoInput.text = "";
        }
    }

    private IEnumerator FlashYCambiar()
    {
        if (cursor != null)
            cursor.gameObject.SetActive(false);

        if (flashBlanco != null)
        {
            flashBlanco.SetActive(true);
            Image img = flashBlanco.GetComponent<Image>();
            Color c = img.color;
            c.a = 0f;
            img.color = c;

            while (c.a < 1f)
            {
                c.a += Time.deltaTime * velocidadFlash;
                img.color = c;
                yield return null;
            }
            c.a = 1f;
            img.color = c;
        }

        yield return new WaitForSeconds(0.5f);

        if (!string.IsNullOrEmpty(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
    }
}