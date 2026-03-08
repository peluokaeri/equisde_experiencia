using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableTarjeta : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Identificacion")]
    public string nombreProvincia; // Debe coincidir exactamente con DropZoneProvincia.provinciaCorrecta

    [Header("Referencias")]
    public Canvas canvas;

    // Estado interno
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 posicionOriginal;
    private Transform padreOriginal;
    public DropZoneProvincia zonaActual = null; // La zona donde esta colocada actualmente

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Si no tiene CanvasGroup, lo agrega automaticamente
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Busca el canvas automaticamente si no fue asignado
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        // Guarda posicion y padre original para poder volver si el drop falla
        posicionOriginal = rectTransform.anchoredPosition;
        padreOriginal = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Si estaba en una zona, la libera
        if (zonaActual != null)
        {
            zonaActual.LiberarZona();
            zonaActual = null;
        }

        // Pasa al padre del canvas para que se dibuje por encima de todo
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        // Hace la tarjeta semi-transparente y desactiva raycasts para que el drop funcione
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Mueve la tarjeta siguiendo el mouse, ajustando por el scale del canvas
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reactiva raycasts y opacidad
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void VolverAOrigen()
    {
        transform.SetParent(padreOriginal);
        rectTransform.anchoredPosition = posicionOriginal;
        zonaActual = null;
    }

    // Llamado por DropZoneProvincia cuando acepta esta tarjeta
    public void ColocarEnZona(DropZoneProvincia zona)
    {
        zonaActual = zona;
    }
}