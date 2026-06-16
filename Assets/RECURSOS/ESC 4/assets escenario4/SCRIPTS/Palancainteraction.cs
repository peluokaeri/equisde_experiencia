using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PalancaInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject instruccionCanvas;
    public Image eImage;

    [Header("Dialogo requerido")]
    public SubtitleController subtitleController;

    [Header("Palanca")]
    public Animator palancaAnimator;
    public AudioSource palancaAudio;

    [Header("Secuencia de sonidos de fabrica")]
    public AudioSource[] sonidosFabrica;
    public float duracionFadeAudio = 2f;    // Segundos que tarda cada sonido en llegar a su volumen

    [Header("Secuencia de animaciones")]
    public AnimatorEntry[] animaciones;     // Lista de Animators con su trigger

    private bool playerInside = false;
    private bool used = false;

    [System.Serializable]
    public class AnimatorEntry
    {
        public Animator animator;
        public string trigger = "Play";
        public float retardo = 0f;          // Segundos de espera antes de activar esta animacion
    }

    void Start()
    {
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        eImage.enabled = false;

        // Desactiva el animator de la palanca al inicio
        if (palancaAnimator != null)
            palancaAnimator.enabled = false;

        // Desactiva todos los animators de la secuencia al inicio
        foreach (var entry in animaciones)
        {
            if (entry.animator != null)
                entry.animator.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;
        playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        eImage.enabled = false;
    }

    void Update()
    {
        if (used) return;

        if (playerInside)
            eImage.enabled = subtitleController != null
                ? !subtitleController.IsDialogueActive
                : true;

        if (!playerInside) return;
        if (subtitleController != null && subtitleController.IsDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            used = true;
            eImage.enabled = false;
            StartCoroutine(SecuenciaFabrica());
        }
    }

    private IEnumerator SecuenciaFabrica()
    {
        // 1 — Animacion y sonido de la palanca
        if (palancaAnimator != null)
            palancaAnimator.enabled = true;

        if (palancaAudio != null)
            palancaAudio.Play();

        // Pequeña pausa tras la palanca
        yield return new WaitForSeconds(0.5f);

        // 2 — Sonidos de la fabrica con fade in
        foreach (var audio in sonidosFabrica)
        {
            if (audio != null)
                StartCoroutine(FadeInAudio(audio));

            yield return new WaitForSeconds(0.3f);
        }

        // 3 — Animaciones en orden con su retardo individual
        foreach (var entry in animaciones)
        {
            if (entry.retardo > 0f)
                yield return new WaitForSeconds(entry.retardo);

            if (entry.animator != null)
                entry.animator.enabled = true;
        }
    }

    private IEnumerator FadeInAudio(AudioSource audio)
    {
        float volumenFinal = audio.volume;
        audio.volume = 0f;
        audio.Play();

        float t = 0f;
        while (t < duracionFadeAudio)
        {
            t += Time.deltaTime;
            audio.volume = Mathf.Lerp(0f, volumenFinal, t / duracionFadeAudio);
            yield return null;
        }

        audio.volume = volumenFinal;
    }
}