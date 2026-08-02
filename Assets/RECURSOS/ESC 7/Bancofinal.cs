using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Banco del escenario final. Al apretar E aparece un dialogo de
// advertencia (al sentarse ya no podras volver). Si el jugador vuelve
// a apretar E, se sienta DEFINITIVAMENTE en silencio (es el final).
public class BancoFinal : MonoBehaviour
{
    [Header("UI")]
    public Image eImage;

    [Header("Advertencia")]
    public SubtitleController subtitleController;
    public DialogueData dialogueAdvertencia;

    [Header("Posicion de asiento")]
    public Transform puntoAsiento;

    [Header("Cierre de la experiencia")]
    public CierreOjos cierreOjos;   // Efecto de ojos cerrandose al sentarse

    [Header("Jugador")]
    public GameObject player;

    private FirstPlayer firstPlayer;
    private Rigidbody playerRb;
    private Collider playerCollider;

    private bool playerInside = false;
    private bool advertenciaMostrada = false;
    private bool sentado = false;

    void Start()
    {
        if (eImage != null)
            eImage.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.gameObject;
        firstPlayer = player.GetComponent<FirstPlayer>();
        playerRb = player.GetComponent<Rigidbody>();
        playerCollider = player.GetComponent<Collider>();
        playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;

        // Apaga la E al salir, pero NO resetea la advertencia:
        // una vez mostrada, la proxima E ya sienta directamente.
        if (eImage != null && !sentado)
            eImage.enabled = false;
    }

    void Update()
    {
        // Ya sentado: fija la posicion y apaga la E para siempre
        if (sentado)
        {
            if (eImage != null) eImage.enabled = false;
            if (puntoAsiento != null && player != null)
                player.transform.position = puntoAsiento.position;
            return;
        }

        bool dialogoActivo = subtitleController != null && subtitleController.IsDialogueActive;

        // Muestra la E cuando esta cerca y no hay dialogo
        if (playerInside && eImage != null)
            eImage.enabled = !dialogoActivo;

        if (!playerInside || dialogoActivo) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!advertenciaMostrada)
            {
                // Primera E: muestra la advertencia
                MostrarAdvertencia();
            }
            else
            {
                // Segunda E (tras la advertencia): se sienta definitivamente
                Sentarse();
            }
        }
    }

    private void MostrarAdvertencia()
    {
        advertenciaMostrada = true;

        if (subtitleController != null && dialogueAdvertencia != null)
            subtitleController.PlayDialogue(dialogueAdvertencia);
    }

    private void Sentarse()
    {
        sentado = true;

        if (eImage != null)
            eImage.enabled = false;

        // Bloquea el movimiento para siempre
        if (firstPlayer != null)
            firstPlayer.canMove = false;

        if (playerRb != null)
        {
            playerRb.isKinematic = true;
            playerRb.useGravity = false;
        }

        // Desactiva el collider para no salir volando
        if (playerCollider != null)
            playerCollider.enabled = false;

        // Sienta al jugador
        if (puntoAsiento != null)
            player.transform.position = puntoAsiento.position;

        // Inicia el cierre de la experiencia (ojos cerrandose)
        if (cierreOjos != null)
            cierreOjos.IniciarCierre();
    }
}