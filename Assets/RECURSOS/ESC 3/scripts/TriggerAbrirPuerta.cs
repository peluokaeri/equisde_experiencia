using UnityEngine;

public class TriggerAbrirPuerta : MonoBehaviour
{
    [Header("Puerta")]
    public Animator puertaAnimator;
    public AudioSource puertaAudioSource;

    private bool abierta = false;

    void OnTriggerEnter(Collider other)
    {
        if (abierta) return;
        if (!other.CompareTag("Player")) return;

        abierta = true;

        if (puertaAnimator != null)
        {
            puertaAnimator.enabled = true;
            puertaAnimator.SetTrigger("Examen2puert");
        }

        if (puertaAudioSource != null)
            puertaAudioSource.Play();

        // Se desactiva a si mismo luego de usarse
        gameObject.SetActive(false);
    }
}