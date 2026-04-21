using UnityEngine;
using System.Collections;

public class TriggerNegro : MonoBehaviour
{
    [Header("Atraccion")]
    public Transform puntoDestino;      // Punto hacia donde se atrae al jugador
    public float fuerzaAtraccion = 5f;  // Velocidad de atraccion

    [Header("Teleport")]
    public Transform puntoTeleport;     // Donde aparece el jugador despues
    public CanvasGroup pantallaNegraCanvas; // CanvasGroup con imagen negra para la transicion
    public float velocidadFade = 2f;

    private GameObject player;
    private FirstPlayer firstPlayer;
    private bool atrayendo = false;
    private bool teleportando = false;

    void Start()
    {
        // Desactivado al inicio, ExamenMatManager lo activa
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

        // Bloquea el movimiento del jugador para controlarlo nosotros
        if (firstPlayer != null)
            firstPlayer.canMove = false;
    }

    void Update()
    {
        if (!atrayendo || player == null || teleportando) return;

        // Mueve al jugador hacia el punto destino
        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            puntoDestino.position,
            fuerzaAtraccion * Time.deltaTime
        );
    }

    // Llamado por el collider del punto destino
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

        // Teleporta al jugador
        if (player != null && puntoTeleport != null)
            player.transform.position = puntoTeleport.position;

        yield return new WaitForSeconds(0.3f);

        // Fade de vuelta a transparente
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

        // Devuelve el control al jugador
        if (firstPlayer != null)
            firstPlayer.canMove = true;

        // Se desactiva a si mismo
        gameObject.SetActive(false);
    }
}