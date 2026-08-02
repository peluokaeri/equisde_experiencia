using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Inicio del escenario 7 (final): la escena arranca en blanco, hace
// fade out (se aclara), y al terminar reproduce el dialogo que invita
// a sentarse. El jugador queda quieto hasta que termina el dialogo.
public class InicioEscenario7 : MonoBehaviour
{
    [Header("Imagen blanca")]
    public Image imagenBlanca;          // Image blanca full screen

    [Header("Configuracion del fade")]
    public float esperaInicial = 0.5f;  // Cuanto se queda en blanco antes de aclarar
    public float velocidadFade = 0.8f;  // Que tan lento se desvanece (mas bajo = mas suave)

    [Header("Dialogo inicial")]
    public SubtitleController subtitleController;
    public DialogueData dialogueInicial;

    void Start()
    {
        // Arranca en blanco total
        if (imagenBlanca != null)
        {
            imagenBlanca.gameObject.SetActive(true);
            Color c = imagenBlanca.color;
            c.a = 1f;
            imagenBlanca.color = c;
        }

        StartCoroutine(Secuencia());
    }

    private IEnumerator Secuencia()
    {
        // 1 — Espera un momento en blanco
        yield return new WaitForSeconds(esperaInicial);

        // 2 — Fade out del blanco (se aclara la escena)
        if (imagenBlanca != null)
        {
            Color c = imagenBlanca.color;
            while (c.a > 0f)
            {
                c.a -= Time.deltaTime * velocidadFade;
                imagenBlanca.color = c;
                yield return null;
            }
            c.a = 0f;
            imagenBlanca.color = c;
            imagenBlanca.gameObject.SetActive(false);
        }

        // 3 — Reproduce el dialogo inicial (invita a sentarse)
        if (subtitleController != null && dialogueInicial != null)
            subtitleController.PlayDialogue(dialogueInicial);
    }
}