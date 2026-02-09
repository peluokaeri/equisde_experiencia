using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLuzFade : MonoBehaviour
{
    public FadeToWhite fadeToWhite;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        fadeToWhite.StartFade();
    }
}
