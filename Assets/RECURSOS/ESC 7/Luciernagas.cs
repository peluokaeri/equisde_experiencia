using UnityEngine;

// Genera particulas de luciernagas: puntos de luz calidos que flotan
// suavemente, parpadean y se mueven despacio por el entorno, para dar
// una sensacion de calma y belleza al lugar final.
[RequireComponent(typeof(ParticleSystem))]
public class Luciernagas : MonoBehaviour
{
    [Header("Cantidad y area")]
    public int cantidad = 40;
    public Vector3 area = new Vector3(15f, 5f, 15f);

    [Header("Aspecto")]
    public Color colorLuciernaga = new Color(1f, 0.9f, 0.5f); // Amarillo calido
    public float tamanoMin = 0.03f;
    public float tamanoMax = 0.08f;

    [Header("Movimiento")]
    public float velocidadFlote = 0.3f;   // Que tan rapido derivan
    public float velocidadParpadeo = 1.5f;// Que tan rapido titilan

    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        Configurar();
    }

    void Configurar()
    {
        var main = ps.main;
        main.startLifetime = 999f;        // Viven "para siempre"
        main.startSpeed = 0f;             // No salen disparadas
        main.startSize = new ParticleSystem.MinMaxCurve(tamanoMin, tamanoMax);
        main.startColor = colorLuciernaga;
        main.maxParticles = cantidad;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.loop = true;

        // Emision: todas de una y se mantienen
        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, (short)cantidad)
        });

        // Forma: caja (el area donde flotan)
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = area;

        // Movimiento suave con ruido (derivan como flotando)
        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = velocidadFlote;
        noise.frequency = 0.2f;
        noise.scrollSpeed = 0.1f;
        noise.damping = true;

        // Parpadeo: alpha oscilante via Color over Lifetime
        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(colorLuciernaga, 0f),
                new GradientColorKey(colorLuciernaga, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.2f, 0f),
                new GradientAlphaKey(1f, 0.3f),
                new GradientAlphaKey(0.3f, 0.6f),
                new GradientAlphaKey(1f, 0.8f),
                new GradientAlphaKey(0.2f, 1f)
            }
        );
        col.color = grad;

        // Tamano pulsante para reforzar el titileo
        var sizeOverLife = ps.sizeOverLifetime;
        sizeOverLife.enabled = true;
        AnimationCurve curva = new AnimationCurve();
        curva.AddKey(0f, 0.6f);
        curva.AddKey(0.5f, 1f);
        curva.AddKey(1f, 0.6f);
        sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, curva);

        // Emision de luz: usar un material aditivo para que "brillen"
        ps.Play();
    }
}