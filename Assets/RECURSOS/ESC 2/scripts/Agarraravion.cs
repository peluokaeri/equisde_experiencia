using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DefaultExecutionOrder(1000)]
public class AgarrarAvion : MonoBehaviour
{
    [Header("Objeto a agarrar")]
    public Transform objetoAgarrar;         // La barbie (si es distinto de este objeto)

    [Header("UI")]
    public GameObject instruccionCanvas;
    public Image eImage;

    [Header("Dialogo")]
    public SubtitleController subtitleController;
    public DialogueData dialogueObjeto;

    [Header("Camara")]
    public Camera mainCamera;
    public float distanciaCamara = 0.6f;
    public Vector3 offsetPosicion = new Vector3(0.44f, -1.53f, 0.21f);
    public Vector3 rotacionEnMano = new Vector3(0f, 214.8f, 0f);
    public float velocidadMovimiento = 3f;

    [Header("Tiempo en mano")]
    public float tiempoEnMano = 1.5f;       // Segundos antes de lanzar

    [Header("Lanzamiento del avion")]
    public float fuerzaLanzamiento = 8f;
    public float fuerzaAscenso = 2f;
    public float duracionVuelo = 3f;
    public Vector3 offsetRotacionVuelo = new Vector3(0f, 180f, 0f); // Ajusta hacia donde mira

    [Header("Movimiento en mano")]
    public bool flotarSuave = true;
    public float flotarIntensidad = 0.02f;
    public float flotarVelocidad = 2f;
    public bool rotarLento = true;
    public float rotarVelocidad = 15f;

    [Header("Movimiento de juego (como jugando)")]
    public bool movimientoJuego = false;
    public float juegoVertical = 0.08f;      // Amplitud arriba/abajo
    public float juegoHorizontal = 0.1f;     // Amplitud izquierda/derecha
    public float juegoVelocidadV = 3f;       // Velocidad vertical
    public float juegoVelocidadH = 1.5f;     // Velocidad horizontal
    public float juegoBalanceo = 12f;        // Inclinacion al moverse

    [Header("Sonido")]
    public AudioSource audioSource;

    private bool playerInside = false;
    private bool agarrado = false;
    private bool enMano = false;

    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private Transform padreOriginal;

    void Start()
    {
        if (instruccionCanvas != null && !instruccionCanvas.activeSelf)
            instruccionCanvas.SetActive(true);

        if (eImage != null)
            eImage.enabled = false;

        if (mainCamera == null)
            mainCamera = Camera.main;

        // Si no se asigna, usa este mismo objeto
        if (objetoAgarrar == null)
            objetoAgarrar = transform;

        // Guarda estado original
        posicionOriginal = objetoAgarrar.position;
        rotacionOriginal = objetoAgarrar.rotation;
        padreOriginal = objetoAgarrar.parent;
    }

    void OnTriggerEnter(Collider other)
    {
        if (agarrado) return;
        if (!other.CompareTag("Player")) return;
        playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        if (eImage != null && !agarrado) eImage.enabled = false;
    }

