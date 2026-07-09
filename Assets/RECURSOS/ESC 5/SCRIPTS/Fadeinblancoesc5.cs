using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInBlancoEsc5 : MonoBehaviour
{
    [Header("Imagen blanca")]
    public Image imagenBlanca;

    [Header("Configuracion")]
    public float velocidadFade = 1.5f;
    public float esperaInicial = 0.2f;

    [Header("Dialogo inicial")]
    public SubtitleController subtitleController;
    public DialogueData dialogueInicio;

    void Start()
    {
        if (imagenBlanca != null)
        {
            Color c = imagenBlanca.color;
            c.a = 1f;
            imagenBlanca.color = c;
            imagenBlanca.gameObject.SetActive(true);
        }

        StartCoroutine(FadeATransparente());
    }

    private IEnumerator FadeATransparente()
    {
        yield return new WaitForSeconds(esperaInicial);

        Color color = imagenBlanca.color;

        while (color.a > 0f)
        {
            color.a -= Time.deltaTime * velocidadFade;
            imagenBlanca.color = color;
            yield return null;
        }

        color.a = 0f;
        imagenBlanca.color = color;

        if (subtitleController != null && dialogueInicio != null)
            subtitleController.PlayDialogue(dialogueInicio);
    }
}