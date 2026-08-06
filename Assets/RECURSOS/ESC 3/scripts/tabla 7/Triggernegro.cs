using UnityEngine;
using System.Collections;

public class TriggerNegro : MonoBehaviour
{
    [Header("Atraccion")]
    public Transform puntoDestino;
    public float fuerzaAtraccion = 5f;

    [Header("Teleport")]
    public Transform puntoTeleport;
    public CanvasGroup pantallaNegraCanvas;
    public float velocidadFade = 2f;

    [Header("Secuencia post teleport")]
    public PostDialogoEspacio postDialogoEspacio;

    [Header("Activacion")]
    // Arranca "apagado" (no reacciona) hasta que el apagon lo habilite.
    public bool habilitado = false;

    private GameObject player;
    private FirstPlayer firstPlayer;
    private bool atrayendo = false;
    private bool teleportando = false;
    private Collider miCollider;

    void Awake()
    {
        // Desactiva el collider lo antes posible (antes de cualquier trigger)
        miCollider = GetComponent<Collider>();
        if (miCollider != null)
            miCollider.enabled = false;
        habilitado = false;
    }

    void Start()
    {
        // Refuerza: collider apagado y bandera en false
        if (miCollider == null)
            miCollider = GetComponent<Collider>();
        if (miCollider != null)
            miCollider.enabled = false;

        if (pantallaNegraCanvas != null)
        {
            pantallaNegraCanvas.alpha = 0f;
            pantallaNegraCanvas.gameObject.SetActive(false);
        }
    }

    // Llamado por ExamenMatManager en el apagon para activar el trigger
    public void Habilitar()
    {
        Debug.Log("TriggerNegro HABILITADO");
        habilitado = true;
        if (miCollider != null)
            miCollider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!habilitado) return;       // Ignora si todavia no esta habilitado
        if (!other.CompareTag("Player")) return;
        if (atrayendo) return;

        Debug.Log("TriggerNegro: player entró (habilitado=" + habilitado + ")");

        player = other.gameObject;
        firstPlayer = player.GetComponent<FirstPlayer>();

        atrayendo = true;

        if (firstPlayer != null)
            firstPlayer.canMove = false;
    }

    void Update()
    {
        // Si no esta habilitado, no hace absolutamente nada
        if (!habilitado) return;

        if (!atrayendo || player == null || teleportando) return;

        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            puntoDestino.position,
            fuerzaAtraccion * Time.deltaTime
        );
    }

    public void IniciarTeleport()
    {
        if (!habilitado)
        {
            Debug.LogWarning("IniciarTeleport llamado pero NO habilitado - se ignora. Quien llama: " + System.Environment.StackTrace);
            return;
        }
        if (teleportando) return;
        if (!gameObject.activeInHierarchy) return;
        Debug.Log("IniciarTeleport EJECUTADO (habilitado=true)");
        teleportando = true;
        StartCoroutine(Teleport());
    }

    private IEnumerator Teleport()
    {
        // Fade a negro
        if (pantallaNegraCanvas != null)
        {
            pantallaNegraCanvas.gameObject.SetActive(true);

            while (pantallaNegraCanvas.alpha < 1f)
            {
                pantallaNegraCanvas.alpha += Time.deltaTime * velocidadFade;
                yield return null;
            }
            pantallaNegraCanvas.alpha = 1f;
        }

        yield return new WaitForSeconds(0.5f);

        // Teleporta
        if (player != null && puntoTeleport != null)
            player.transform.position = puntoTeleport.position;

        yield return new WaitForSeconds(0.3f);

        // Fade de vuelta
        if (pantallaNegraCanvas != null)
        {
            while (pantallaNegraCanvas.alpha > 0f)
            {
                pantallaNegraCanvas.alpha -= Time.deltaTime * velocidadFade;
                yield return null;
            }
            pantallaNegraCanvas.alpha = 0f;
            pantallaNegraCanvas.gameObject.SetActive(false);
        }

        // Devuelve movimiento
        if (firstPlayer != null)
            firstPlayer.canMove = true;

        // 1 — Primero dispara el dialogo
        if (postDialogoEspacio != null && postDialogoEspacio.subtitleController != null)
            postDialogoEspacio.subtitleController.PlayDialogue(postDialogoEspacio.GetDialogue());

        // 2 — Luego inicia la espera
        if (postDialogoEspacio != null)
            postDialogoEspacio.IniciarEspera();

        // Se "apaga" desactivando el collider y la bandera (NO el GameObject,
        // asi las corrutinas y referencias siguen funcionando)
        habilitado = false;
        atrayendo = false;
        if (miCollider != null)
            miCollider.enabled = false;
    }
}