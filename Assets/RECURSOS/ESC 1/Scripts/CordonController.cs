using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CordonController : MonoBehaviour
{
    [Header("References")]
    public Animator cordonAnimator;
    public AudioSource cordonAudio;

    [Header("Nombre del estado de animacion")]
    public string nombreAnimacion = "Cordonmov";

    private bool started = false;

    void Start()
    {
        if (cordonAnimator != null)
            cordonAnimator.enabled = false;

        if (cordonAudio != null)
            cordonAudio.Stop();
    }

    // Se dispara solo cuando el player entra a ESTE trigger
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PlayCordon();
    }

    public void PlayCordon()
    {
        if (started) return;
        started = true;

        if (cordonAnimator != null)
        {
            cordonAnimator.enabled = true;
            cordonAnimator.Play(nombreAnimacion, 0, 0f);
        }

        if (cordonAudio != null)
            cordonAudio.Play();
    }
}