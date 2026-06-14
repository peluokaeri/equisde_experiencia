using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuOpcionesBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Construir Menu")]
    void ConstruirMenu()
    {
        GameObject canvasGO = new GameObject("MenuOpcionesCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();
        CanvasGroup cg = canvasGO.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        // Fondo negro total
        GameObject fondo = new GameObject("Fondo");
        fondo.transform.SetParent(canvasGO.transform, false);
        fondo.AddComponent<Image>().color = Color.black;
        EstirarAlCanvas(fondo);

        // Tarjeta negra central
        GameObject tarjeta = new GameObject("Tarjeta");
        tarjeta.transform.SetParent(canvasGO.transform, false);
        tarjeta.AddComponent<Image>().color = Color.black;
        RectTransform tarjetaRT = tarjeta.GetComponent<RectTransform>();
        tarjetaRT.anchorMin = new Vector2(0.5f, 0.5f);
        tarjetaRT.anchorMax = new Vector2(0.5f, 0.5f);
        tarjetaRT.pivot = new Vector2(0.5f, 0.5f);
        tarjetaRT.sizeDelta = new Vector2(860, 640);
        tarjetaRT.anchoredPosition = Vector2.zero;

        // Borde blanco exterior grueso
        CrearBordeLineas(tarjeta, "BordeExt", new Vector2(820, 600), Color.white, 3f);

        // Borde blanco interior fino
        CrearBordeLineas(tarjeta, "BordeInt", new Vector2(790, 570), new Color(1f,1f,1f,0.6f), 1f);

        // Decoracion superior — lineas ornamentales
        CrearRect(tarjeta, "OrnaSup_L", new Color(1f,1f,1f,0.8f), new Vector2(-180, 255), new Vector2(200, 1.5f));
        CrearRect(tarjeta, "OrnaSup_R", new Color(1f,1f,1f,0.8f), new Vector2(180,  255), new Vector2(200, 1.5f));
        CrearRect(tarjeta, "OrnaSup_L2", new Color(1f,1f,1f,0.4f), new Vector2(-180, 248), new Vector2(200, 1f));
        CrearRect(tarjeta, "OrnaSup_R2", new Color(1f,1f,1f,0.4f), new Vector2(180,  248), new Vector2(200, 1f));

        // Decoracion inferior
        CrearRect(tarjeta, "OrnaInf_L",  new Color(1f,1f,1f,0.8f), new Vector2(-180, -255), new Vector2(200, 1.5f));
        CrearRect(tarjeta, "OrnaInf_R",  new Color(1f,1f,1f,0.8f), new Vector2(180,  -255), new Vector2(200, 1.5f));

        // Titulo ornamental
        CrearTMP(tarjeta, "Orna", "— ✦ —", 16, new Color(1f,1f,1f,0.5f),
            new Vector2(0, 260), new Vector2(700, 30), TextAlignmentOptions.Center);

        // Pregunta
        CrearTMP(tarjeta, "Pregunta",
            "¿Qué hace a Chaplin\nINOLVIDABLE?",
            38, Color.white,
            new Vector2(0, 130), new Vector2(780, 150),
            TextAlignmentOptions.Center);

        // Separador
        CrearRect(tarjeta, "Sep", new Color(1f,1f,1f,0.3f), new Vector2(0, 50), new Vector2(600, 1f));

        // Mensaje incorrecto (oculto)
        GameObject msg = CrearTMP(tarjeta, "MensajeIncorrecto",
            "— Sí, pero hay algo más icónico aún… —",
            18, new Color(0.9f, 0.75f, 0.4f),
            new Vector2(0, 15), new Vector2(740, 40),
            TextAlignmentOptions.Center);
        msg.SetActive(false);

        // Botones — bien espaciados
        CrearBoton(tarjeta, "BotonBaston",   "I.",   "El bastón",   new Vector2(0, -70));
        CrearBoton(tarjeta, "BotonSombrero", "II.",  "El sombrero", new Vector2(0, -155));
        CrearBoton(tarjeta, "BotonBigote",   "III.", "El bigote",   new Vector2(0, -240));

        Undo.RegisterCreatedObjectUndo(canvasGO, "Construir Menu Silent Film");
        Debug.Log("Menu Silent Film construido.");
    }

    void EstirarAlCanvas(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void CrearBordeLineas(GameObject padre, string nombre, Vector2 size, Color color, float grosor)
    {
        float w = size.x * 0.5f;
        float h = size.y * 0.5f;
        CrearRect(padre, nombre + "_T", color, new Vector2(0, h),  new Vector2(size.x, grosor));
        CrearRect(padre, nombre + "_B", color, new Vector2(0, -h), new Vector2(size.x, grosor));
        CrearRect(padre, nombre + "_L", color, new Vector2(-w, 0), new Vector2(grosor, size.y));
        CrearRect(padre, nombre + "_R", color, new Vector2(w, 0),  new Vector2(grosor, size.y));
    }

    void CrearRect(GameObject padre, string nombre, Color color, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre.transform, false);
        go.AddComponent<Image>().color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    GameObject CrearTMP(GameObject padre, string nombre, string texto, int size, Color color,
        Vector2 pos, Vector2 sizeDelta, TextAlignmentOptions align)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre.transform, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = align;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = sizeDelta;
        return go;
    }

    void CrearBoton(GameObject padre, string nombre, string numero, string etiqueta, Vector2 pos)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre.transform, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(680, 60);

        Image img = go.AddComponent<Image>();
        img.color = new Color(0,0,0,0);

        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor    = new Color(0,0,0,0);
        cb.highlightedColor = new Color(1f,1f,1f,0.08f);
        cb.pressedColor   = new Color(1f,1f,1f,0.18f);
        cb.selectedColor  = cb.normalColor;
        btn.colors = cb;

        // Linea inferior
        CrearRect(go, "Linea", new Color(1f,1f,1f,0.2f), new Vector2(0,-29), new Vector2(640, 1f));

        // Numero romano
        GameObject goNum = new GameObject("Numero");
        goNum.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmpNum = goNum.AddComponent<TextMeshProUGUI>();
        tmpNum.text = numero;
        tmpNum.fontSize = 13;
        tmpNum.color = new Color(1f,1f,1f,0.4f);
        tmpNum.alignment = TextAlignmentOptions.MidlineLeft;
        RectTransform rtNum = goNum.GetComponent<RectTransform>();
        rtNum.anchorMin = Vector2.zero;
        rtNum.anchorMax = Vector2.one;
        rtNum.offsetMin = new Vector2(30, 0);
        rtNum.offsetMax = new Vector2(-30, 0);

        // Etiqueta
        GameObject goLabel = new GameObject("Label");
        goLabel.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmpLabel = goLabel.AddComponent<TextMeshProUGUI>();
        tmpLabel.text = etiqueta;
        tmpLabel.fontSize = 24;
        tmpLabel.color = Color.white;
        tmpLabel.alignment = TextAlignmentOptions.MidlineLeft;
        RectTransform rtLabel = goLabel.GetComponent<RectTransform>();
        rtLabel.anchorMin = Vector2.zero;
        rtLabel.anchorMax = Vector2.one;
        rtLabel.offsetMin = new Vector2(80, 0);
        rtLabel.offsetMax = new Vector2(-30, 0);

        go.AddComponent<BotonAnimacion>();
    }
#endif
}