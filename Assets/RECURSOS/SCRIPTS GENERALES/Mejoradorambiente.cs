using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class MejoradorAmbiente : MonoBehaviour
{
    [Header("Camara HDR")]
    public Camera mainCamera;
    public bool activarHDR = true;
    public bool activarMSAA = true;

    [Header("Luz ambiental")]
    public Color colorAmbienteEcielo = new Color(0.15f, 0.2f, 0.35f);
    public Color colorAmbienteEcuador = new Color(0.1f, 0.1f, 0.12f);
    public Color colorAmbienteSuelo = new Color(0.05f, 0.05f, 0.08f);
    [Range(0f, 2f)] public float intensidadAmbiental = 0.4f;

    [Header("Sombras")]
    public bool sombrasSupaves = true;
    public float distanciaSombras = 50f;

    [Header("Niebla sutil")]
    public bool activarNiebla = false;
    public Color colorNiebla = new Color(0.5f, 0.6f, 0.8f);
    [Range(0f, 0.05f)] public float densidadNiebla = 0.01f;

    void OnEnable() => Aplicar();
    void OnValidate() => Aplicar();

    void Aplicar()
    {
        // ── CAMARA ────────────────────────────────────────────
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            mainCamera.allowHDR = activarHDR;
            mainCamera.allowMSAA = activarMSAA;
        }

        // ── LUZ AMBIENTAL (ilumina todo desde el cielo) ───────
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor     = colorAmbienteEcielo;
        RenderSettings.ambientEquatorColor = colorAmbienteEcuador;
        RenderSettings.ambientGroundColor  = colorAmbienteSuelo;
        RenderSettings.ambientIntensity    = intensidadAmbiental;

        // ── SOMBRAS ───────────────────────────────────────────
        QualitySettings.shadows = sombrasSupaves ?
            ShadowQuality.All : ShadowQuality.HardOnly;
        QualitySettings.shadowDistance    = distanciaSombras;
        QualitySettings.shadowResolution  = ShadowResolution.VeryHigh;
        QualitySettings.shadowProjection  = ShadowProjection.CloseFit;
        QualitySettings.shadowCascades    = 4;

        // ── NIEBLA SUTIL ──────────────────────────────────────
        RenderSettings.fog        = activarNiebla;
        RenderSettings.fogColor   = colorNiebla;
        RenderSettings.fogMode    = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = densidadNiebla;
    }
}