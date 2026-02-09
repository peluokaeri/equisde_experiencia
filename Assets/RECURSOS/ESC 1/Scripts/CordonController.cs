using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CordonController : MonoBehaviour
{
    [Header("References")]
    public Animator cordonAnimator;
    public AudioSource cordonAudio;

    private bool started = false;

    void Start()
    {
        // Aseguramos que no arranque solo
        if (cordonAnimator != null)
            cordonAnimator.enabled = false;

        if (cordonAudio != null)
            cordonAudio.Stop();
    }

    public void PlayCordon()
    {
        if (started) return;
        started = true;

        // ▶️ Animación
        if (cordonAnimator != null)
        {
            cordonAnimator.enabled = true;
            cordonAnimator.Play("Cordonmov", 0, 0f);
        }

        // 🔊 Sonido (se mueve con el objeto)
        if (cordonAudio != null)
            cordonAudio.Play();
    }
}
