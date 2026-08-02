using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Efecto de ojos cerrandose con parpados CURVOS (texturas) que bajan y
// suben desde fuera de pantalla, con un parpadeo antes del cierre final.
// Al cerrarse del todo, termina la aplicacion.
public class CierreOjos : MonoBehaviour
{
    [Header("Parpados (Images con textura curva)")]
    public RectTransform parpadoSuperior;   // Image con parpado_superior.png
    public RectTransform parpadoInferior;    // Image con parpado_inferior.png

    [Header("Respaldo negro (garantiza cierre total)")]
    // Panel negro full screen que aparece al final para tapar cualquier
    // ranura que dejen los parpados curvos. Arranca invisible.
    public Image panelNegroFinal;

    [Header("Tiempos")]
    public float tiempoAntesDeEmpezar = 8f;
    public float duracionCierreLento = 4f;
    public float aperturaParpadeo = 0.5f;

    [Header("Parpadeo previo")]
    public int parpadeos = 1;
    [Range(0f, 1f)]
    public float cierreParpadeo = 0.6f;     // Que tanto se cierran en el parpadeo

    [Header("Solapamiento en el centro")]
    // Cuanto se pasan del centro al cerrar. Alto para que la parte curva
    // no deje ranuras en los lados. Subilo si no cierra del todo.
    public float solapamiento = 150f;

    private bool iniciado = false;
    private float alto;

    // Posiciones: cerrado (cubriendo) y abierto (fuera de pantalla)
    private float supAbierto, supCerrado;
    private float infAbierto, infCerrado;

    void Start()
    {
        alto = Screen.height;

        ConfigurarParpado(parpadoSuperior, true);
        ConfigurarParpado(parpadoInferior, false);

        // Alto de cada parpado (el que se le puso en ConfigurarParpado)
        float altoParpado = Screen.height * 0.75f;

        // Posiciones:
        // ABIERTO: el parpado esta fuera de pantalla (su borde interno en el borde)
        // CERRADO: el parpado baja/sube hasta que su borde curvo cruza el centro
        //
        // Superior (pivote centro): abierto = arriba fuera; cerrado = su centro
        // queda de modo que el borde inferior pase el centro de la pantalla.
        supAbierto = alto * 0.5f + altoParpado * 0.5f;  // todo fuera arriba
        supCerrado = altoParpado * 0.5f - solapamiento; // borde inferior cruza el centro

        infAbierto = -(alto * 0.5f + altoParpado * 0.5f); // todo fuera abajo
        infCerrado = -(altoParpado * 0.5f - solapamiento);

        // Arranca abierto (ojos abiertos)
        if (parpadoSuperior != null)
            parpadoSuperior.anchoredPosition = new Vector2(0f, supAbierto);
        if (parpadoInferior != null)
            parpadoInferior.anchoredPosition = new Vector2(0f, infAbierto);

        // El panel negro de respaldo arranca invisible
        if (panelNegroFinal != null)
        {
            Color c = panelNegroFinal.color;
            c.a = 0f;
            panelNegroFinal.color = c;
            panelNegroFinal.gameObject.SetActive(true);
        }
    }

    // Estira el parpado a todo el ancho y le da alto de media pantalla + extra
    private void ConfigurarParpado(RectTransform p, bool superior)
    {
        if (p == null) return;

        // Anclaje centrado horizontal, para moverlo en Y libremente
        p.anchorMin = new Vector2(0.5f, 0.5f);
        p.anchorMax = new Vector2(0.5f, 0.5f);
        p.pivot = new Vector2(0.5f, 0.5f);

        // Ancho = pantalla completa (con un margen extra por las dudas)
        // Alto = mas de media pantalla para cubrir bien
        p.sizeDelta = new Vector2(Screen.width * 1.2f, Screen.height * 0.75f);
    }

    public void IniciarCierre()
    {
        if (iniciado) return;
        iniciado = true;
        StartCoroutine(SecuenciaCierre());
    }

    private IEnumerator SecuenciaCierre()
    {
        yield return new WaitForSeconds(tiempoAntesDeEmpezar);

        // Parpadeo(s) previo(s): baja parcial y vuelve a subir
        for (int i = 0; i < parpadeos; i++)
        {
            float supParcial = Mathf.Lerp(supAbierto, supCerrado, cierreParpadeo);
            float infParcial = Mathf.Lerp(infAbierto, infCerrado, cierreParpadeo);

            yield return StartCoroutine(MoverParpados(supParcial, infParcial, aperturaParpadeo));
            yield return StartCoroutine(MoverParpados(supAbierto, infAbierto, aperturaParpadeo));
            yield return new WaitForSeconds(0.4f);
        }

        // Cierre final lento y completo
        // Los parpados se juntan Y el panel negro aparece para tapar todo
        StartCoroutine(FundirPanelNegro(duracionCierreLento));
        yield return StartCoroutine(MoverParpados(supCerrado, infCerrado, duracionCierreLento));

        // Asegura negro total
        if (panelNegroFinal != null)
        {
            Color c = panelNegroFinal.color;
            c.a = 1f;
            panelNegroFinal.color = c;
        }

        // Momento en negro
        yield return new WaitForSeconds(1.5f);

        CerrarAplicacion();
    }

    // Funde el panel negro de respaldo mientras los parpados se cierran.
    // Arranca a mitad del cierre para que primero se vea la forma curva.
    private IEnumerator FundirPanelNegro(float duracionTotal)
    {
        if (panelNegroFinal == null) yield break;

        // Espera a que el cierre este avanzado (60%) antes de empezar a tapar
        yield return new WaitForSeconds(duracionTotal * 0.6f);

        float dur = duracionTotal * 0.4f;
        float t = 0f;
        Color c = panelNegroFinal.color;

        while (t < dur)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / dur);
            panelNegroFinal.color = c;
            yield return null;
        }
        c.a = 1f;
        panelNegroFinal.color = c;
    }

    private IEnumerator MoverParpados(float destinoSup, float destinoInf, float duracion)
    {
        float supInicial = parpadoSuperior != null ? parpadoSuperior.anchoredPosition.y : 0f;
        float infInicial = parpadoInferior != null ? parpadoInferior.anchoredPosition.y : 0f;

        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float k = t / duracion;
            // Ease in-out suave (como el movimiento real de un parpado)
            float suave = k * k * (3f - 2f * k);

            if (parpadoSuperior != null)
                parpadoSuperior.anchoredPosition = new Vector2(0f, Mathf.Lerp(supInicial, destinoSup, suave));
            if (parpadoInferior != null)
                parpadoInferior.anchoredPosition = new Vector2(0f, Mathf.Lerp(infInicial, destinoInf, suave));

            yield return null;
        }

        if (parpadoSuperior != null)
            parpadoSuperior.anchoredPosition = new Vector2(0f, destinoSup);
        if (parpadoInferior != null)
            parpadoInferior.anchoredPosition = new Vector2(0f, destinoInf);
    }

    private void CerrarAplicacion()
    {
        Debug.Log("Fin de la experiencia - cerrando aplicacion");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}