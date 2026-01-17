using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToboganSlideSound : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    private int playerContacts = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerContacts++;

        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerContacts--;

        if (playerContacts <= 0)
        {
            playerContacts = 0;

            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}
