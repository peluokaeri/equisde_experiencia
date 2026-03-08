using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ExamenManager : MonoBehaviour
{
    [Header("Zonas del mapa")]
    public DropZoneProvincia[] dropZones;     // Arrastra las 10 zonas aqui en el inspector

    [Header("UI del resultado")]
    public TextMeshProUGUI textoNota;         // TMP directo sobre el mapa, desactivado al inicio

    [Header("Boton para confirmar examen")]
    public Button botonEntregar;             // Boton "Entregar examen"
    public TextMeshProUGUI textoBotonEntregar; // Texto del boton (para cambiar a "Completa el mapa")

    // Estado interno
    private int totalZonas;
    private bool examenEntregado = false;

    void Start()
    {
        totalZonas = dropZones.Length;

        if (botonEntregar != null)
            botonEntregar.onClick.AddListener(IntentarEntregar);
    }

    // Verifica si todas las zonas tienen tarjeta antes de permitir entregar
    public void IntentarEntregar()
    {
        if (examenEntregado) return;

        int zonasCompletas = 0;
        foreach (var zona in dropZones)
        {
            if (zona.EstaOcupada) zonasCompletas++;
        }

        if (zonasCompletas < totalZonas)
        {
            // Feedback: todavia faltan provincias
            int faltantes = totalZonas - zonasCompletas;
            if (textoBotonEntregar != null)
                textoBotonEntregar.text = $"Faltan {faltantes} provincia{(faltantes > 1 ? "s" : "")}";

            StartCoroutine(RestablecerTextoBoton());
            return;
        }

        // Todas las zonas completas: calcula resultado
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

        // Cuenta aciertos y muestra feedback visual en el mapa
        int aciertos = 0;
        foreach (var zona in dropZones)
        {
            if (zona.MostrarFeedback())
                aciertos++;
        }

        // Calcula nota del 1 al 10
        int nota = CalcularNota(aciertos, totalZonas);

        // Muestra el panel de resultado con fade
        StartCoroutine(MostrarResultado(nota));
    }

    private int CalcularNota(int aciertos, int total)
    {
        // Escala de 1 a 10: 0 aciertos = 1, todos correctos = 10
        float porcentaje = (float)aciertos / total;
        int nota = Mathf.RoundToInt(1 + porcentaje * 9);
        return Mathf.Clamp(nota, 1, 10);
    }

    private IEnumerator MostrarResultado(int nota)
    {
        yield return new WaitForSeconds(0.5f);

        if (textoNota != null)
        {
            textoNota.gameObject.SetActive(true);
            textoNota.text = $"{nota} / 10";
        }
    }

}