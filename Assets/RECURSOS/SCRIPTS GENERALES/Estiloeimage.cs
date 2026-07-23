using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Acomoda la(s) eImage de la escena: las centra un poco por debajo
// del centro, las achica, y les agrega fade suave + pulso sutil.
// Poner UNA sola instancia por escena.
public class EstiloEImage : MonoBehaviour
{
    [Header("Imagenes E de la escena")]
    // En casi todos los escenarios va 1 sola.
    // En el escenario 5 arrastra todas (una por carta).
    public Image[] eImages;

    [Header("Posicion")]
    // Cuanto por DEBAJO del centro de pantalla (en px del canvas 1920x1080)
    public float offsetY = -120f;

    [Header("Tamano")]
    public float tamano = 42f;

    [Header("Estilo")]
    [Range(0f, 1f)]
    public float opacidadMaxima = 0.75f;
    public float velocidadFade = 6f;

    [Header("Pulso sutil")]
    public bool pulso = true;
    public float pulsoIntensidad = 0.04f;   // Cuanto crece/achica
    public float pulsoVelocidad = 2f;       // Que tan rapido respira

    // Estado interno por imagen
    private bool[] visibleAnterior;
    private float[] alphaActual;

    void Start()
    {
        if (eImages == null || eImages.Length == 0) return;

        visibleAnterior = new bool[eImages.Length];
        alphaActual = new float[eImages.Length];

        for (int i = 0; i < eImages.Length; i++)
        {
            if (eImages[i] == null) continue;

            RectTransform rt = eImages[i].rectTransform;

            // Centrada horizontal, un poco por debajo del centro
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot     = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, offsetY);
            rt.sizeDelta = new Vector2(tamano, tamano);

            // Arranca transparente
            Color c = eImages[i].color;
            c.a = 0f;
            eImages[i].color = c;

            alphaActual[i] = 0f;
            visibleAnterior[i] = false;
        }
    }

    void Update()
    {
        if (eImages == null) return;

        for (int i = 0; i < eImages.Length; i++)
        {
            Image img = eImages[i];
            if (img == null) continue;

            // Los otros scripts prenden/apagan con .enabled.
            // Nosotros lo interpretamos como "deberia verse o no"
            // y hacemos el fade nosotros.
            bool deberiaVerse = img.enabled;

            float destino = deberiaVerse ? opacidadMaxima : 0f;
            alphaActual[i] = Mathf.MoveTowards(
                alphaActual[i], destino, Time.deltaTime * velocidadFade);

            Color c = img.color;
            c.a = alphaActual[i];
            img.color = c;

            // Pulso sutil mientras esta visible
            if (pulso && alphaActual[i] > 0.01f)
            {
                float escala = 1f + Mathf.Sin(Time.time * pulsoVelocidad) * pulsoIntensidad;
                img.rectTransform.localScale = Vector3.one * escala;
            }
            else if (alphaActual[i] <= 0.01f)
            {
                img.rectTransform.localScale = Vector3.one;
            }
        }
    }
}