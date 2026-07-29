using UnityEngine;

public class SkyboxNegroZona : MonoBehaviour
{
    [Header("Skybox")]
    public Material skyboxNegro;        // Un material skybox negro (opcional)

    [Header("Ambiente")]
    public Color colorAmbienteNegro = Color.black;

    [Header("Global Volume")]
    public GameObject globalVolume;     // El Global Volume a desactivar en la zona

    // Guarda los valores originales (limpios) de la escena
    private Material skyboxOriginal;
    private Color ambienteOriginal;
    private UnityEngine.Rendering.AmbientMode modoAmbienteOriginal;
    private float intensidadOriginal;
    private bool fogOriginal;

    private bool enZonaNegra = false;

    void Awake()
    {
        // Guarda el estado LIMPIO de la escena al arrancar (antes de tocar nada)
        GuardarOriginal();
    }

    void GuardarOriginal()
    {
        skyboxOriginal = RenderSettings.skybox;
        ambienteOriginal = RenderSettings.ambientLight;
        modoAmbienteOriginal = RenderSettings.ambientMode;
        intensidadOriginal = RenderSettings.ambientIntensity;
        fogOriginal = RenderSettings.fog;
    }

    void RestaurarOriginal()
    {
        RenderSettings.skybox = skyboxOriginal;
        RenderSettings.ambientMode = modoAmbienteOriginal;
        RenderSettings.ambientLight = ambienteOriginal;
        RenderSettings.ambientIntensity = intensidadOriginal;
        RenderSettings.fog = fogOriginal;

        if (globalVolume != null)
            globalVolume.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        enZonaNegra = true;

        // Aplica negro puro
        RenderSettings.skybox = skyboxNegro;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = colorAmbienteNegro;
        RenderSettings.fog = false;

        if (globalVolume != null)
            globalVolume.SetActive(false);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        enZonaNegra = false;
        RestaurarOriginal();
    }

    // 🔑 CLAVE: al deshabilitar o destruir (ej: cambio de escena),
    // restaura SIEMPRE para no arrastrar el negro a la escena siguiente.
    void OnDisable()
    {
        if (enZonaNegra)
            RestaurarOriginal();
    }

    void OnDestroy()
    {
        if (enZonaNegra)
            RestaurarOriginal();
    }
}