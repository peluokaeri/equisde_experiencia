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

    [Header("Dialogue")]
    public SubtitleController subtitleController;

    private bool playerInside = false;
    private bool used = false;

    void Start()
    {
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        eImage.enabled = false;

        if (camera2 != null)
            camera2.gameObject.SetActive(false);
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
        if (used) return;

        // ✅ Mostrar E solo cuando:
        // - jugador está dentro
        // - no hay diálogo activo
        if (playerInside && !subtitleController.IsDialogueActive)
        {
            eImage.enabled = true;
        }
        else
        {
            eImage.enabled = false;
        }

        if (!playerInside) return;
        if (subtitleController.IsDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            used = true;

            eImage.enabled = false;

            // ❌ Player desaparece
            if (player != null)
                player.SetActive(false);

            // ❌ Luz desaparece
            if (LUZ1 != null)
                LUZ1.SetActive(false);

            // 📷 Cámara principal off
            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false);

            // 📷 Cámara 2 on
            if (camera2 != null)
                camera2.gameObject.SetActive(true);

            // 🎬 Animación cámara
            if (camera2Animator != null)
                camera2Animator.SetTrigger("Play");
        }
    }
}