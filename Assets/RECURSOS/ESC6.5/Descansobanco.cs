using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Interaccion del banco del descanso. El jugador entra al trigger,
// aparece la E, la toca y se sienta. Se reproduce un dialogo y al
// terminar puede levantarse. Igual que el corcel/hamaca. Se usa una vez.
public class DescansoBanco : MonoBehaviour
{
    [Header("UI")]
    public Image eImage;

    [Header("Bloqueo hasta sentarse")]
    // Collider que corta el paso hasta que el jugador se siente.
    // Ponerlo al final de la zona de descanso, cruzando el camino.
    public Collider colliderBloqueo;

    [Header("Dialogo")]
    public SubtitleController subtitleController;
    public DialogueData dialogueDescanso;

    [Header("Posicion de asiento")]
    public Transform puntoAsiento;      // Donde se sienta el jugador

    [Header("Fragmentos de recuerdos (opcional)")]
    public FragmentosRecuerdos fragmentos;

    private GameObject player;
    private FirstPlayer firstPlayer;
    private Rigidbody playerRb;
    private Collider playerCollider;

    private bool playerInside = false;
    private bool sentado = false;
    private bool used = false;

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
        if (eImage != null && !sentado) eImage.enabled = false;
    }

    void Update()
    {
        // Mientras esta sentado, sigue la posicion del asiento + E apagada
        if (sentado)
        {
            if (eImage != null)
                eImage.enabled = false;

            if (puntoAsiento != null && player != null)
                player.transform.position = puntoAsiento.position;
            return;
        }

        // Ya se uso: E apagada, no se puede volver a interactuar
        if (used)
        {
            if (eImage != null && playerInside)
                eImage.enabled = false;
            return;
        }

        bool dialogoActivo = subtitleController != null && subtitleController.IsDialogueActive;

        if (playerInside && eImage != null)
            eImage.enabled = !dialogoActivo;

        if (!playerInside || dialogoActivo) return;

        if (Input.GetKeyDown(KeyCode.E))
            Sentarse();
    }

    private void Sentarse()
    {
        sentado = true;

        if (eImage != null)
            eImage.enabled = false;

        // Bloquea movimiento
        if (firstPlayer != null)
            firstPlayer.canMove = false;

        if (playerRb != null)
        {
            playerRb.isKinematic = true;
            playerRb.useGravity = false;
        }

        // Desactiva el collider para que no salga volando
        if (playerCollider != null)
            playerCollider.enabled = false;

        // Sienta al jugador
        if (puntoAsiento != null)
            player.transform.position = puntoAsiento.position;

        // Activa los fragmentos de recuerdos
        if (fragmentos != null)
        {
            Debug.Log("DescansoBanco: llamando a fragmentos.Activar()");
            fragmentos.Activar();
        }
        else
        {
            Debug.LogWarning("DescansoBanco: Fragmentos NO asignado en el inspector");
        }

        // Dialogo
        StartCoroutine(SecuenciaDescanso());
    }

    private IEnumerator SecuenciaDescanso()
    {
        if (subtitleController != null && dialogueDescanso != null)
        {
            subtitleController.PlayDialogue(dialogueDescanso);

            yield return null;
            yield return null;
            yield return new WaitUntil(() => !subtitleController.IsDialogueActive);
        }

        // Desvanece los fragmentos de a poco, y SOLO cuando desaparecieron
        // todos, recien ahi se levanta.
        if (fragmentos != null)
        {
            bool terminado = false;
            fragmentos.DesvanecerTodos(() => terminado = true);
            yield return new WaitUntil(() => terminado);
        }

        Levantarse();
    }

    private void Levantarse()
    {
        sentado = false;
        used = true;

        // Quita el collider que bloqueaba el paso
        if (colliderBloqueo != null)
            colliderBloqueo.enabled = false;

        if (eImage != null)
            eImage.enabled = false;

        // Restaura movimiento
        if (firstPlayer != null)
            firstPlayer.canMove = true;

        if (playerRb != null)
        {
            playerRb.isKinematic = false;
            playerRb.useGravity = true;

            // Limpia velocidad para que no salga disparado
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // Reactiva el collider
        if (playerCollider != null)
            playerCollider.enabled = true;
    }
}