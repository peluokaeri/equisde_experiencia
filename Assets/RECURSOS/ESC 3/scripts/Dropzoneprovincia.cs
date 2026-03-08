using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZoneProvincia : MonoBehaviour, IDropHandler
{
    [Header("Configuracion")]
    public string provinciaCorrecta; // Ej: "Buenos Aires" - debe coincidir con DraggableTarjeta.nombreProvincia

    // Estado
    public DraggableTarjeta tarjetaColocada = null;
    public bool EstaOcupada => tarjetaColocada != null;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableTarjeta tarjeta = eventData.pointerDrag?.GetComponent<DraggableTarjeta>();

        if (tarjeta == null) return;
        if (EstaOcupada) return; // Zona ocupada, la tarjeta se queda donde está

        AceptarTarjeta(tarjeta);
    }

    private void AceptarTarjeta(DraggableTarjeta tarjeta)
    {
        tarjetaColocada = tarjeta;
        tarjeta.ColocarEnZona(this);
        // La tarjeta se queda donde fue soltada, no se reparentea a la zona
    }

    public void LiberarZona()
    {
        tarjetaColocada = null;
    }

    // Llamado por ExamenManager al finalizar — retorna si la tarjeta colocada es correcta
    public bool MostrarFeedback()
    {
        if (tarjetaColocada == null) return false;

        return tarjetaColocada.nombreProvincia == provinciaCorrecta;
    }
}