using UnityEngine;

public class PuntoAtraccionFinal : MonoBehaviour
{
    public AtraccionFinal atraccionFinal;
    public SecuenciaFinal secuenciaFinal;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (atraccionFinal != null)
            atraccionFinal.LlegoAlPunto();

        if (secuenciaFinal != null)
            secuenciaFinal.IniciarSecuenciaFinal();
    }
}