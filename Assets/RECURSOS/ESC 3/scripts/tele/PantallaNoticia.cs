using UnityEngine;
using UnityEngine.Video;

public class PantallaNoticia : MonoBehaviour
{
    private Transform playerTransform;
    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (videoPlayer != null)
            StartCoroutine(PlayDelayed());
    }

    private System.Collections.IEnumerator PlayDelayed()
    {
        yield return null;
        yield return null;
        videoPlayer.Stop();
        videoPlayer.Play();
    }

    void Update()
    {
        if (playerTransform == null) return;

        Vector3 direccion = playerTransform.position - transform.position;
        direccion.y = 0f;

        if (direccion != Vector3.zero)
        {
            float anguloY = Quaternion.LookRotation(direccion).eulerAngles.y;
            transform.rotation = Quaternion.Euler(90f, anguloY, 0f);
        }
    }
}