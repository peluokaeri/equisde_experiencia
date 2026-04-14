using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZoneHueco : MonoBehaviour, IDropHandler
{
    [Header("Configuracion")]
    public int respuestaCorrecta; // Ej: 7, 14, 21, 28...
    public int multiplicador;     // 1 al 10

    [HideInInspector] public DraggableNumero numeroColocado = null;
    public bool EstaOcupado => numeroColocado != null;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableNumero numero = eventData.pointerDrag?.GetComponent<DraggableNumero>();

        if (numero == null) return;
        if (EstaOcupado) return;

        AceptarNumero(numero);
    }

    private void AceptarNumero(DraggableNumero numero)
    {
        numeroColocado = numero;
        numero.ColocarEnHueco(this);

        // Notifica al manager que se colocó un número
        ExamenMatManager manager = FindObjectOfType<ExamenMatManager>();
        if (manager != null)
            manager.NumeroColocado(this);
    }

    public void LiberarZona()
    {
        numeroColocado = null;
    }

    public bool EsCorrecto()
    {
        if (numeroColocado == null) return false;
        return numeroColocado.valor == respuestaCorrecta;
    }
}