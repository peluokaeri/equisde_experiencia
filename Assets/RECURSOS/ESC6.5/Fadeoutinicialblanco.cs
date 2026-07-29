using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Al entrar el jugador al trigger, la pantalla se pone blanca y se
// desvanece suavemente hasta ver la escena (fade out del blanco).
public class FadeOutInicialBlanco : MonoBehaviour
{
    [Header("Imagen blanca")]
    public Image imagenBlanca;          // Image blanca full screen

    [Header("Configuracion")]
    public float esperaInicial = 0.3f;  // Cuanto se queda en blanco antes de aclarar
    public float velocidadFade = 1f;    // Que tan rapido se desvanece

    [Header("Jugador (opcional)")]
    public FirstPlayer firstPlayer;     // Si se asigna, lo bloquea hasta terminar

    [Header("Dialogo inicial")]
    public SubtitleController subtitleController;
    public DialogueData dialogueInicial;
    public bool esperarFinDialogo = true;  // El jugador queda quieto hasta que termine

    private bool activado = false;

    void Start()
    {
        // Arranca activa pero transparente, esperando el trigger
        if (imagenBlanca != null)
        {
            imagenBlanca.gameObject.SetActive(true);
            Color c = imagenBlanca.color;
            c.a = 0f;
            imagenBlanca.color = c;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (activado) return;
        if (!other.CompareTag("Player")) return;

        activado = true;

        if (imagenBlanca == null)
        {
            Debug.LogError("Imagen Blanca es NULL - no se asigno en el inspector");
            return;
        }

        // Se pone blanca de golpe y despues se desvanece
        imagenBlanca.gameObject.SetActive(true);
        Color col = imagenBlanca.color;
        col.a = 1f;
        imagenBlanca.color = col;

        // Bloquea el movimiento mientras se aclara
        if (firstPlayer != null)
            firstPlayer.canMove = false;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(esperaInicial);

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

        // Reproduce el dialogo inicial al terminar de aclarar
        if (subtitleController != null && dialogueInicial != null)
        {
            subtitleController.PlayDialogue(dialogueInicial);

            if (esperarFinDialogo)
            {
                // El jugador queda quieto hasta que termine el dialogo
                yield return null;
                yield return null;
                yield return new WaitUntil(() => !subtitleController.IsDialogueActive);
            }
        }

        // Libera el movimiento
        if (firstPlayer != null)
            firstPlayer.canMove = true;
    }
}