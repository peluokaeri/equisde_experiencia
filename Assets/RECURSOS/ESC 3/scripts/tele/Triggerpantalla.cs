using UnityEngine;

public class TriggerPantalla : MonoBehaviour
{
    [Header("Pantalla a activar")]
    public GameObject pantalla;

    private bool activado = false;

    void OnTriggerEnter(Collider other)
    {
        if (activado) return;
        if (!other.CompareTag("Player")) return;

        activado = true;

        if (pantalla != null)
            pantalla.SetActive(true);
    }
}