using UnityEngine;

// Se coloca en el trigger de la plataforma final. Cuando el jugador
// pisa la plataforma, dispara la secuencia del salto de fe.
public class DisparadorFinal : MonoBehaviour
{
    [HideInInspector] public SecuenciaEscaleraFinal secuenciaFinal;

    private bool disparado = false;

    void OnTriggerEnter(Collider other)
    {
        if (disparado) return;
        if (!other.CompareTag("Player")) return;

        disparado = true;

        if (secuenciaFinal != null)
            secuenciaFinal.Activar();
        else
            Debug.LogError("DisparadorFinal: secuenciaFinal es NULL");
    }
}