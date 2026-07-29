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
    public Transform puntoAsiento;      // Donde se sienta el jugador
    public Transform puntoBajada;       // Donde aparece al bajarse (lugar despejado)

    [Header("Animacion")]
    public Animator burroAnimator;

    [Header("Sonido")]
    public AudioSource audioSource;

    private bool playerInside = false;
    private bool enHamaca = false;
    private bool used = false;

    private GameObject player;
    private FirstPlayer firstPlayer;
    private Rigidbody playerRb;
    private Collider playerCollider;

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
        playerCollider = player.GetComponent<Collider>();
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
        // Mientras esta montado, sigue la posicion del asiento
        if (enHamaca)
        {
            if (eImage != null)
                eImage.enabled = false;

            if (puntoAsiento != null && player != null)
                player.transform.position = puntoAsiento.position;
            return;
        }

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

        if (firstPlayer != null)
            firstPlayer.canMove = false;

        if (playerRb != null)
        {
            playerRb.useGravity = false;
            playerRb.isKinematic = true;
        }

        // Desactiva el collider mientras monta
        if (playerCollider != null)
            playerCollider.enabled = false;

        if (puntoAsiento != null)
            player.transform.position = puntoAsiento.position;

        if (burroAnimator != null)
        {
            burroAnimator.enabled = true;
            StartCoroutine(EsperarFinAnimacion());
        }

        if (audioSource != null)
            audioSource.Play();

        if (subtitleController != null && dialogueBurro != null)
            subtitleController.PlayDialogue(dialogueBurro);
    }

    private System.Collections.IEnumerator EsperarFinAnimacion()
    {
        yield return null;
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

        // 1 Mueve al player a posicion segura ANTES de activar colisiones
        if (player != null)
        {
            if (puntoBajada != null)
                player.transform.position = puntoBajada.position;
            else
                player.transform.position += Vector3.up * 0.2f - transform.forward * 1f;
        }

        // 2 Limpia velocidad antes de reactivar
        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // 3 Reactiva el collider (ya en posicion segura)
        if (playerCollider != null)
            playerCollider.enabled = true;

        // 4 Reactiva la fisica y vuelve a limpiar velocidad
        if (playerRb != null)
        {
            playerRb.isKinematic = false;
            playerRb.useGravity = true;
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // Restaura movimiento
        if (firstPlayer != null)
            firstPlayer.canMove = true;

        // Detiene la animacion
        if (burroAnimator != null)
            burroAnimator.enabled = false;

        if (audioSource != null)
            audioSource.Stop();
    }
}