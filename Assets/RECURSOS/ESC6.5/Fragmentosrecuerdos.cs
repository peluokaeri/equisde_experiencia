using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cuando el jugador se sienta, van apareciendo capturas de la experiencia
// dispersas frente a el, cada una recortada con forma de splash de pintura
// (via el shader SplashRecuerdo). Aparecen con rebote y quedan flotando.
public class FragmentosRecuerdos : MonoBehaviour
{
    [Header("Capturas")]
    public Texture2D[] capturas;        // Las imagenes (como Texture2D, no Sprite)

    [Header("Material splash")]
    // Material que usa el shader Custom/SplashRecuerdo, con la mascara asignada
    public Material materialSplash;

    [Header("Referencia")]
    public Transform player;            // El jugador (los fragmentos aparecen frente a el)
    public float alturaOjos = 1.6f;     // Altura de la vista desde la base del player

    [Header("Distribucion")]
    public float distanciaMin = 2.5f;
    public float distanciaMax = 5f;
    public float dispersionX = 3.5f;
    public float dispersionY = 2f;
    public float tamanoBase = 1.2f;

    [Header("Ritmo de aparicion")]
    public float retardoInicial = 1f;
    public float intervalo = 0.6f;

    [Header("Animacion splash")]
    public float duracionSplash = 0.4f;
    public float escalaRebote = 1.25f;

    [Header("Flotacion")]
    public float flotIntensidad = 0.1f;
    public float flotVelocidad = 1f;

    private List<Transform> fragmentos = new List<Transform>();
    private List<Material> materiales = new List<Material>();
    private bool activo = false;

    public void Activar()
    {
        if (activo) return;
        activo = true;
        Debug.Log("FragmentosRecuerdos ACTIVADO. Capturas: " + (capturas != null ? capturas.Length : 0) + " | Material: " + (materialSplash != null ? materialSplash.name : "NULL"));
        StartCoroutine(GenerarFragmentos());
    }

    // Desvanece los fragmentos de a poco y avisa cuando terminaron todos
    public void DesvanecerTodos(System.Action alTerminar)
    {
        StartCoroutine(DesvanecerCoroutine(alTerminar));
    }

    private IEnumerator DesvanecerCoroutine(System.Action alTerminar)
    {
        activo = false;  // Detiene la flotacion

        Debug.Log("DesvanecerTodos: desvaneciendo " + fragmentos.Count + " fragmentos");

        // Desvanece cada fragmento con un pequeno desfase
        for (int i = 0; i < fragmentos.Count; i++)
        {
            if (materiales[i] != null)
                StartCoroutine(FadeOutFragmento(fragmentos[i], materiales[i]));
            yield return new WaitForSeconds(0.15f);
        }

        // Espera a que el ultimo termine su fade
        yield return new WaitForSeconds(duracionSplash + 0.2f);

        // Limpia todo
        foreach (var f in fragmentos)
            if (f != null) Destroy(f.gameObject);
        fragmentos.Clear();
        materiales.Clear();

        Debug.Log("DesvanecerTodos: terminado");
        alTerminar?.Invoke();
    }

    private IEnumerator FadeOutFragmento(Transform frag, Material mat)
    {
        if (frag == null || mat == null) yield break;

        Vector3 escalaInicial = frag.localScale;
        Color c = mat.color;

        // Espejo de la entrada: crece un poco (rebote) y luego se va a cero
        float t = 0f;
        while (t < duracionSplash)
        {
            if (frag == null) yield break;
            t += Time.deltaTime;
            float k = t / duracionSplash;

            // Inverso del splash: primero un pequeno rebote hacia afuera, luego a 0
            float escalaK = k < 0.4f
                ? Mathf.Lerp(1f, escalaRebote, k / 0.4f)
                : Mathf.Lerp(escalaRebote, 0f, (k - 0.4f) / 0.6f);

            frag.localScale = escalaInicial * escalaK;

            // Fade out
            c.a = Mathf.Clamp01(1f - k);
            mat.color = c;

            yield return null;
        }

        frag.localScale = Vector3.zero;
        c.a = 0f;
        mat.color = c;
    }

    // Limpieza inmediata (por si se necesita cortar de golpe)
    public void Limpiar()
    {
        activo = false;
        StopAllCoroutines();
        foreach (var f in fragmentos)
            if (f != null) Destroy(f.gameObject);
        fragmentos.Clear();
        materiales.Clear();
    }

