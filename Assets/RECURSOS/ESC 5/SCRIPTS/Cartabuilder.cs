using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CartaBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Construir Carta")]
    void ConstruirCarta()
    {
        // ── CANVAS ──────────────────────────────────────────
        GameObject canvasGO = new GameObject("CartaCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();
        CanvasGroup cg = canvasGO.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        // ── FONDO OSCURO ─────────────────────────────────────
        GameObject fondo = new GameObject("Fondo");
        fondo.transform.SetParent(canvasGO.transform, false);
        fondo.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
        Stretch(fondo);

        // ── CARTA (panel crema) ──────────────────────────────
        GameObject carta = new GameObject("Carta");
        carta.transform.SetParent(canvasGO.transform, false);
        carta.AddComponent<Image>().color = new Color(0.96f, 0.93f, 0.84f);
        SetRT(carta.GetComponent<RectTransform>(),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 40), new Vector2(680, 750));

        // Lineas decorativas
        Rect2D(carta, "LineaSup", new Color(0.6f,0.45f,0.25f,0.4f), new Vector2(0, 340),  new Vector2(580, 2));
        Rect2D(carta, "LineaInf", new Color(0.6f,0.45f,0.25f,0.4f), new Vector2(0, -340), new Vector2(580, 2));

        // Titulo
        GameObject titulo = new GameObject("Titulo");
        titulo.transform.SetParent(carta.transform, false);
        var tmpT = titulo.AddComponent<TextMeshProUGUI>();
        tmpT.text = "Querido/a...";
        tmpT.fontSize = 20;
        tmpT.color = new Color(0.45f, 0.28f, 0.1f);
        tmpT.fontStyle = FontStyles.Italic;
        tmpT.alignment = TextAlignmentOptions.Left;
        SetRT(titulo.GetComponent<RectTransform>(),
            new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f),
            new Vector2(0, 318), new Vector2(580, 36));

        // ── SCROLLVIEW ───────────────────────────────────────
        GameObject scrollGO = new GameObject("ScrollView");
        scrollGO.transform.SetParent(carta.transform, false);
        var scrollImg = scrollGO.AddComponent<Image>();
        scrollImg.color = new Color(0,0,0,0);
        scrollImg.raycastTarget = true;
        var scrollRect = scrollGO.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 40f;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        SetRT(scrollGO.GetComponent<RectTransform>(),
            new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f),
            new Vector2(0, -20), new Vector2(600, 600));

        // ── VIEWPORT con RectMask2D ──────────────────────────
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollGO.transform, false);
        viewport.AddComponent<RectMask2D>();
        var vpRT = viewport.GetComponent<RectTransform>();
        vpRT.anchorMin = Vector2.zero;
        vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = Vector2.zero;
        vpRT.offsetMax = Vector2.zero;
        vpRT.pivot = new Vector2(0.5f, 0.5f);

        // ── CONTENT ──────────────────────────────────────────
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRT = content.AddComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot    = new Vector2(0.5f, 1);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = new Vector2(0, 1200);

        // ── TEXTO CARTA ───────────────────────────────────────
        GameObject textoGO = new GameObject("TextoCarta");
        textoGO.transform.SetParent(content.transform, false);
        var tmpTexto = textoGO.AddComponent<TextMeshProUGUI>();
        tmpTexto.text = "Escribí aquí el contenido de la carta...";
        tmpTexto.fontSize = 22;
        tmpTexto.color = new Color(0.15f, 0.1f, 0.05f);
        tmpTexto.alignment = TextAlignmentOptions.TopLeft;
        tmpTexto.enableWordWrapping = true;
        tmpTexto.overflowMode = TextOverflowModes.Overflow;
        var textoRT = textoGO.GetComponent<RectTransform>();
        textoRT.anchorMin = Vector2.zero;
        textoRT.anchorMax = Vector2.one;
        textoRT.offsetMin = new Vector2(16, 16);
        textoRT.offsetMax = new Vector2(-16, -16);

        // Indicador de scroll
        GameObject indicador = new GameObject("IndicadorScroll");
        indicador.transform.SetParent(carta.transform, false);
        TextMeshProUGUI tmpInd = indicador.AddComponent<TextMeshProUGUI>();
        tmpInd.text = "▼ deslizá para leer ▼";
        tmpInd.fontSize = 14;
        tmpInd.color = new Color(0.45f, 0.28f, 0.1f, 0.7f);
        tmpInd.alignment = TextAlignmentOptions.Center;
        tmpInd.fontStyle = FontStyles.Italic;
        SetRT(indicador.GetComponent<RectTransform>(),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -320), new Vector2(580, 28));

        // Conectar ScrollRect
        scrollRect.viewport = vpRT;
        scrollRect.content  = contentRT;

        // ── BOTON CERRAR (fuera de la carta) ─────────────────
        GameObject btnGO = new GameObject("BotonCerrar");
        btnGO.transform.SetParent(canvasGO.transform, false);
        var btnImg = btnGO.AddComponent<Image>();
        btnImg.color = new Color(0.15f, 0.1f, 0.05f, 0.9f);
        SetRT(btnGO.GetComponent<RectTransform>(),
            new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f),
            new Vector2(0, -365), new Vector2(220, 50));

        var btn = btnGO.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor      = Color.white;
        cb.highlightedColor = new Color(0.85f, 0.85f, 0.85f);
        cb.pressedColor     = new Color(0.7f, 0.7f, 0.7f);
        cb.selectedColor    = cb.normalColor;
        btn.targetGraphic   = btnImg;
        btn.colors = cb;

        GameObject btnLabel = new GameObject("Label");
        btnLabel.transform.SetParent(btnGO.transform, false);
        var btnTMP = btnLabel.AddComponent<TextMeshProUGUI>();
        btnTMP.text = "[ E ] Cerrar carta";
        btnTMP.fontSize = 17;
        btnTMP.color = new Color(0.96f, 0.93f, 0.84f);
        btnTMP.alignment = TextAlignmentOptions.Center;
        btnTMP.fontStyle = FontStyles.Italic;
        var lblRT = btnLabel.GetComponent<RectTransform>();
        lblRT.anchorMin = Vector2.zero;
        lblRT.anchorMax = Vector2.one;
        lblRT.offsetMin = Vector2.zero;
        lblRT.offsetMax = Vector2.zero;

        Undo.RegisterCreatedObjectUndo(canvasGO, "Construir Carta");
        Debug.Log("Carta construida.");
    }

    void Stretch(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void SetRT(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    void Rect2D(GameObject padre, string nombre, Color color, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre.transform, false);
        go.AddComponent<Image>().color = color;
        SetRT(go.GetComponent<RectTransform>(),
            new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), pos, size);
    }
#endif
}