using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ExamenManager : MonoBehaviour
{
    [Header("Mapa")]
    public MapaColorDrop mapaColorDrop;

    [Header("UI del resultado")]
    public TextMeshProUGUI textoNota;

    [Header("Boton para confirmar examen")]
    public Button botonEntregar;

    [Header("Camaras")]
    public Camera mainCamera;
    public Camera camera2;
    public GameObject examenCanvas;

    [Header("Jugador")]
    public GameObject player;

    [Header("Examen en mesa (3D)")]
    public GameObject examenMesa;
    public Renderer examenMesaRenderer;
    public Texture2D[] texturasPorNota;

    [Header("Post examen")]
    public SubtitleController subtitleController;
    public DialogueData dialoguePostExamen;
    public Puerta2AfterDialogue puerta2;    // Script en la puerta del 2do aula

    private bool examenEntregado = false;
    private FirstPlayer firstPlayer;

    void Start()
    {
        if (botonEntregar != null)
            botonEntregar.onClick.AddListener(IntentarEntregar);

        if (examenMesa != null)
            examenMesa.SetActive(false);

        if (player != null)
        {
            firstPlayer = player.GetComponent<FirstPlayer>();
            if (firstPlayer == null)
                firstPlayer = player.GetComponentInChildren<FirstPlayer>();
        }
    }


    public void IntentarEntregar()
    {
        if (examenEntregado) return;

        if (mapaColorDrop == null)
        {
            Debug.LogError("MapaColorDrop no asignado en ExamenManager");
            return;
        }

        EntregarExamen();
    }

    private void EntregarExamen()
    {
        examenEntregado = true;

        int aciertos = mapaColorDrop.CalcularAciertos();
        int total = mapaColorDrop.GetTotalZonas();
        int nota = CalcularNota(aciertos, total);

        StartCoroutine(FinalizarExamen(nota));
    }

    private int CalcularNota(int aciertos, int total)
    {
        float porcentaje = (float)aciertos / total;
        int nota = Mathf.RoundToInt(1 + porcentaje * 9);
        return Mathf.Clamp(nota, 1, 10);
    }

    private IEnumerator FinalizarExamen(int nota)
    {
        // 1 — Muestra nota sobre el mapa
        yield return new WaitForSeconds(0.5f);

        if (textoNota != null)
        {
            textoNota.gameObject.SetActive(true);
            textoNota.text = $"{nota}/10";
        }

        // 2 — Espera 3 segundos
        yield return new WaitForSeconds(3f);

        // 3 — Cierra examen y restaura camaras
        if (examenCanvas != null)
            examenCanvas.SetActive(false);

        if (camera2 != null)
            camera2.gameObject.SetActive(false);

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);

        // 4 — Reactiva jugador visualmente y habilita movimiento
        if (player != null)
        {
            MeshRenderer[] renderers = player.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var r in renderers) r.enabled = true;
        }

        if (firstPlayer != null)
            firstPlayer.canMove = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 5 — Activa examen en mesa con textura correcta
        if (examenMesa != null)
        {
            examenMesa.SetActive(true);
            int indice = Mathf.Clamp(nota - 1, 0, 9);
            if (examenMesaRenderer != null && texturasPorNota != null &&
                texturasPorNota.Length > indice && texturasPorNota[indice] != null)
                examenMesaRenderer.material.mainTexture = texturasPorNota[indice];
        }

        // 6 — Inicia dialogo y activa la espera de la puerta
        if (subtitleController != null && dialoguePostExamen != null)
        {
            subtitleController.PlayDialogue(dialoguePostExamen);

            if (puerta2 != null)
                puerta2.IniciarEspera();
        }
        else
        {
            Debug.LogError("SubtitleController o DialoguePostExamen no asignado");
        }
    }
}