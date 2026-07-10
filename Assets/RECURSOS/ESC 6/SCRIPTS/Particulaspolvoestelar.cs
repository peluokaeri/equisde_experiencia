using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class ParticulasPolvoEstelar : MonoBehaviour
{
    void OnEnable()
    {
        Configurar();
    }

    void Configurar()
    {
        var ps = GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.loop = true;
        main.duration = 10f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(5f, 12f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.01f, 0.05f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.01f, 0.06f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 0.9f, 0.5f, 0.4f),
            new Color(1f, 0.95f, 0.6f, 0.8f));
        main.maxParticles = 500;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.02f;
        main.playOnAwake = true;

        var emission = ps.emission;
        emission.rateOverTime = 60f;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(30f, 8f, 30f);

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 0.95f, 0.55f), 0f),
                new GradientColorKey(new Color(1f, 0.95f, 0.55f), 0.5f),
                new GradientColorKey(new Color(1f, 0.9f, 0.4f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.8f, 0.15f),
                new GradientAlphaKey(0.8f, 0.85f),
                new GradientAlphaKey(0f, 1f)
            });
        col.color = new ParticleSystem.MinMaxGradient(grad);

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.15f;
        noise.frequency = 0.1f;
        noise.scrollSpeed = 0.05f;
        noise.quality = ParticleSystemNoiseQuality.High;

        ps.Play();
    }
}