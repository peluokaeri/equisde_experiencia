using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExamenMatManager : MonoBehaviour
{
    [Header("Huecos de la tabla")]
    public DropZoneHueco[] huecos; // 10 huecos en orden multiplicador 1 al 10

    [Header("Boton entregar")]
    public Button botonEntregar;   // Aparece al completar los 10

    [Header("Sonido de colocacion")]
    public AudioSource audioSource;
    public AudioClip sonidoColocacion;

    [Header("Dialogo del machete")]
    public SubtitleController subtitleController;
    public DialogueData dialogueMachete;

    [Header("Apagon")]
    public Light[] luces;
    public AudioSource audioApagon;
    public AudioSource audioLugubre;

    private bool dialogoMacheteActivado = false;
    private bool esperandoFinDialogoMachete = false;

    void Start()
    {
        if (botonEntregar != null)
            botonEntregar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!esperandoFinDialogoMachete) return;
        if (subtitleController == null) return;

        if (!subtitleController.IsDialogueActive)
        {
            esperandoFinDialogoMachete = false;
            StartCoroutine(Apagon());
        }
    }

    // Llamado por DropZoneHueco al recibir un numero
    public void NumeroColocado(DropZoneHueco hueco)
    {
        // Sonido de colocacion
        if (audioSource != null && sonidoColocacion != null)
            audioSource.PlayOneShot(sonidoColocacion);

        // Verifica si los primeros 5 huecos estan completos
        if (!dialogoMacheteActivado && PrimerosCincoCompletos())
        {
            dialogoMacheteActivado = true;
            StartCoroutine(IniciarDialogoMachete());
        }

        // Verifica si los 10 huecos estan completos → muestra boton
        if (TodosCompletos())
        {
            if (botonEntregar != null)
                botonEntregar.gameObject.SetActive(true);
        }
    }

    private bool PrimerosCincoCompletos()
    {
        // Huecos con multiplicador 1 al 5 (indices 0 al 4)
        for (int i = 0; i < 5 && i < huecos.Length; i++)
        {
            if (!huecos[i].EstaOcupado) return false;
        }
        return true;
    }

    private bool TodosCompletos()
    {
        foreach (var hueco in huecos)
        {
            if (!hueco.EstaOcupado) return false;
        }
        return true;
    }

    private IEnumerator IniciarDialogoMachete()
    {
        yield return null; // Espera un frame para que el drop termine

        if (subtitleController != null && dialogueMachete != null)
        {
            subtitleController.PlayDialogue(dialogueMachete);
            yield return new WaitUntil(() => subtitleController.IsDialogueActive);
            esperandoFinDialogoMachete = true;
        }
    }

    private IEnumerator Apagon()
    {
        foreach (var luz in luces)
        {
            if (luz != null)
                luz.enabled = false;
        }

        if (audioApagon != null)
            audioApagon.Play();

        yield return new WaitForSeconds(0.5f);

        if (audioLugubre != null)
        {
            audioLugubre.loop = true;
            audioLugubre.Play();
        }
    }

    public void IntentarEntregar()
    {
        Debug.Log("Examen de matematica entregado");
    }
}