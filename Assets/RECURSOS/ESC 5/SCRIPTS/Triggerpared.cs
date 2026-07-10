using UnityEngine;

public class TriggerPared : MonoBehaviour
{
    public ContadorCartas contadorCartas;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (contadorCartas != null)
            contadorCartas.IntentarPasar();
    }
}