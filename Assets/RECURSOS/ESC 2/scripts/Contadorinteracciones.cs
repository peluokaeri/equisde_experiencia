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

    [Header("Menu de palabra")]
    public CanvasGroup menuPalabraCanvas;
    public TMP_InputField inputPalabra;
    public Button botonConfirmar;
    public TextMeshProUGUI textoError;
    public float velocidadFade = 1.5f;

    [Header("Palabra correcta y escena")]
    public string palabraCorrecta = "INFANCIA";
    public string nombreEscena;             // Nombre exacto del escenario 3

    [Header("Jugador")]
    public GameObject player;

    private bool dialogoIniciado = false;
    private bool menuMostrado = false;
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

        if (textoError != null)
            textoError.gameObject.SetActive(false);

        if (botonConfirmar != null)
            botonConfirmar.onClick.AddListener(VerificarPalabra);

        if (player != null)
        {
            firstPlayer = player.GetComponent<FirstPlayer>();
            if (firstPlayer == null)
                firstPlayer = player.GetComponentInChildren<FirstPlayer>();
        }
    }

    void Update()
    {
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

            // Patron Puerta2AfterDialogue
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

            FieldInfo field = script.GetType().GetField("used",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (field == null)
                field = script.GetType().GetField("triggered",
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
        // Bloquea movimiento y mouse
        if (firstPlayer != null)
            firstPlayer.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

            if (inputPalabra != null)
                inputPalabra.Select();
        }
    }

    public void VerificarPalabra()
    {
        if (inputPalabra == null) return;

        string palabra = inputPalabra.text.Trim().ToUpper();

        if (palabra == palabraCorrecta.ToUpper())
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            if (textoError != null)
            {
                textoError.gameObject.SetActive(true);
                textoError.text = "Palabra incorrecta";
                StartCoroutine(OcultarError());
            }

            inputPalabra.text = "";
            inputPalabra.Select();
        }
    }

    private IEnumerator OcultarError()
    {
        yield return new WaitForSeconds(2f);
        if (textoError != null)
            textoError.gameObject.SetActive(false);
    }
}