    void Update()
    {
        if (agarrado) return;

        bool dialogoActivo = subtitleController != null && subtitleController.IsDialogueActive;

        if (playerInside && eImage != null)
            eImage.enabled = !dialogoActivo;

        if (!playerInside || dialogoActivo) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Agarrar();
        }
    }

    void LateUpdate()
    {
        if (agarrado && enMano)
            MovimientoEnMano();
    }

    private void Agarrar()
    {
        agarrado = true;

        if (eImage != null)
            eImage.enabled = false;

        if (audioSource != null)
            audioSource.Play();

        StartCoroutine(MoverHaciaCamara());

        if (subtitleController != null && dialogueObjeto != null)
            subtitleController.PlayDialogue(dialogueObjeto);
    }

    void OnValidate()
    {
        // Se llama automaticamente cuando tocas el inspector
        if (Application.isPlaying && enMano)
            MovimientoEnMano();
    }

    private IEnumerator MoverHaciaCamara()
    {
        // Desactiva el Animator que pelea por la posicion
        Animator anim = objetoAgarrar.GetComponent<Animator>();
        if (anim == null)
            anim = objetoAgarrar.GetComponentInChildren<Animator>();
        if (anim != null)
            anim.enabled = false;

        // Desactiva fisica y colisiones mientras esta en la mano
        Rigidbody rb = objetoAgarrar.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        // Desactiva todos los colliders del objeto
        Collider[] colliders = objetoAgarrar.GetComponentsInChildren<Collider>();
        foreach (var c in colliders)
            c.enabled = false;

        float t = 0f;
        Vector3 inicioPos = objetoAgarrar.position;
        Quaternion inicioRot = objetoAgarrar.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * velocidadMovimiento;

            // Posicion objetivo frente a la camara
            Vector3 destino = mainCamera.transform.position
                + mainCamera.transform.forward * (distanciaCamara + offsetPosicion.z)
                + mainCamera.transform.right * offsetPosicion.x
                + mainCamera.transform.up * offsetPosicion.y;

            objetoAgarrar.position = Vector3.Lerp(inicioPos, destino, t);

            Quaternion destinoRot = mainCamera.transform.rotation * Quaternion.Euler(rotacionEnMano);
            objetoAgarrar.rotation = Quaternion.Slerp(inicioRot, destinoRot, t);

            yield return null;
        }

        enMano = true;

        // Fuerza el refresco de posicion varios frames seguidos
        for (int i = 0; i < 5; i++)
        {
            MovimientoEnMano();
            yield return null;
        }

        // Espera 1.5 segundos y lanza el avion
        yield return new WaitForSeconds(tiempoEnMano);
        Lanzar();
    }

    private void Lanzar()
    {
        enMano = false;
        StartCoroutine(VueloAvion());
    }

    private IEnumerator VueloAvion()
    {
        // Direccion hacia donde mira la camara
        Vector3 direccion = (mainCamera.transform.forward + Vector3.up * 0.2f).normalized;

        // Orientacion fija hacia la direccion de vuelo
        Quaternion rotVuelo = Quaternion.LookRotation(direccion) * Quaternion.Euler(offsetRotacionVuelo);
        objetoAgarrar.rotation = rotVuelo;

        float t = 0f;
        while (t < duracionVuelo)
        {
            t += Time.deltaTime;

            // Movimiento recto hacia adelante con ascenso leve
            Vector3 movimiento = direccion * fuerzaLanzamiento * Time.deltaTime;
            movimiento += Vector3.up * fuerzaAscenso * Time.deltaTime * (1f - t / duracionVuelo);
            objetoAgarrar.position += movimiento;

            // Mantiene la orientacion recta
            objetoAgarrar.rotation = rotVuelo;

            yield return null;
        }

        objetoAgarrar.gameObject.SetActive(false);

        // Asegura que la E quede apagada
        if (eImage != null)
            eImage.enabled = false;
    }

    private void MovimientoEnMano()
    {
        // Sigue a la camara
        Vector3 destino = mainCamera.transform.position
            + mainCamera.transform.forward * (distanciaCamara + offsetPosicion.z)
            + mainCamera.transform.right * offsetPosicion.x
            + mainCamera.transform.up * offsetPosicion.y;

        // Movimiento de juego — oscila centrado en la posicion base
        if (movimientoJuego)
        {
            float vert  = Mathf.Sin(Time.time * juegoVelocidadV) * juegoVertical;
            float horiz = Mathf.Sin(Time.time * juegoVelocidadH) * juegoHorizontal;
            destino += mainCamera.transform.up * vert;
            destino += mainCamera.transform.right * horiz;
        }

        // Flotar suave
        if (flotarSuave)
            destino += mainCamera.transform.up * Mathf.Sin(Time.time * flotarVelocidad) * flotarIntensidad;

        objetoAgarrar.position = destino;

        // Rotacion base siguiendo la camara
        Quaternion baseRot = mainCamera.transform.rotation * Quaternion.Euler(rotacionEnMano);

        // Balanceo como si jugara con ella
        if (movimientoJuego)
        {
            float balanceoZ = Mathf.Sin(Time.time * juegoVelocidadH) * juegoBalanceo;
            float balanceoX = Mathf.Sin(Time.time * juegoVelocidadV) * (juegoBalanceo * 0.5f);
            baseRot *= Quaternion.Euler(balanceoX, 0f, balanceoZ);
        }

        // Rotar lento sobre si mismo
        if (rotarLento)
            baseRot *= Quaternion.Euler(0f, Time.time * rotarVelocidad % 360f, 0f);

        objetoAgarrar.rotation = baseRot;
    }

    // Llamar para soltar el objeto (por ejemplo al terminar el dialogo)
    public void Soltar()
    {
        enMano = false;
        StartCoroutine(VolverAOrigen());
    }

    private IEnumerator VolverAOrigen()
    {
        float t = 0f;
        Vector3 inicioPos = objetoAgarrar.position;
        Quaternion inicioRot = objetoAgarrar.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * velocidadMovimiento;
            objetoAgarrar.position = Vector3.Lerp(inicioPos, posicionOriginal, t);
            objetoAgarrar.rotation = Quaternion.Slerp(inicioRot, rotacionOriginal, t);
            yield return null;
        }

        objetoAgarrar.position = posicionOriginal;
        objetoAgarrar.rotation = rotacionOriginal;

        // Reactiva colliders
        Collider[] colliders = objetoAgarrar.GetComponentsInChildren<Collider>();
        foreach (var c in colliders)
            c.enabled = true;

        Rigidbody rb = objetoAgarrar.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;
    }
}