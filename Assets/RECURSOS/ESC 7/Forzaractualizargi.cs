using System.Collections;
using UnityEngine;

// Fuerza la actualizacion de la iluminacion global (GI) al cargar la
// escena. Soluciona el caso donde, al venir de otra escena por LoadScene,
// el ambiente/GI no se recalcula y las superficies quedan oscuras aunque
// los valores de Lighting sean correctos.
// Poner en un GameObject del escenario 7.
public class ForzarActualizarGI : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Actualizar());
    }

    private IEnumerator Actualizar()
    {
        // Espera un frame a que la escena termine de cargar
        yield return null;

        // Fuerza el recalculo del entorno (ambiente + GI)
        DynamicGI.UpdateEnvironment();

        // Por las dudas, reaplica el ambiente actual para "empujar" el update
        var modo = RenderSettings.ambientMode;
        RenderSettings.ambientMode = modo;

        yield return null;

        // Segundo update tras estabilizar
        DynamicGI.UpdateEnvironment();
    }
}