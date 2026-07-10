using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ParticleSystem))]
public class NieblaSuperficie : MonoBehaviour
{
    void OnEnable()
    {
        Configurar();
    }

    void Configurar()
    {
        var ps = GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // ── MAIN ─────────────────────────────────────────────
        var main = ps.main;
        main.loop = true;
        main.duration = 10f;
        main.startLifetime  = new ParticleSystem.MinMaxCurve(8f, 16f);
        main.startSpeed     = new ParticleSystem.MinMaxCurve(0.1f, 0.4f);
        main.startSize      = new ParticleSystem.MinMaxCurve(3f, 10f);  // Muy grandes
        main.startRotation  = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        main.startColor     = new ParticleSystem.MinMaxGradient(
            new Color(0.7f, 0.8f, 1f, 0.04f),
            new Color(0.85f, 0.9f, 1f, 0.08f));
        main.gravityModifier  = 0f;
        main.maxParticles   = 80;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake    = true;

        // ── EMISION ──────────────────────────────────────────
        var emission = ps.emission;
        emission.rateOverTime = 5f;

        // ── FORMA — caja plana al nivel del suelo ────────────
        var shape = ps.shape;
        shape.enabled    = true;
        shape.shapeType  = ParticleSystemShapeType.Box;
        shape.scale      = new Vector3(40f, 0.5f, 40f); // Muy plana
        shape.position   = new Vector3(0f, 0.3f, 0f);   // Justo sobre el suelo

        // ── COLOR — muy transparente, desaparece suavemente ──
        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.7f, 0.82f, 1f), 0f),
                new GradientColorKey(new Color(0.85f, 0.92f, 1f), 0.5f),
                new GradientColorKey(new Color(0.75f, 0.85f, 1f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f,    0f),
                new GradientAlphaKey(0.07f, 0.2f),
                new GradientAlphaKey(0.07f, 0.8f),
                new GradientAlphaKey(0f,    1f)
            });
        col.color = new ParticleSystem.MinMaxGradient(grad);

        // ── TAMANIO — crece lentamente ────────────────────────
        var size = ps.sizeOverLifetime;
        size.enabled = true;
        AnimationCurve sc = new AnimationCurve();
        sc.AddKey(0f,   0.3f);
        sc.AddKey(0.3f, 0.8f);
        sc.AddKey(0.7f, 1f);
        sc.AddKey(1f,   1.2f);
        size.size = new ParticleSystem.MinMaxCurve(1f, sc);

        // ── ROTACION lenta ────────────────────────────────────
        var rot = ps.rotationOverLifetime;
        rot.enabled = true;
        rot.z = new ParticleSystem.MinMaxCurve(
            -8f * Mathf.Deg2Rad,
             8f * Mathf.Deg2Rad);

        // ── RUIDO suave para movimiento organico ──────────────
        var noise = ps.noise;
        noise.enabled     = true;
        noise.strength    = 0.4f;
        noise.frequency   = 0.08f;
        noise.scrollSpeed = 0.05f;
        noise.octaveCount = 2;
        noise.quality     = ParticleSystemNoiseQuality.Medium;

        // ── RENDERER ──────────────────────────────────────────
        var rend = ps.GetComponent<ParticleSystemRenderer>();
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        rend.sortingFudge = 5f;
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rend.receiveShadows = false;

        ps.Play();
    }
}