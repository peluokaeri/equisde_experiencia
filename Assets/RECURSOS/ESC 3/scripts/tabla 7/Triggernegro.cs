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

    private GameObject player;
    private FirstPlayer firstPlayer;
    private bool atrayendo = false;
    private bool teleportando = false;

    void Start()
    {
        gameObject.SetActive(false);

        if (pantallaNegraCanvas != null)
        {
            pantallaNegraCanvas.alpha = 0f;
            pantallaNegraCanvas.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (atrayendo) return;

        player = other.gameObject;
        firstPlayer = player.GetComponent<FirstPlayer>();

        atrayendo = true;

        if (firstPlayer != null)
            firstPlayer.canMove = false;
    }

    void Update()
    {
        if (!atrayendo || player == null || teleportando) return;

        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            puntoDestino.position,
            fuerzaAtraccion * Time.deltaTime
        );
    }

    public void IniciarTeleport()
    {
        if (teleportando) return;
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

        // 2 — Luego inicia la espera (igual que ExamenManager con Puerta2)
        if (postDialogoEspacio != null)
            postDialogoEspacio.IniciarEspera();

        gameObject.SetActive(false);
    }
}