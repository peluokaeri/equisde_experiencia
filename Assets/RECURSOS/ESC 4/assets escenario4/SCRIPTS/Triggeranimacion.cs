using UnityEngine;

public class TriggerAnimacion : MonoBehaviour
{
    [Header("Animacion")]
    public Animator animator;

    private bool activado = false;

    void Start()
    {
        if (animator != null)
            animator.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (activado) return;
        if (!other.CompareTag("Player")) return;

        activado = true;

        if (animator != null)
            animator.enabled = true;
    }
}