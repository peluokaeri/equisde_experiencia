using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Al llegar al limite de las escaleras: aparece una plataforma final
// con un cubo. Un ultimo dialogo invita al salto de fe. El jugador
// salta por su cuenta, cae unos segundos, y ahi arranca el fade a
// blanco + cambio de escena.
public class SecuenciaEscaleraFinal : MonoBehaviour
{
    [Header("Dialogo del salto de fe")]
    public SubtitleController subtitleController;
    public DialogueData dialogueSaltoDeFe;

    [Header("Bloqueo hasta terminar el dialogo")]
    // Collider en el borde que evita saltar antes de leer el dialogo.
    public Collider colliderBloqueoSalto;

    [Header("Deteccion de caida")]
    public Transform player;
    // Cuando el jugador cae por debajo de esta altura respecto a la
    // plataforma final, se considera que salto al vacio.
    public float distanciaCaidaParaContar = 3f;
    // Segundos cayendo antes de que arranque el fade
    public float segundosCayendo = 2f;

    [Header("Flash blanco")]
    public Image imagenBlanca;          // Image blanca full screen (alpha 0)
    public float velocidadFlash = 1.5f;

    [Header("Cambio de escena")]
    public string nombreEscena;

    [Header("Efecto de caida (opcional)")]
    public EfectoCaida efectoCaida;     // Balanceo + zoom durante la caida

    private bool activado = false;
    private bool cayendo = false;
    private bool fadeIniciado = false;
    private float alturaPlataforma;

    void Start()
    {
        if (imagenBlanca != null)
        {
            Color c = imagenBlanca.color;
            c.a = 0f;
            imagenBlanca.color = c;
            imagenBlanca.gameObject.SetActive(true);
        }
    }

    // Llamado por EscalerasInfinitas al llegar al limite
    public void Activar()
    {
        if (activado) return;
        activado = true;

        // Guarda la altura actual del jugador como referencia de la plataforma
        if (player != null)
            alturaPlataforma = player.position.y;

        StartCoroutine(SecuenciaDialogo());
    }

    private IEnumerator SecuenciaDialogo()
    {
        // Reproduce el ultimo dialogo (salto de fe)
        if (subtitleController != null && dialogueSaltoDeFe != null)
        {
            subtitleController.PlayDialogue(dialogueSaltoDeFe);

            yield return null;
            yield return null;
            yield return new WaitUntil(() => !subtitleController.IsDialogueActive);
        }

        // Ahora el jugador puede saltar al vacio por su cuenta.
        // Quita el collider que bloqueaba el borde.
        if (colliderBloqueoSalto != null)
            colliderBloqueoSalto.enabled = false;

        // Empezamos a monitorear la caida.
        cayendo = true;
    }

    void Update()
    {
        if (!cayendo || fadeIniciado || player == null) return;

        // Detecta si el jugador cayo lo suficiente (salto al vacio)
        float caida = alturaPlataforma - player.position.y;

        // DEBUG: ver cuanto lleva caido
        if (Time.frameCount % 30 == 0)
            Debug.Log("Cayendo... caida actual: " + caida + " / necesaria: " + distanciaCaidaParaContar);

        if (caida >= distanciaCaidaParaContar)
        {
            fadeIniciado = true;
            Debug.Log("FADE iniciado");

            // Activa el efecto de camara (balanceo + zoom crecientes)
            if (efectoCaida != null)
                efectoCaida.Activar();

            StartCoroutine(CaidaYFade());
        }
    }

    private IEnumerator CaidaYFade()
    {
        // Deja que caiga unos segundos antes del fade (momento de vertigo)
        yield return new WaitForSeconds(segundosCayendo);

        // Fade a blanco
        if (imagenBlanca != null)
        {
            Color c = imagenBlanca.color;
            while (c.a < 1f)
            {
                c.a += Time.deltaTime * velocidadFlash;
                imagenBlanca.color = c;
                yield return null;
            }
            c.a = 1f;
            imagenBlanca.color = c;
        }

        yield return new WaitForSeconds(0.4f);

        if (!string.IsNullOrEmpty(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
    }
}