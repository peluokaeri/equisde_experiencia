using UnityEngine;

// Genera hojas de otono que caen LENTAMENTE, meciendose de lado a lado
// y rotando suavemente. Usa [ExecuteAlways] para verse y acomodarse en
// la ventana de escena sin necesidad de darle play.
[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class HojasCayendo : MonoBehaviour
{
    [Header("Cantidad y area")]
    public int cantidad = 30;
    public Vector3 areaEmision = new Vector3(12f, 0.5f, 12f); // Ancho x alto x profundidad
    public float alturaCaida = 8f;      // Cuanto caen antes de desaparecer

    [Header("Velocidad (lenta)")]
    public float velocidadCaidaMin = 0.3f;
    public float velocidadCaidaMax = 0.7f;

    [Header("Aspecto")]
    public Material materialHoja;       // Material con la textura de hoja
    public float tamanoMin = 0.15f;
    public float tamanoMax = 0.35f;
    public Color tinte = Color.white;   // Tinte (blanco = colores originales de la textura)

    [Header("Movimiento")]
    public float balanceo = 1.5f;       // Cuanto se mecen de lado a lado
    public float rotacionVelocidad = 45f; // Grados por segundo que giran

    private ParticleSystem ps;

    void OnEnable()
    {
        ps = GetComponent<ParticleSystem>();
        Configurar();
    }

    void Configurar()
    {
        if (ps == null) ps = GetComponent<ParticleSystem>();

        var main = ps.main;
        main.startLifetime = alturaCaida / velocidadCaidaMin;
        main.startSpeed = new ParticleSystem.MinMaxCurve(velocidadCaidaMin, velocidadCaidaMax);
        main.startSize = new ParticleSystem.MinMaxCurve(tamanoMin, tamanoMax);
        main.startColor = tinte;
        main.maxParticles = cantidad * 3;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0f;       // La caida la controla startSpeed, no la gravedad
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f); // Rotacion inicial random
        main.loop = true;

        // Las hojas caen hacia abajo: la emision apunta para abajo
        var emission = ps.emission;
        emission.rateOverTime = cantidad / main.startLifetime.constantMax;

        // Forma: caja plana arriba (de donde caen las hojas)
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = areaEmision;
        shape.rotation = new Vector3(90f, 0f, 0f); // Emite hacia abajo

        // Balanceo lateral (mecerse cayendo) con ruido
        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = balanceo;
        noise.frequency = 0.3f;
        noise.scrollSpeed = 0.2f;
        noise.damping = false;

        // Rotacion continua mientras caen
        var rotOverLife = ps.rotationOverLifetime;
        rotOverLife.enabled = true;
        rotOverLife.z = new ParticleSystem.MinMaxCurve(
            rotacionVelocidad * Mathf.Deg2Rad,
            -rotacionVelocidad * Mathf.Deg2Rad
        );

        // Fade in y out suave (aparecen y desaparecen sin cortar)
        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.15f),
                new GradientAlphaKey(1f, 0.85f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        col.color = grad;

        // Aplica el material de la hoja al renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        if (renderer != null && materialHoja != null)
        {
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.material = materialHoja;
        }

        if (!Application.isPlaying)
        {
            // En el editor, simula para que se vea el efecto sin play
            ps.Simulate(2f, true, true);
            ps.Play();
        }
    }
}
