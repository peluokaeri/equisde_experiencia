using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapaColorDrop : MonoBehaviour, IDropHandler
{
    [Header("Textura mascara")]
    public Texture2D texturaMascara; // La imagen mascara con Read/Write habilitado

    [Header("Provincias y sus colores")]
    public ProvinciaColor[] provincias; // Configura en el inspector

    // Diccionario provincia → tarjeta colocada
    private Dictionary<string, DraggableTarjeta> tarjetasColocadas = new Dictionary<string, DraggableTarjeta>();

    private RectTransform rectTransform;
    private Canvas canvas;

    [System.Serializable]
    public class ProvinciaColor
    {
        public string nombreProvincia;
        public Color color;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableTarjeta tarjeta = eventData.pointerDrag?.GetComponent<DraggableTarjeta>();
        if (tarjeta == null) return;

        // Convierte posicion del mouse a coordenadas UV de la textura
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        // Normaliza a 0-1
        Rect rect = rectTransform.rect;
        float u = (localPoint.x - rect.x) / rect.width;
        float v = (localPoint.y - rect.y) / rect.height;

        if (u < 0 || u > 1 || v < 0 || v > 1) return;

        // Samplea el pixel en esa posicion
        int px = Mathf.FloorToInt(u * texturaMascara.width);
        int py = Mathf.FloorToInt(v * texturaMascara.height);
        Color pixelColor = texturaMascara.GetPixel(px, py);

        // Busca la provincia correspondiente al color
        string provinciaDetectada = DetectarProvincia(pixelColor);

        if (provinciaDetectada == null)
        {
            Debug.Log("No se detectó provincia en ese punto");
            return;
        }

        // Si la provincia ya tiene tarjeta, no acepta
        if (tarjetasColocadas.ContainsKey(provinciaDetectada))
        {
            Debug.Log($"Provincia {provinciaDetectada} ya ocupada");
            return;
        }

        // Si la tarjeta ya estaba en otra provincia, la libera
        if (tarjeta.zonaActual != null)
        {
            // Busca y elimina la entrada anterior
            string provinciaAnterior = null;
            foreach (var kvp in tarjetasColocadas)
            {
                if (kvp.Value == tarjeta)
                {
                    provinciaAnterior = kvp.Key;
                    break;
                }
            }
            if (provinciaAnterior != null)
                tarjetasColocadas.Remove(provinciaAnterior);
        }

        // Registra la tarjeta en la provincia
        tarjetasColocadas[provinciaDetectada] = tarjeta;
        tarjeta.zonaActual = null; // Usamos el diccionario en lugar de zonaActual

        Debug.Log($"Tarjeta '{tarjeta.nombreProvincia}' colocada en '{provinciaDetectada}'");
    }

    private string DetectarProvincia(Color pixelColor)
    {
        float threshold = 0.1f; // Tolerancia de color
        foreach (var p in provincias)
        {
            if (ColorDistance(pixelColor, p.color) < threshold)
                return p.nombreProvincia;
        }
        return null;
    }

    private float ColorDistance(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b);
    }

    // Para ExamenManager
    public int GetZonasCompletas()
    {
        return tarjetasColocadas.Count;
    }

    public int GetTotalZonas()
    {
        return provincias.Length;
    }

    public int CalcularAciertos()
    {
        int aciertos = 0;
        foreach (var kvp in tarjetasColocadas)
        {
            if (kvp.Key == kvp.Value.nombreProvincia)
                aciertos++;
        }
        return aciertos;
    }
}