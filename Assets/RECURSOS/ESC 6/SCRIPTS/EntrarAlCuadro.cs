using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Efecto tipo Mario 64: el jugador aprieta E, se reproduce un dialogo,
// y al terminar el JUGADOR es atraido hacia el cuadro y "entra" en el
// con un efecto de ondulacion (como meterse al agua) antes de cargar 6.5.
public class EntrarAlCuadro : MonoBehaviour
{
    [Header("UI")]
    public Image eImage;

    [Header("Dialogo")]
    public SubtitleController subtitleController;
    public DialogueData dialogueAntesDeEntrar;

    [Header("Referencias")]
    public Transform puntoEntrada;      // Punto justo frente al cuadro (a donde es atraido)
    public Transform camara;            // La camara del jugador

    [Header("Atraccion hacia el cuadro")]
    public float duracionAtraccion = 1.2f;  // Cuanto tarda en ser tragado
    public float aceleracion = 2f;           // Que tan rapido acelera al entrar

    [Header("Efecto de agua/ondulacion")]
    public Material materialCuadro;     // Material del cuadro (para el ripple)
    public float intensidadOnda = 0.05f;
    public float velocidadOnda = 3f;
    public float duracionOnda = 1f;

    [Header("Fade final")]
    public Image imagenFade;            // Image full screen
    public float velocidadFade = 2.5f;

    [Header("Jugador")]
    public FirstPlayer firstPlayer;

    [Header("Cambio de escena")]
    public string nombreEscena = "Escenario6.5";

    private GameObject player;
    private bool playerInside = false;
    private bool activado = false;

    void Start()
    {
        if (eImage != null)
            eImage.enabled = false;

        if (imagenFade != null)
        {
            Color c = imagenFade.color;
            c.a = 0f;
            imagenFade.color = c;
            imagenFade.gameObject.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        player = other.gameObject;
        playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        if (eImage != null && !activado) eImage.enabled = false;
    }

    void Update()
    {
        if (activado)
        {
            if (eImage != null) eImage.enabled = false;
            return;
        }

        bool dialogoActivo = subtitleController != null && subtitleController.IsDialogueActive;

        if (playerInside && eImage != null)
            eImage.enabled = !dialogoActivo;

        if (!playerInside || dialogoActivo) return;

        if (Input.GetKeyDown(KeyCode.E))
            Entrar();
    }

    private void Entrar()
    {
        activado = true;

        if (eImage != null)
            eImage.enabled = false;

        // Bloquea el control del jugador (a partir de aca lo movemos nosotros)
        if (firstPlayer != null)
            firstPlayer.canMove = false;

        StartCoroutine(SecuenciaEntrada());
    }

    private IEnumerator SecuenciaEntrada()
    {
        // 1 — Dialogo antes de entrar
        if (subtitleController != null && dialogueAntesDeEntrar != null)
        {
            subtitleController.PlayDialogue(dialogueAntesDeEntrar);

            yield return null;
            yield return null;
            yield return new WaitUntil(() => !subtitleController.IsDialogueActive);
        }

        // 2 — Arranca el efecto de ondulacion en el cuadro (como agua)
        if (materialCuadro != null)
            StartCoroutine(OndularCuadro());

        // 3 — El JUGADOR es atraido hacia el punto de entrada del cuadro
        if (player != null && puntoEntrada != null)
        {
            Vector3 posInicial = player.transform.position;
            float t = 0f;

            while (t < duracionAtraccion)
            {
                t += Time.deltaTime;
                float k = t / duracionAtraccion;
                // Ease in: acelera al acercarse (sensacion de ser tragado)
                float suave = Mathf.Pow(k, aceleracion);

                player.transform.position = Vector3.Lerp(posInicial, puntoEntrada.position, suave);
                yield return null;
            }

            player.transform.position = puntoEntrada.position;
        }

        // 4 — Fade final justo al "entrar" a la superficie
        if (imagenFade != null)
        {
            Color c = imagenFade.color;
            while (c.a < 1f)
            {
                c.a += Time.deltaTime * velocidadFade;
                imagenFade.color = c;
                yield return null;
            }
            c.a = 1f;
            imagenFade.color = c;
        }

        yield return new WaitForSeconds(0.3f);

        // 5 — Cambio de escena
        if (!string.IsNullOrEmpty(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
    }

    // Ondula la superficie del cuadro como agua (mueve el offset de la textura)
    private IEnumerator OndularCuadro()
    {
        float t = 0f;
        Vector2 offsetOriginal = materialCuadro.mainTextureOffset;

        while (t < duracionOnda)
        {
            t += Time.deltaTime;

            // Ondas en la superficie moviendo el offset de textura
            float onda = Mathf.Sin(t * velocidadOnda * Mathf.PI * 2f) * intensidadOnda;
            float ondaX = Mathf.Cos(t * velocidadOnda * Mathf.PI * 2f * 0.7f) * intensidadOnda * 0.5f;

            materialCuadro.mainTextureOffset = offsetOriginal + new Vector2(ondaX, onda);
            yield return null;
        }

        materialCuadro.mainTextureOffset = offsetOriginal;
    }
}