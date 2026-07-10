using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class ParticulasDestellos : MonoBehaviour
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
        main.duration = 5f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0f, 0.1f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.3f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 0.92f, 0.4f, 1f),
            new Color(1f, 0.88f, 0.3f, 1f));
        main.maxParticles = 80;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0f;
        main.playOnAwake = true;

        var emission = ps.emission;
        emission.rateOverTime = 8f;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 3, 6, 3, 1.5f),
            new ParticleSystem.Burst(2f, 2, 5, 2, 2f),
        });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(25f, 8f, 25f);

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 0.95f, 0.55f), 0f),
                new GradientColorKey(new Color(1f, 0.95f, 0.55f), 0.4f),
                new GradientColorKey(new Color(1f, 0.85f, 0.25f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.8f, 0.3f),
                new GradientAlphaKey(0f, 1f)
            });
        col.color = new ParticleSystem.MinMaxGradient(grad);

        var size = ps.sizeOverLifetime;
        size.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 0.1f);
        sizeCurve.AddKey(0.15f, 1f);
        sizeCurve.AddKey(1f, 0f);
        size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        ps.Play();
    }
}