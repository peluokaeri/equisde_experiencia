using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeToWhite : MonoBehaviour
{
    public Image whiteImage;
    public float fadeDuration = 3f;

    private bool fading = false;

    public void StartFade()
    {
        if (fading) return;
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        fading = true;

        Color color = whiteImage.color;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            color.a = alpha;
            whiteImage.color = color;
            yield return null;
        }

        color.a = 1f;
        whiteImage.color = color;
    }
}
