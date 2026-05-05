using UnityEngine;

public class PuntoAtraccion : MonoBehaviour
{
    public TriggerNegro triggerNegro;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggerNegro != null)
            triggerNegro.IniciarTeleport();
    }
}