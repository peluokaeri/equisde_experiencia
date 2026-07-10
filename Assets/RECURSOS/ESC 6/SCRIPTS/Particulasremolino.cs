using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class ParticulasRemolino : MonoBehaviour
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
        main.duration = 6f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(4f, 8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.18f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 0.88f, 0.4f, 0.7f),
            new Color(1f, 0.95f, 0.6f, 0.9f));
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        main.maxParticles = 200;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0f;
        main.playOnAwake = true;

        var emission = ps.emission;
        emission.rateOverTime = 25f;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 6f;
        shape.arc = 360f;
        shape.arcMode = ParticleSystemShapeMultiModeValue.Loop;
        shape.arcSpeed = new ParticleSystem.MinMaxCurve(0.3f);

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 0.82f, 0.2f), 0f),
                new GradientColorKey(new Color(1f, 0.95f, 0.55f), 0.5f),
                new GradientColorKey(new Color(1f, 0.88f, 0.35f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.9f, 0.2f),
                new GradientAlphaKey(0.9f, 0.8f),
                new GradientAlphaKey(0f, 1f)
            });
        col.color = new ParticleSystem.MinMaxGradient(grad);

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.orbitalY = new ParticleSystem.MinMaxCurve(1.5f);
        velocity.radial = new ParticleSystem.MinMaxCurve(-0.2f);

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.8f;
        noise.frequency = 0.4f;
        noise.scrollSpeed = 0.2f;
        noise.octaveCount = 3;
        noise.quality = ParticleSystemNoiseQuality.High;

        var rotOverLifetime = ps.rotationOverLifetime;
        rotOverLifetime.enabled = true;
        rotOverLifetime.z = new ParticleSystem.MinMaxCurve(
            -90f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad);

        ps.Play();
    }
}