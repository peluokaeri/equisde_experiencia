using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ContadorCartas : MonoBehaviour
{
    [Header("Cartas a leer")]
    public CartaInteraction[] cartas;           // Las 6 cartas

    [Header("Pared de cajas (primeras 3 cartas)")]
    public GameObject[] cajas;
    public Collider coliderBloqueo;

    [Header("Dialogo si intenta pasar antes")]
    public SubtitleController subtitleController;
    public DialogueData dialogueFaltanCartas;

    [Header("Dialogo final (al leer las 6)")]
    public DialogueData dialogueFinal;

    [Header("Transicion final")]
    public Image imagenBlanca;
    public float velocidadFade = 1f;
    public string nombreEscena;

    private bool paredEliminada = false;
    private bool secuenciaFinalIniciada = false;

    void Update()
    {
        if (!paredEliminada && PrimerasMitadLeidas())
            EliminarPared();

        if (!secuenciaFinalIniciada && TodasLeidas())
        {
            secuenciaFinalIniciada = true;
            StartCoroutine(SecuenciaFinal());
        }
    }

    // Verifica si las primeras 3 cartas fueron leidas
    private bool PrimerasMitadLeidas()
    {
        int mitad = Mathf.CeilToInt(cartas.Length / 2f);
        for (int i = 0; i < mitad && i < cartas.Length; i++)
        {
            if (cartas[i] == null || !cartas[i].fueLeida)
                return false;
        }
        return true;
    }

    private bool TodasLeidas()
    {
        foreach (var carta in cartas)
        {
            if (carta == null || !carta.fueLeida)
                return false;
        }
        return true;
    }

    private void EliminarPared()
    {
        paredEliminada = true;

        foreach (var caja in cajas)
            if (caja != null) caja.SetActive(false);

        if (coliderBloqueo != null)
            coliderBloqueo.enabled = false;
    }

    public void IntentarPasar()
    {
        if (paredEliminada) return;
        if (subtitleController == null || dialogueFaltanCartas == null) return;
        if (subtitleController.IsDialogueActive) return;

        subtitleController.PlayDialogue(dialogueFaltanCartas);
    }

    private IEnumerator SecuenciaFinal()
    {
        // Espera que no haya dialogo activo
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);
        yield return new WaitForSeconds(0.5f);

        // Dialogo final — patron Puerta2AfterDialogue
        if (subtitleController != null && dialogueFinal != null)
        {
            subtitleController.PlayDialogue(dialogueFinal);

            // Espera dos frames para que arranque
            yield return null;
            yield return null;

            // Espera que termine
            yield return new WaitUntil(() => !subtitleController.IsDialogueActive);
        }

        yield return new WaitForSeconds(0.3f);

        // Fade a blanco completo
        if (imagenBlanca != null)
        {
            imagenBlanca.gameObject.SetActive(true);
            Color c = imagenBlanca.color;
            c.a = 0f;
            imagenBlanca.color = c;

            while (c.a < 1f)
            {
                c.a += Time.deltaTime * velocidadFade;
                imagenBlanca.color = c;
                yield return null;
            }

            c.a = 1f;
            imagenBlanca.color = c;
        }

        // Solo cambia de escena cuando el fade esta 100% completo
        yield return new WaitForSeconds(0.3f);

        if (!string.IsNullOrEmpty(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
    }
}