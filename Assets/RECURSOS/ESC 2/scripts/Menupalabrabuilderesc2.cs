using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuPalabraBuilderEsc2 : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Construir Menu Palabra")]
    void ConstruirMenu()
    {
        // Canvas
        GameObject canvasGO = new GameObject("MenuPalabraCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();
        CanvasGroup cg = canvasGO.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        // Fondo negro completo
        GameObject fondo = new GameObject("Fondo");
        fondo.transform.SetParent(canvasGO.transform, false);
        Image fondoImg = fondo.AddComponent<Image>();
        fondoImg.color = new Color(0.02f, 0.02f, 0.02f, 1f);
        RectTransform fondoRT = fondo.GetComponent<RectTransform>();
        fondoRT.anchorMin = Vector2.zero;
        fondoRT.anchorMax = Vector2.one;
        fondoRT.offsetMin = Vector2.zero;
        fondoRT.offsetMax = Vector2.zero;

        // Pregunta en serif italica
        GameObject pregunta = new GameObject("Pregunta");
        pregunta.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI tmpPreg = pregunta.AddComponent<TextMeshProUGUI>();
        tmpPreg.text = "¿Qué palabra aprendiste hoy?";
        tmpPreg.fontSize = 34;
        tmpPreg.color = new Color(0.54f, 0.54f, 0.54f);
        tmpPreg.alignment = TextAlignmentOptions.Center;
        tmpPreg.fontStyle = FontStyles.Italic;
        RectTransform pregRT = pregunta.GetComponent<RectTransform>();
        pregRT.anchorMin = new Vector2(0.5f, 0.5f);
        pregRT.anchorMax = new Vector2(0.5f, 0.5f);
        pregRT.pivot = new Vector2(0.5f, 0.5f);
        pregRT.anchoredPosition = new Vector2(0, 90);
        pregRT.sizeDelta = new Vector2(900, 60);

        // Texto ingresado
        GameObject input = new GameObject("TextoInput");
        input.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI tmpInput = input.AddComponent<TextMeshProUGUI>();
        tmpInput.text = "";
        tmpInput.fontSize = 46;
        tmpInput.color = new Color(0.91f, 0.91f, 0.91f);
        tmpInput.alignment = TextAlignmentOptions.Center;
        tmpInput.characterSpacing = 18f;
        RectTransform inputRT = input.GetComponent<RectTransform>();
        inputRT.anchorMin = new Vector2(0.5f, 0.5f);
        inputRT.anchorMax = new Vector2(0.5f, 0.5f);
        inputRT.pivot = new Vector2(0.5f, 0.5f);
        inputRT.anchoredPosition = new Vector2(0, -10);
        inputRT.sizeDelta = new Vector2(700, 70);

        // Linea inferior sutil
        GameObject linea = new GameObject("Linea");
        linea.transform.SetParent(canvasGO.transform, false);
        Image lineaImg = linea.AddComponent<Image>();
        lineaImg.color = new Color(0.2f, 0.2f, 0.2f);
        RectTransform lineaRT = linea.GetComponent<RectTransform>();
        lineaRT.anchorMin = new Vector2(0.5f, 0.5f);
        lineaRT.anchorMax = new Vector2(0.5f, 0.5f);
        lineaRT.pivot = new Vector2(0.5f, 0.5f);
        lineaRT.anchoredPosition = new Vector2(0, -55);
        lineaRT.sizeDelta = new Vector2(420, 1.5f);

        // Cursor parpadeante
        GameObject cursor = new GameObject("Cursor");
        cursor.transform.SetParent(canvasGO.transform, false);
        Image cursorImg = cursor.AddComponent<Image>();
        cursorImg.color = new Color(0.91f, 0.91f, 0.91f);
        RectTransform cursorRT = cursor.GetComponent<RectTransform>();
        cursorRT.anchorMin = new Vector2(0.5f, 0.5f);
        cursorRT.anchorMax = new Vector2(0.5f, 0.5f);
        cursorRT.pivot = new Vector2(0.5f, 0.5f);
        cursorRT.anchoredPosition = new Vector2(20, -10);
        cursorRT.sizeDelta = new Vector2(2f, 44f);

        Undo.RegisterCreatedObjectUndo(canvasGO, "Construir Menu Palabra");
        Debug.Log("Menu palabra construido. Asigná los campos al ContadorInteracciones.");
    }
#endif
}