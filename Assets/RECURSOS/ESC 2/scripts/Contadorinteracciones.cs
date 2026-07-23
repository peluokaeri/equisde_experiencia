using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Reflection;

public class ContadorInteracciones : MonoBehaviour
{
    [Header("Objetos interactuables")]
    public MonoBehaviour[] scripts; // 10 scripts

    [Header("Dialogo final")]
    public SubtitleController subtitleController;
    public DialogueData dialogueFinalEsc2;

    [Header("Menu de palabra (minimalista)")]
    public CanvasGroup menuPalabraCanvas;
    public TextMeshProUGUI textoInput;      // Donde se muestra lo que escribe
    public RectTransform cursor;            // Cursor parpadeante
    public float velocidadFade = 1.5f;

    [Header("Palabra correcta y escena")]
    public string palabraCorrecta = "COLEGIO";
    public string nombreEscena;             // Nombre exacto del escenario 3

    [Header("Jugador")]
    public GameObject player;

    private bool dialogoIniciado = false;
    private bool menuMostrado = false;
    private bool inputActivo = false;
    private string textoActual = "";
    private int totalObjetosUsados = 0;
    private FirstPlayer firstPlayer;

    void Start()
    {
        if (menuPalabraCanvas != null)
        {
            menuPalabraCanvas.alpha = 0f;
            menuPalabraCanvas.gameObject.SetActive(false);
            menuPalabraCanvas.interactable = false;
        }

        if (textoInput != null)
            textoInput.text = "";

        if (player != null)
        {
            firstPlayer = player.GetComponent<FirstPlayer>();
            if (firstPlayer == null)
                firstPlayer = player.GetComponentInChildren<FirstPlayer>();
        }
    }

    void Update()
    {
        // Input del menu de palabra
        if (inputActivo)
        {
            LeerTeclado();
            return;
        }

        if (dialogoIniciado) return;

        int usados = ContarUsados();

        if (usados != totalObjetosUsados)
        {
            totalObjetosUsados = usados;
            Debug.Log($"Objetos interactuados: {totalObjetosUsados}/{scripts.Length}");
        }

        if (usados >= scripts.Length)
        {
            dialogoIniciado = true;
            StartCoroutine(EsperarUltimoDialogoYArrancar());
        }
    }

    private void LeerTeclado()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // Backspace
            {
                if (textoActual.Length > 0)
                    textoActual = textoActual.Substring(0, textoActual.Length - 1);
            }
            else if (c == '\n' || c == '\r') // Enter
            {
                VerificarPalabra();
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

    private IEnumerator EsperarUltimoDialogoYArrancar()
    {
        // Espera a que termine el dialogo del ultimo objeto
        yield return new WaitUntil(() => subtitleController.IsDialogueActive);
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        // Espera 2 segundos
        yield return new WaitForSeconds(2f);

        // Inicia el dialogo final
        if (subtitleController != null && dialogueFinalEsc2 != null)
        {
            subtitleController.PlayDialogue(dialogueFinalEsc2);

            yield return new WaitUntil(() => subtitleController.IsDialogueActive);
            yield return new WaitUntil(() => !subtitleController.IsDialogueActive);
        }

        if (!menuMostrado)
        {
            menuMostrado = true;
            StartCoroutine(MostrarMenu());
        }
    }

    private int ContarUsados()
    {
        int count = 0;
        foreach (var script in scripts)
        {
            if (script == null) continue;

            // Busca "used", "triggered" o "agarrado" segun el script
            FieldInfo field = script.GetType().GetField("used",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (field == null)
                field = script.GetType().GetField("triggered",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (field == null)
                field = script.GetType().GetField("agarrado",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(bool))
            {
                bool usado = (bool)field.GetValue(script);
                if (usado) count++;
            }
        }
        return count;
    }

    private IEnumerator MostrarMenu()
    {
        // Bloquea movimiento y camara
        if (firstPlayer != null)
            firstPlayer.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false; // Solo teclado

        if (menuPalabraCanvas != null)
        {
            menuPalabraCanvas.gameObject.SetActive(true);

            while (menuPalabraCanvas.alpha < 1f)
            {
                menuPalabraCanvas.alpha += Time.deltaTime * velocidadFade;
                yield return null;
            }

            menuPalabraCanvas.alpha = 1f;
            menuPalabraCanvas.interactable = true;
        }

        inputActivo = true;
        StartCoroutine(ParpadeoCursor());
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

    public void VerificarPalabra()
    {
        if (textoActual.Trim().ToUpper() == palabraCorrecta.ToUpper())
        {
            inputActivo = false;
            if (cursor != null)
                cursor.gameObject.SetActive(false);

            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            // Palabra incorrecta: limpia el texto
            textoActual = "";
            if (textoInput != null)
                textoInput.text = "";
        }
    }
}