    private IEnumerator GenerarFragmentos()
    {
        yield return new WaitForSeconds(retardoInicial);

        if (player == null)
        {
            Debug.LogError("Player no asignado en el inspector.");
            yield break;
        }

        for (int i = 0; i < capturas.Length; i++)
        {
            CrearFragmento(capturas[i]);
            yield return new WaitForSeconds(intervalo);
        }
    }

    private int indiceActual = 0;   // Para distribuir sin solapar

    private void CrearFragmento(Texture2D captura)
    {
        if (captura == null)
        {
            Debug.LogError("Captura NULL en el array");
            return;
        }
        if (materialSplash == null)
        {
            Debug.LogError("Material Splash NULL - asignalo en el inspector");
            return;
        }
        if (player == null)
        {
            Debug.LogError("Player NULL - asignalo en el inspector");
            return;
        }

        // Quad que muestra la captura recortada por el splash
        GameObject frag = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(frag.GetComponent<Collider>());

        MeshRenderer mr = frag.GetComponent<MeshRenderer>();
        Material mat = new Material(materialSplash);
        mat.SetTexture("_MainTex", captura);
        mr.material = mat;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;

        // Direccion "hacia adelante" del player, aplanada
        Vector3 adelante = player.forward;
        adelante.y = 0f;
        adelante.Normalize();

        Vector3 derecha = Vector3.Cross(Vector3.up, adelante);

        Vector3 origenOjos = player.position + Vector3.up * alturaOjos;

        // Distribucion en anillo: reparte los fragmentos en angulos
        // alrededor del centro de la vista, con leve variacion aleatoria.
        int total = Mathf.Max(1, capturas.Length);
        float angulo = (indiceActual / (float)total) * Mathf.PI * 2f
                       + Random.Range(-0.25f, 0.25f);
        indiceActual++;

        // Radio alterna entre cercano y lejano para dar profundidad
        float radioLateral = dispersionX * (0.5f + (indiceActual % 2) * 0.5f)
                             * (0.7f + Random.value * 0.3f);
        float radioVertical = dispersionY * (0.7f + Random.value * 0.3f);

        float distancia = Random.Range(distanciaMin, distanciaMax);
        Vector3 baseFrente = origenOjos + adelante * distancia;

        Vector3 offset = derecha * Mathf.Cos(angulo) * radioLateral
                       + Vector3.up * Mathf.Sin(angulo) * radioVertical;
        frag.transform.position = baseFrente + offset;

        // Mira hacia el player
        frag.transform.rotation = Quaternion.LookRotation(frag.transform.position - origenOjos);

        // Leve rotacion aleatoria
        frag.transform.Rotate(0f, 0f, Random.Range(-8f, 8f));

        float aspecto = (float)captura.width / captura.height;

        fragmentos.Add(frag.transform);
        materiales.Add(mat);

        StartCoroutine(AnimarSplash(frag.transform, mat, aspecto));
    }

    private IEnumerator AnimarSplash(Transform frag, Material mat, float aspecto)
    {
        Vector3 escalaFinal = new Vector3(tamanoBase * aspecto, tamanoBase, 1f);

        frag.localScale = Vector3.zero;

        Color c = mat.color;
        c.a = 0f;
        mat.color = c;

        float t = 0f;
        while (t < duracionSplash)
        {
            t += Time.deltaTime;
            float k = t / duracionSplash;

            float escalaK = k < 0.6f
                ? Mathf.Lerp(0f, escalaRebote, k / 0.6f)
                : Mathf.Lerp(escalaRebote, 1f, (k - 0.6f) / 0.4f);

            frag.localScale = escalaFinal * escalaK;

            c.a = Mathf.Clamp01(k * 2f);
            mat.color = c;

            yield return null;
        }

        frag.localScale = escalaFinal;
        c.a = 1f;
        mat.color = c;

        StartCoroutine(Flotar(frag));
    }

    private IEnumerator Flotar(Transform frag)
    {
        Vector3 posBase = frag.position;
        float offsetFase = Random.Range(0f, Mathf.PI * 2f);

        while (frag != null && activo)
        {
            float y = Mathf.Sin(Time.time * flotVelocidad + offsetFase) * flotIntensidad;
            frag.position = posBase + Vector3.up * y;
            yield return null;
        }
    }
}