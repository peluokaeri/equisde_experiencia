using UnityEngine;

public class SkyboxNegroZona : MonoBehaviour
{
    [Header("Skybox")]
    public Material skyboxNegro;        // Un material skybox negro (opcional)

    [Header("Ambiente")]
    public Color colorAmbienteNegro = Color.black;

    [Header("Global Volume")]
    public GameObject globalVolume;     // El Global Volume a desactivar en la zona

    // Guarda los valores originales para restaurar
    private Material skyboxOriginal;
    private Color ambienteOriginal;
    private UnityEngine.Rendering.AmbientMode modoAmbienteOriginal;
    private bool fogOriginal;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Guarda estado actual
        skyboxOriginal = RenderSettings.skybox;
        ambienteOriginal = RenderSettings.ambientLight;
        modoAmbienteOriginal = RenderSettings.ambientMode;
        fogOriginal = RenderSettings.fog;

        // Aplica negro puro
        RenderSettings.skybox = skyboxNegro; // Si es null, el fondo usa el color de camara
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = colorAmbienteNegro;
        RenderSettings.fog = false;

        // Desactiva el global volume
        if (globalVolume != null)
            globalVolume.SetActive(false);

        DynamicGI.UpdateEnvironment();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Restaura estado original
        RenderSettings.skybox = skyboxOriginal;
        RenderSettings.ambientMode = modoAmbienteOriginal;
        RenderSettings.ambientLight = ambienteOriginal;
        RenderSettings.fog = fogOriginal;

        // Reactiva el global volume
        if (globalVolume != null)
            globalVolume.SetActive(true);

        DynamicGI.UpdateEnvironment();
    }
}