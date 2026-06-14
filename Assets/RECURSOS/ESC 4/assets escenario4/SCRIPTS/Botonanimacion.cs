using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class BotonAnimacion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform rt;
    private Vector3 escalaOriginal;
    private Coroutine animActual;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        escalaOriginal = rt.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimarA(new Vector3(1.03f, 1.03f, 1f), 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimarA(escalaOriginal, 0.12f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(AnimacionClick());
    }

    private IEnumerator AnimacionClick()
    {
        // Achica rapido
        yield return StartCoroutine(LerpEscala(new Vector3(0.96f, 0.96f, 1f), 0.07f));
        // Vuelve a normal
        yield return StartCoroutine(LerpEscala(escalaOriginal, 0.1f));
    }

    private void AnimarA(Vector3 destino, float duracion)
    {
        if (animActual != null)
            StopCoroutine(animActual);
        animActual = StartCoroutine(LerpEscala(destino, duracion));
    }

    private IEnumerator LerpEscala(Vector3 destino, float duracion)
    {
        Vector3 origen = rt.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            rt.localScale = Vector3.Lerp(origen, destino, t);
            yield return null;
        }
        rt.localScale = destino;
    }
}