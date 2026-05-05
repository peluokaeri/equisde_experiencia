using UnityEngine;
using System.Collections;

public class ControladorPantallas : MonoBehaviour
{
    [Header("Dialogo inicial")]
    public SubtitleController subtitleController;
    public DialogueData dialogueEspacio;

    [Header("Pantallas del camino")]
    public PantallaNoticia[] pantallasCamino;
    public float intervaloAparicion = 3f;

    [Header("Pantallas finales")]
    public PantallaNoticia[] pantallasFinales;
    public float intervaloFinal = 0.2f;

    [Header("Jugador")]
    public GameObject player;

    private FirstPlayer firstPlayer;
    private bool usado = false;

    void Start()
    {
        if (player != null)
        {
            firstPlayer = player.GetComponent<FirstPlayer>();
            if (firstPlayer == null)
                firstPlayer = player.GetComponentInChildren<FirstPlayer>();
        }
    }

    public void IniciarSecuencia()
    {
        if (usado) return;
        usado = true;

        if (subtitleController != null && dialogueEspacio != null)
        {
            subtitleController.PlayDialogue(dialogueEspacio);
            StartCoroutine(EsperarDialogo());
        }
        else
        {
            StartCoroutine(AparecerPantallas());
        }
    }

    IEnumerator EsperarDialogo()
    {
        // Mismo patron que Puerta2AfterDialogue que funciona
        yield return new WaitUntil(() => subtitleController.IsDialogueActive);
        yield return new WaitUntil(() => !subtitleController.IsDialogueActive);
        StartCoroutine(AparecerPantallas());
    }

    private IEnumerator AparecerPantallas()
    {
        foreach (var pantalla in pantallasCamino)
        {
            if (pantalla != null)
                pantalla.gameObject.SetActive(true);

            yield return new WaitForSeconds(intervaloAparicion);
        }

        yield return new WaitForSeconds(1f);

        foreach (var pantalla in pantallasFinales)
        {
            if (pantalla != null)
                pantalla.gameObject.SetActive(true);

            yield return new WaitForSeconds(intervaloFinal);
        }

        if (firstPlayer != null)
            firstPlayer.canMove = false;
    }
}