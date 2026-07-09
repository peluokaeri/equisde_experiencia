using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TriggerLuz : MonoBehaviour
{
    [Header("Atraccion")]
    public Transform puntoDestino;
    public float fuerzaAtraccion = 4f;

    [Header("Fade blanco")]
    public Image imagenBlanca;

    [Header("Cambio de escena")]
    public string nombreEscena;

    private GameObject player;
    private FirstPlayer firstPlayer;
    private Rigidbody playerRb;
    private bool atrayendo = false;
    private bool habilitado = false;
    private float distanciaInicial;

    void Start()
    {
        if (imagenBlanca != null)
        {
            imagenBlanca.gameObject.SetActive(false);
        }
    }

    // Llamado desde TriggerAuto al terminar el dialogo
    public void Habilitar()
    {
        habilitado = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (atrayendo) return;
        if (!habilitado) return;
        if (!other.CompareTag("Player")) return;

        player = other.gameObject;
        firstPlayer = player.GetComponent<FirstPlayer>();
        playerRb = player.GetComponent<Rigidbody>();

        atrayendo = true;
        distanciaInicial = Vector3.Distance(player.transform.position, puntoDestino.position);

        // Activa la imagen blanca recien ahora
        if (imagenBlanca != null)
        {
            imagenBlanca.gameObject.SetActive(true);
            Color c = imagenBlanca.color;
            c.a = 0f;
            imagenBlanca.color = c;
        }

        if (firstPlayer != null)
            firstPlayer.canMove = false;

        if (playerRb != null)
        {
            playerRb.useGravity = false;
            playerRb.isKinematic = true;
            playerRb.velocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (!atrayendo || player == null) return;

        // Mueve al jugador hacia el punto
        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            puntoDestino.position,
            fuerzaAtraccion * Time.deltaTime
        );

        float distanciaActual = Vector3.Distance(player.transform.position, puntoDestino.position);
        Debug.Log($"Distancia al punto: {distanciaActual}");
        float progreso = 1f - Mathf.Clamp01(distanciaActual / distanciaInicial);

        if (imagenBlanca != null)
        {
            Color c = imagenBlanca.color;
            c.a = progreso;
            imagenBlanca.color = c;
        }

        // Al llegar al punto cambia de escena
        if (distanciaActual < 0.5f)
        {
            atrayendo = false;
            Debug.Log("Llegó al punto, iniciando cambio de escena");
            StartCoroutine(CambiarEscena());
        }
    }

    private IEnumerator CambiarEscena()
    {
        // Fade suave a opacidad total
        if (imagenBlanca != null)
        {
            Color c = imagenBlanca.color;
            while (c.a < 1f)
            {
                c.a += Time.deltaTime * 3f;
                imagenBlanca.color = c;
                yield return null;
            }
            c.a = 1f;
            imagenBlanca.color = c;
        }

        yield return new WaitForSeconds(0.3f);

        Debug.Log($"Cargando escena: '{nombreEscena}'");

        if (!string.IsNullOrEmpty(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
        else
            Debug.LogError("Nombre de escena vacío");
    }
}