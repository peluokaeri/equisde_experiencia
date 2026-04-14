using UnityEngine;
using UnityEngine.UI;

public class SillaBanco2 : MonoBehaviour
{
    [Header("UI")]
    public GameObject instruccionCanvas;
    public Image eImage;

    [Header("Dialogo requerido")]
    public SubtitleController subtitleController;

    [HideInInspector] public bool habilitado = false; // Activado por TriggerAula2 al terminar dialogo

    private bool playerInside = false;

    void Start()
    {
        if (eImage != null)
            eImage.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        if (eImage != null) eImage.enabled = false;
    }

    void Update()
    {
        if (!habilitado) return;

        // Mostrar E solo si el jugador esta dentro y no hay dialogo activo
        bool puedeInteractuar = playerInside &&
                                subtitleController != null &&
                                !subtitleController.IsDialogueActive;

        if (eImage != null)
            eImage.enabled = puedeInteractuar;

        if (puedeInteractuar && Input.GetKeyDown(KeyCode.E))
        {
            // Aca va la logica del examen de matematica
            // Por ahora solo un log para verificar que funciona
            Debug.Log("Banco 2 activado — listo para el examen de matematica");
        }
    }
}