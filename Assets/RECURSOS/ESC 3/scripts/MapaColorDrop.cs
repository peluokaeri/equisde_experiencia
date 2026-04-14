using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapaColorDrop : MonoBehaviour, IDropHandler
{
    [Header("Textura mascara")]
    public Texture2D texturaMascara;

    [Header("Provincias y sus colores")]
    public ProvinciaColor[] provincias;

    [Header("Boton entregar")]
    public Button botonEntregar; // Desactivado al inicio, se activa cuando se colocan todas

    [System.Serializable]
    public class ProvinciaColor
    {
        public string nombreProvincia;
        public Color color;
    }

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoColocacion; // Sonidito al detectar provincia

    private Dictionary<string, DraggableTarjeta> tarjetasColocadas = new Dictionary<string, DraggableTarjeta>();
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Boton oculto al inicio
        if (botonEntregar != null)
            botonEntregar.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableTarjeta tarjeta = eventData.pointerDrag?.GetComponent<DraggableTarjeta>();
        if (tarjeta == null) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        Rect rect = rectTransform.rect;
        float u = (localPoint.x - rect.x) / rect.width;
        float v = (localPoint.y - rect.y) / rect.height;

        if (u < 0 || u > 1 || v < 0 || v > 1) return;

        int px = Mathf.FloorToInt(u * texturaMascara.width);
        int py = Mathf.FloorToInt(v * texturaMascara.height);
        Color pixelColor = texturaMascara.GetPixel(px, py);

        string provinciaDetectada = DetectarProvincia(pixelColor);

        if (provinciaDetectada == null)
        {
            Debug.Log("No se detectó provincia en ese punto");
            return;
        }

        if (tarjetasColocadas.ContainsKey(provinciaDetectada))
        {
            Debug.Log($"Provincia {provinciaDetectada} ya ocupada");
            return;
        }

        // Registra la tarjeta
        tarjetasColocadas[provinciaDetectada] = tarjeta;
        tarjeta.zonaActual = null;

        // Sonido de colocacion
        if (audioSource != null && sonidoColocacion != null)
            audioSource.PlayOneShot(sonidoColocacion);

        Debug.Log($"Tarjeta '{tarjeta.nombreProvincia}' colocada en '{provinciaDetectada}'");

        // Verifica si se completaron todas las provincias
        VerificarCompleto();
    }

    private void VerificarCompleto()
    {
        if (tarjetasColocadas.Count >= provincias.Length)
        {
            if (botonEntregar != null)
                botonEntregar.gameObject.SetActive(true);
        }
    }

    public void LiberarTarjeta(DraggableTarjeta tarjeta)
    {
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
        {
            tarjetasColocadas.Remove(provinciaAnterior);
            Debug.Log($"Provincia '{provinciaAnterior}' liberada");

            // Oculta el boton si ya no estan todas
            if (botonEntregar != null)
                botonEntregar.gameObject.SetActive(false);
        }
    }

    private string DetectarProvincia(Color pixelColor)
    {
        float threshold = 0.1f;
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