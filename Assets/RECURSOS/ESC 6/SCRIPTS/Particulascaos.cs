using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class ParticulasCaos : MonoBehaviour
{
    void OnEnable()
    {
        Configurar();
    }

    void Configurar()
    {
        var ps = GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // ── MAIN ────────────────────────────────────────────
        var main = ps.main;
        main.loop = true;
        main.duration = 12f;
        main.startLifetime    = new ParticleSystem.MinMaxCurve(4f, 14f);
        main.startSpeed       = new ParticleSystem.MinMaxCurve(-2f, 4f);
        main.startSize        = new ParticleSystem.MinMaxCurve(0.05f, 1.8f);
        main.startRotation    = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 0.95f, 0.3f, 0.9f),
            new Color(1f, 0.6f,  0.1f, 1f));
        main.gravityModifier  = new ParticleSystem.MinMaxCurve(-0.15f, 0.05f);
        main.maxParticles     = 150;
        main.simulationSpace  = ParticleSystemSimulationSpace.World;
        main.playOnAwake      = true;

        // ── EMISION ─────────────────────────────────────────
        var emission = ps.emission;
        emission.rateOverTime = 12f;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 8, 15, 2, 1.5f),
            new ParticleSystem.Burst(6f, 6, 12, 2, 2f),
        });

        // ── FORMA CAOTICA ────────────────────────────────────
        // Hemisferio hacia arriba simulando explosion en el cielo
        var shape = ps.shape;
        shape.enabled    = true;
        shape.shapeType  = ParticleSystemShapeType.Sphere;
        shape.radius     = 8f;
        shape.arc        = 360f;
        shape.radiusThickness = 0.3f;

        // ── COLOR A LO LARGO DEL TIEMPO ──────────────────────
        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 1f, 0.8f),    0f),
                new GradientColorKey(new Color(1f, 0.85f, 0.2f), 0.25f),
                new GradientColorKey(new Color(1f, 0.5f, 0.05f), 0.5f),
                new GradientColorKey(new Color(0.9f, 0.2f, 0.1f),0.75f),
                new GradientColorKey(new Color(0.4f, 0.1f, 0.4f),1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f,   0f),
                new GradientAlphaKey(1f,   0.1f),
                new GradientAlphaKey(0.9f, 0.5f),
                new GradientAlphaKey(0.6f, 0.8f),
                new GradientAlphaKey(0f,   1f)
            });
        col.color = new ParticleSystem.MinMaxGradient(grad);

        // ── TAMANIO PULSANTE ─────────────────────────────────
        var size = ps.sizeOverLifetime;
        size.enabled = true;
        AnimationCurve sc = new AnimationCurve();
        sc.AddKey(0f,    0f);
        sc.AddKey(0.08f, 1.2f);
        sc.AddKey(0.2f,  0.7f);
        sc.AddKey(0.4f,  1f);
        sc.AddKey(0.6f,  0.5f);
        sc.AddKey(0.8f,  0.8f);
        sc.AddKey(1f,    0f);
        size.size = new ParticleSystem.MinMaxCurve(1f, sc);

        // ── VELOCIDAD CAOTICA ────────────────────────────────
        var vel = ps.velocityOverLifetime;
        vel.enabled   = true;
        vel.orbitalX  = new ParticleSystem.MinMaxCurve(-1.5f, 1.5f);
        vel.orbitalY  = new ParticleSystem.MinMaxCurve(-2f,   2f);
        vel.orbitalZ  = new ParticleSystem.MinMaxCurve(-1f,   1f);
        vel.radial    = new ParticleSystem.MinMaxCurve(-0.5f, 1f);
        vel.speedModifier = new ParticleSystem.MinMaxCurve(0.3f, 1.8f);

        // ── RUIDO REDUCIDO ───────────────────────────────────
        var noise = ps.noise;
        noise.enabled      = true;
        noise.strength     = 1.5f;
        noise.frequency    = 0.5f;
        noise.scrollSpeed  = 0.3f;
        noise.octaveCount  = 2;
        noise.quality      = ParticleSystemNoiseQuality.Medium;

        // ── ROTACION SOBRE SI MISMAS ─────────────────────────
        var rot = ps.rotationOverLifetime;
        rot.enabled = true;
        rot.z = new ParticleSystem.MinMaxCurve(
            -180f * Mathf.Deg2Rad,
             180f * Mathf.Deg2Rad);

        // ── RENDERER ─────────────────────────────────────────
        var rend = ps.GetComponent<ParticleSystemRenderer>();
        rend.renderMode       = ParticleSystemRenderMode.Billboard;
        rend.minParticleSize  = 0.005f;
        rend.maxParticleSize  = 0.12f;
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        ps.Play();
    }
}