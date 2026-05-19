using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExamenMatManager : MonoBehaviour
{
    [Header("Huecos de la tabla")]
    public DropZoneHueco[] huecos;

    [Header("Sonido de colocacion")]
    public AudioSource audioSource;
    public AudioClip sonidoColocacion;

    [Header("Apagon")]
    public Light[] luces;
    public AudioSource audioApagon;
    public AudioSource audioLugubre;   // Desactivado al inicio, solo suena con el apagon

    [Header("Dialogo post apagon")]
    public SubtitleController subtitleController;
    public DialogueData dialoguePostApagon;
    public Camera mainCamera;
    public Camera camera2;
    public GameObject examenMatCanvas;

    [Header("Jugador")]
    public GameObject player;

    [Header("Apagon extras")]
    public ParticleSystem sistemaParticulas;    // Se activa con el apagon
    public GameObject triggerNegro;             // Se habilita con el apagon
    public AudioSource audioApagarPostApagon;   // Sonido que se apaga tras el apagon
    public Animator puertaAnimator;             // Animator de la puerta
    public AudioSource puertaGolpeAudio;        // Sonido del golpe de la puerta

    private bool apagonActivado = false;
    private FirstPlayer firstPlayer;

    void Start()
    {
        // Asegura que el sonido lugubre este desactivado al inicio
        if (audioLugubre != null)
        {
            audioLugubre.Stop();
            audioLugubre.loop = true;
        }

        if (player != null)
        {
            firstPlayer = player.GetComponent<FirstPlayer>();
            if (firstPlayer == null)
                firstPlayer = player.GetComponentInChildren<FirstPlayer>();
        }
    }

    public void NumeroColocado(DropZoneHueco hueco)
    {
        // Sonido de colocacion
        if (audioSource != null && sonidoColocacion != null)
            audioSource.PlayOneShot(sonidoColocacion);

        // Al colocar 6 tarjetas → apagon
        if (!apagonActivado && SeisCompletos())
        {
            apagonActivado = true;
            StartCoroutine(Apagon());
        }
    }

    private bool SeisCompletos()
    {
        int completos = 0;
        foreach (var hueco in huecos)
        {
            if (hueco.EstaOcupado) completos++;
        }
        return completos >= 6;
    }

    private IEnumerator Apagon()
    {
        yield return new WaitForSeconds(0.3f);

        // Apaga todas las luces y cambia camara inmediatamente
        foreach (var luz in luces)
        {
            if (luz != null)
                luz.enabled = false;
        }

        // Cierra el examen y restaura camara al instante
        if (examenMatCanvas != null)
            examenMatCanvas.SetActive(false);

        if (camera2 != null)
            camera2.gameObject.SetActive(false);

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);

        // Reactiva jugador
        if (player != null)
        {
            MeshRenderer[] renderers = player.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var r in renderers) r.enabled = true;
        }

        if (firstPlayer != null)
            firstPlayer.canMove = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Activa sistema de particulas
        if (sistemaParticulas != null)
            sistemaParticulas.Play();

        // Habilita el TriggerNegro
        if (triggerNegro != null)
            triggerNegro.SetActive(true);

        // Sonido apagon
        if (audioApagon != null)
            audioApagon.Play();

        // Apaga el sonido indicado
        if (audioApagarPostApagon != null)
            audioApagarPostApagon.Stop();

        // Arranca sonido lugubre
        if (audioLugubre != null)
            audioLugubre.Play();

        // Animacion de la puerta
        if (puertaAnimator != null)
        {
            puertaAnimator.enabled = true;
            puertaAnimator.SetTrigger("Play");
        }

        // Sonido del golpe de la puerta
        if (puertaGolpeAudio != null)
            puertaGolpeAudio.Play();

        // Inicia dialogo post apagon
        if (subtitleController != null && dialoguePostApagon != null)
            subtitleController.PlayDialogue(dialoguePostApagon);
    }
}