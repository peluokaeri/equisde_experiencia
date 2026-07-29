using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Efecto de caida del salto de fe: sube la intensidad del Motion Blur
// del Volume progresivamente + un leve zoom de FOV, para dar sensacion
// de velocidad al caer. No toca la rotacion de la camara (se puede mover
// libremente). Requiere un Volume con Motion Blur agregado.
public class EfectoCaida : MonoBehaviour
{
    [Header("Camara")]
    public Camera camara;

    [Header("Volume con Motion Blur")]
    public Volume volume;               // El Global Volume de la escena

    [Header("Motion Blur")]
    public float motionBlurMaximo = 1f; // Intensidad maxima al caer (0-1)

    [Header("Zoom (refuerza la velocidad)")]
    public float fovExtra = 15f;        // Cuanto se abre el FOV al maximo

    [Header("Progresion")]
    public float tiempoHastaMaximo = 2f;

    private bool activo = false;
    private float tiempo = 0f;
    private float fovBase;
    private MotionBlur motionBlur;

    void Start()
    {
        if (camara == null)
            camara = Camera.main;

        if (camara != null)
            fovBase = camara.fieldOfView;

        // Busca el Motion Blur dentro del Volume
        if (volume != null && volume.profile != null)
        {
            if (!volume.profile.TryGet<MotionBlur>(out motionBlur))
            {
                // Si no existe, lo agrega
                motionBlur = volume.profile.Add<MotionBlur>(true);
            }
            motionBlur.intensity.overrideState = true;
            motionBlur.intensity.value = 0f;
        }
    }

    // Llamado por SecuenciaEscaleraFinal al empezar la caida
    public void Activar()
    {
        activo = true;
        tiempo = 0f;
    }

    public void Desactivar()
    {
        activo = false;
        if (camara != null)
            camara.fieldOfView = fovBase;
        if (motionBlur != null)
            motionBlur.intensity.value = 0f;
    }

    void Update()
    {
        if (!activo) return;

        tiempo += Time.deltaTime;

        // Intensidad crece de 0 a 1
        float k = Mathf.Clamp01(tiempo / tiempoHastaMaximo);

        // Sube el Motion Blur progresivamente
        if (motionBlur != null)
            motionBlur.intensity.value = motionBlurMaximo * k;

        // Leve zoom del FOV para reforzar la sensacion de velocidad
        if (camara != null)
            camara.fieldOfView = fovBase + fovExtra * k;
    }
}