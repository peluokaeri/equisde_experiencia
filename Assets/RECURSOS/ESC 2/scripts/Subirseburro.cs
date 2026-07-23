using UnityEngine;
using UnityEngine.UI;

public class SubirseBurro : MonoBehaviour
{
    [Header("UI")]
    public GameObject instruccionCanvas;
    public Image eImage;

    [Header("Dialogo")]
    public SubtitleController subtitleController;
    public DialogueData dialogueBurro;

    [Header("Posicion")]
    public Transform puntoAsiento;          // Donde se sienta el jugador (hijo de la hamaca para que siga la animacion)

    [Header("Animacion de la hamaca")]
    public Animator burroAnimator;

    [Header("Sonido")]
    public AudioSource audioSource;

    private bool playerInside = false;
    private bool enHamaca = false;
    private bool used = false;

    private GameObject player;
    private FirstPlayer firstPlayer;
    private Rigidbody playerRb;
    private Vector3 offsetAsiento;

    void Start()
    {
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        if (eImage != null)
            eImage.enabled = false;

        if (burroAnimator != null)
            burroAnimator.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (enHamaca) return;
        if (!other.CompareTag("Player")) return;

        player = other.gameObject;
        firstPlayer = player.GetComponent<FirstPlayer>();
        playerRb = player.GetComponent<Rigidbody>();
        playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        if (eImage != null && !enHamaca) eImage.enabled = false;
    }

    void Update()
    {
        // Mientras esta en la hamaca, sigue la posicion del asiento
        if (enHamaca)
        {
            if (eImage != null)
                eImage.enabled = false;

            if (puntoAsiento != null && player != null)
                player.transform.position = puntoAsiento.position;
            return;
        }

        // Ya se uso: fuerza E apagada y no permite reinteractuar
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
            Subir();
    }

    private void Subir()
    {
        enHamaca = true;
        used = true;

        if (eImage != null)
            eImage.enabled = false;

        // Bloquea movimiento normal
        if (firstPlayer != null)
            firstPlayer.canMove = false;

        if (playerRb != null)
        {
            playerRb.useGravity = false;
            playerRb.isKinematic = true;
        }

        // Sienta al jugador en el asiento (sin cambiar parent)
        if (puntoAsiento != null)
            player.transform.position = puntoAsiento.position;

        // Activa la animacion de la hamaca
        if (burroAnimator != null)
        {
            burroAnimator.enabled = true;
            StartCoroutine(EsperarFinAnimacion());
        }

        if (audioSource != null)
            audioSource.Play();

        // Dialogo
        if (subtitleController != null && dialogueBurro != null)
            subtitleController.PlayDialogue(dialogueBurro);
    }

    private System.Collections.IEnumerator EsperarFinAnimacion()
    {
        // Espera un frame para que el Animator entre al estado
        yield return null;

        // Duracion del clip actual
        float duracion = burroAnimator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(duracion);

        Bajar();
    }

    private void Bajar()
    {
        enHamaca = false;
        used = true;

        if (eImage != null)
            eImage.enabled = false;

        // Restaura movimiento
        if (firstPlayer != null)
            firstPlayer.canMove = true;

        if (playerRb != null)
        {
            playerRb.isKinematic = false;
            playerRb.useGravity = true;

            // 🔒 Limpia cualquier velocidad acumulada para que no salga disparado
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // Detiene la animacion
        if (burroAnimator != null)
            burroAnimator.enabled = false;

        if (audioSource != null)
            audioSource.Stop();

    }
}