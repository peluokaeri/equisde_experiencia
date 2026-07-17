using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class RevertirAmbiente : MonoBehaviour
{
    void OnEnable()
    {
        Revertir();
    }

    [ContextMenu("Revertir ahora")]
    void Revertir()
    {
        // Camara
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.allowHDR = true;
            cam.allowMSAA = true;
        }

        // Luz ambiental - default Unity
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 1f;
        RenderSettings.ambientSkyColor = new Color(0.212f, 0.227f, 0.259f);
        RenderSettings.ambientEquatorColor = new Color(0.114f, 0.125f, 0.133f);
        RenderSettings.ambientGroundColor = new Color(0.047f, 0.043f, 0.035f);

        // Sombras - default Unity
        QualitySettings.shadows = ShadowQuality.All;
        QualitySettings.shadowDistance = 150f;
        QualitySettings.shadowResolution = ShadowResolution.Medium;
        QualitySettings.shadowProjection = ShadowProjection.StableFit;
        QualitySettings.shadowCascades = 2;

        // Niebla - desactivada
        RenderSettings.fog = false;

        Debug.Log("Ambiente revertido a valores por defecto.");
    }
}