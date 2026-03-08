using UnityEngine;
using UnityEngine.UI;

public class SillaExamen1Interaction : MonoBehaviour
{
    [Header("UI")]
    public GameObject instruccionCanvas;
    public Image eImage;

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera camera2;
    public Animator camera2Animator;

    [Header("Objects")]
    public GameObject player;
    public GameObject LUZ1;

    [Header("Examen")]
    public GameObject examenCanvas;

    private FirstPlayer firstPlayer;
    private bool playerInside = false;
    private bool used = false;
    private bool esperandoFinAnimacion = false;

    void Start()
    {
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        eImage.enabled = false;

        if (camera2 != null)
            camera2.gameObject.SetActive(false);

        if (examenCanvas != null)
            examenCanvas.SetActive(false);

        // Obtiene el script del jugador para bloquear movimiento
        if (player != null)
            firstPlayer = player.GetComponent<FirstPlayer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        eImage.enabled = false;
    }

    void Update()
    {
        if (used && esperandoFinAnimacion)
        {
            VerificarFinAnimacion();
            return;
        }

        if (used) return;

        // Mostrar E solo cuando jugador esta dentro
        eImage.enabled = playerInside;

        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            used = true;
            eImage.enabled = false;

            // Bloquea movimiento del jugador
            if (firstPlayer != null)
                firstPlayer.canMove = false;

            // Player desaparece
            if (player != null)
                player.SetActive(false);

            // Luz desaparece
            if (LUZ1 != null)
                LUZ1.SetActive(false);

            // Camara principal off
            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false);

            // Camara 2 on
            if (camera2 != null)
                camera2.gameObject.SetActive(true);

            // Animacion camara
            if (camera2Animator != null)
            {
                camera2Animator.SetTrigger("Play");
                esperandoFinAnimacion = true;
            }
            else
            {
                ActivarExamen();
            }
        }
    }

    private void VerificarFinAnimacion()
    {
        if (camera2Animator == null) return;

        AnimatorStateInfo stateInfo = camera2Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 1f && !camera2Animator.IsInTransition(0))
        {
            esperandoFinAnimacion = false;
            ActivarExamen();
        }
    }

    private void ActivarExamen()
    {
        // Desbloquea el mouse para poder usar el drag & drop
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (examenCanvas != null)
            examenCanvas.SetActive(true);
    }
}