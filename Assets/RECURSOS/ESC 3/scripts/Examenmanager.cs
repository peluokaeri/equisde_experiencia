using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ExamenManager : MonoBehaviour
{
    [Header("Mapa")]
    public MapaColorDrop mapaColorDrop; // El RawImage mascara con el script

    [Header("UI del resultado")]
    public TextMeshProUGUI textoNota;

    [Header("Boton para confirmar examen")]
    public Button botonEntregar;
    public TextMeshProUGUI textoBotonEntregar;

    [Header("Camaras")]
    public Camera mainCamera;
    public Camera camera2;
    public GameObject examenCanvas;

    [Header("Jugador")]
    public GameObject player;

    [Header("Examen en mesa (3D)")]
    public GameObject examenMesa;
    public TextMeshPro textoNotaMesa;

    private bool examenEntregado = false;
    private FirstPlayer firstPlayer;

    void Start()
    {
        if (botonEntregar != null)
            botonEntregar.onClick.AddListener(IntentarEntregar);

        if (examenMesa != null)
            examenMesa.SetActive(false);

        if (player != null)
            firstPlayer = player.GetComponent<FirstPlayer>();
    }

    public void IntentarEntregar()
    {
        if (examenEntregado) return;

        int zonasCompletas = mapaColorDrop.GetZonasCompletas();
        int totalZonas = mapaColorDrop.GetTotalZonas();

        if (zonasCompletas < totalZonas)
        {
            int faltantes = totalZonas - zonasCompletas;
            if (textoBotonEntregar != null)
                textoBotonEntregar.text = $"Faltan {faltantes} provincia{(faltantes > 1 ? "s" : "")}";

            StartCoroutine(RestablecerTextoBoton());
            return;
        }

        EntregarExamen();
    }

    private IEnumerator RestablecerTextoBoton()
    {
        yield return new WaitForSeconds(1.5f);
        if (textoBotonEntregar != null)
            textoBotonEntregar.text = "Entregar examen";
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
        yield return new WaitForSeconds(0.5f);

        if (textoNota != null)
        {
            textoNota.gameObject.SetActive(true);
            textoNota.text = $"{nota} / 10";
        }

        yield return new WaitForSeconds(3f);

        if (examenCanvas != null)
            examenCanvas.SetActive(false);

        if (camera2 != null)
            camera2.gameObject.SetActive(false);

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);

        if (player != null)
            player.SetActive(true);

        if (firstPlayer != null)
            firstPlayer.canMove = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (examenMesa != null)
        {
            examenMesa.SetActive(true);
            if (textoNotaMesa != null)
                textoNotaMesa.text = $"{nota} / 10";
        }
    }
}