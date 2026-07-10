using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class NieblaVolumetrica : MonoBehaviour
{
    [Header("Configuracion niebla")]
    public Color colorNiebla = new Color(0.5f, 0.65f, 0.9f, 1f);
    [Range(0f, 0.1f)]
    public float densidad = 0.015f;
    [Range(0f, 500f)]
    public float inicio = 5f;
    [Range(0f, 500f)]
    public float fin = 80f;

    void OnEnable()
    {
        ConfigurarNiebla();
    }

    void OnValidate()
    {
        ConfigurarNiebla();
    }

    void ConfigurarNiebla()
    {
        // Niebla global de Unity
        RenderSettings.fog = true;
        RenderSettings.fogColor = colorNiebla;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = inicio;
        RenderSettings.fogEndDistance = fin;
        RenderSettings.fogDensity = densidad;
    }

    void OnDisable()
    {
        RenderSettings.fog = false;
    }
}