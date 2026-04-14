using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableNumero : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Identificacion")]
    public int valor; // El numero que representa esta tarjeta

    [Header("Referencias")]
    public Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 posicionOriginal;
    private Transform padreOriginal;
    [HideInInspector] public DropZoneHueco huecoActual = null;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        posicionOriginal = rectTransform.anchoredPosition;
        padreOriginal = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Si estaba en un hueco lo libera
        if (huecoActual != null)
        {
            huecoActual.LiberarZona();
            huecoActual = null;
        }

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void ColocarEnHueco(DropZoneHueco hueco)
    {
        huecoActual = hueco;
    }
}