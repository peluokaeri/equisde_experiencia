using UnityEngine;
using UnityEngine.UI;

public class SillaBanco2Interaction : MonoBehaviour
{
    [Header("UI")]
    public GameObject instruccionCanvas;
    public Image eImage;

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera camera2;
    public Animator camera2Animator;

    [Header("Dialogo requerido")]
    public SubtitleController subtitleController; // Dialogo examen2col debe terminar

    [Header("Examen")]
    public GameObject examenMatCanvas;

    [Header("Luz")]
    public GameObject luz;

    [Header("Jugador")]
    public GameObject player;

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

        if (examenMatCanvas != null)
            examenMatCanvas.SetActive(false);

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

        // E solo aparece si el dialogo examen2col termino
        bool dialogoTerminado = subtitleController != null && !subtitleController.IsDialogueActive;
        eImage.enabled = playerInside && dialogoTerminado;

        if (!playerInside) return;
        if (!dialogoTerminado) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            used = true;
            eImage.enabled = false;

            if (firstPlayer != null)
                firstPlayer.canMove = false;

            // Oculta el player visualmente sin desactivarlo
            if (player != null)
            {
                MeshRenderer[] renderers = player.GetComponentsInChildren<MeshRenderer>();
                foreach (var r in renderers) r.enabled = false;
            }

            if (luz != null)
            {
                Light lightComponent = luz.GetComponent<Light>();
                if (lightComponent != null)
                    lightComponent.enabled = false;
            }

            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false);

            if (camera2 != null)
                camera2.gameObject.SetActive(true);

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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (examenMatCanvas != null)
            examenMatCanvas.SetActive(true);
    }
}