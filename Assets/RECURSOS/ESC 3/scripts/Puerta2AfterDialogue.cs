using System.Collections;
using UnityEngine;

public class Puerta2AfterDialogue : MonoBehaviour
{
    public SubtitleController subtitleController;
    public Animator animator;
    public AudioSource audioSource;

    private bool used = false;
    private bool esperaIniciada = false;

    void Awake()
    {
        // Apaga el Animator lo antes posible (antes que cualquier Start)
        if (animator != null)
        {
            animator.enabled = false;
            // Rebobina cualquier animacion al frame 0 por las dudas
            animator.Rebind();
            animator.enabled = false;
        }
    }

    void Start()
    {
        // Refuerza que quede apagado al arrancar la escena
        if (animator != null)
            animator.enabled = false;
    }

    void Update()
    {
        // Mientras no se haya iniciado la espera, mantiene el Animator
        // FORZADAMENTE apagado, por si algo lo reactiva por error.
        if (!esperaIniciada && animator != null && animator.enabled)
            animator.enabled = false;
    }

    // Llamado por ExamenManager cuando termina el examen
    public void IniciarEspera()
    {
        esperaIniciada = true;
        StartCoroutine(WaitForDialogueProperly());
    }

    IEnumerator WaitForDialogueProperly()
    {
        // 1 — Esperar a que el dialogo realmente empiece
        yield return new WaitUntil(() => subtitleController.IsDialogueActive);

        // 2 — Esperar a que termine
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);

        if (used) yield break;
        used = true;

        // 3 — Activar animacion
        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger("Examen2puert");
        }

        // 4 — Reproducir sonido
        if (audioSource != null)
            audioSource.Play();
    }
}