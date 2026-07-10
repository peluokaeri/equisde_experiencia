using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class ParticulasEstrellas : MonoBehaviour
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
        main.duration = 8f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(3f, 7f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.02f, 0.08f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.12f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 0.92f, 0.5f, 0.6f),
            new Color(1f, 0.85f, 0.3f, 0.9f));
        main.maxParticles = 300;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0f;
        main.playOnAwake = true;

        var emission = ps.emission;
        emission.rateOverTime = 40f;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(20f, 10f, 20f);

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 0.9f, 0.4f), 0f),
                new GradientColorKey(new Color(1f, 0.95f, 0.55f), 0.5f),
                new GradientColorKey(new Color(1f, 0.85f, 0.3f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.3f),
                new GradientAlphaKey(1f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            });
        col.color = new ParticleSystem.MinMaxGradient(grad);

        var size = ps.sizeOverLifetime;
        size.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 0f);
        sizeCurve.AddKey(0.3f, 1f);
        sizeCurve.AddKey(0.6f, 0.7f);
        sizeCurve.AddKey(1f, 0f);
        size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.3f;
        noise.frequency = 0.2f;
        noise.scrollSpeed = 0.1f;
        noise.quality = ParticleSystemNoiseQuality.High;

        ps.Play();
    }
}