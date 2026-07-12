using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    [Header("Sonidos")]
    public AudioClip[] clips;           // Varios clips para variar
    public float volumen = 0.4f;
    public float intervalo = 0.45f;     // Segundos entre pasos

    private AudioSource audioSource;
    private float timer = 0f;
    private Rigidbody playerRb;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;   // 2D - suena igual en todos lados
        audioSource.volume = volumen;
        audioSource.playOnAwake = false;

        playerRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (clips == null || clips.Length == 0) return;

        float speed = 0f;

        if (playerRb != null)
        {
            Vector3 vel = playerRb.velocity;
            vel.y = 0f;
            speed = vel.magnitude;
        }
        else
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            speed = new Vector2(h, v).magnitude;
        }

        if (speed > 0.3f)
        {
            timer += Time.deltaTime;

            if (timer >= intervalo)
            {
                timer = 0f;
                ReproducirPaso();
            }
        }
        else
        {
            timer = 0f;
        }
    }

    void ReproducirPaso()
    {
        // Elige un clip al azar
        AudioClip clip = clips[Random.Range(0, clips.Length)];

        // Varia levemente el pitch para que no suene robotico
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clip, volumen);
    }
}