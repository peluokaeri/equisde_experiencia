using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Genera escalones con cubos hacia adelante y arriba a medida que el
// jugador sube. Cada cierta distancia inserta un rellano con un banco.
// Recicla los escalones viejos para no saturar la memoria.
public class EscalerasInfinitas : MonoBehaviour
{
    [Header("Jugador")]
    public Transform player;

    [Header("Dimensiones del escalon")]
    public float ancho = 3f;        // Ancho (eje X)
    public float alto = 0.25f;      // Altura de cada escalon (sube)
    public float profundidad = 0.4f;// Fondo de cada escalon (avanza en Z)

    [Header("Generacion")]
    [Tooltip("Cuantos escalones aparecen por delante tuyo. Ajustalo en vivo.")]
    public int escalonesAdelante = 4;    // Solo los 4 mas cercanos
    public int escalonesDetras = 15;     // Cuantos mantener detras antes de reciclar
    public Material materialEscalon;

    [Header("Animacion de aparicion")]
    public float alturaInicial = 2f;     // Cuanto por debajo arranca antes de subir
    public float duracionSubida = 0.4f;  // Cuanto tarda en llegar a su lugar
    public float retardoEntreEscalones = 0.08f; // Pausa entre uno y el siguiente

    [Header("Paredes invisibles (evitan caidas)")]
    public bool generarParedes = true;
    public float alturaPared = 2f;      // Que tan altas son las barreras
    public float grosorPared = 0.2f;

    [Header("Pared trasera")]
    public bool generarParedTrasera = true;
    private GameObject paredTrasera;    // La pared que sigue por detras al player

    [Header("Limite final")]
    public int escalonFinal = 300;         // En que escalon se dispara el final
    public SecuenciaEscaleraFinal secuenciaFinal; // Script que hace el destello

    [Header("Plataforma final (salto de fe)")]
    public float largoPlataformaFinal = 6f;
    public Material materialPlataformaFinal;
    private bool plataformaFinalCreada = false;

    [Header("Descanso (piso + banco)")]
    public int escalonDescanso = 150;      // A que escalon aparece el descanso
    public float largoDescanso = 6f;       // Profundidad del piso de descanso
    public float anchoDescanso = 5f;       // Ancho del piso de descanso
    public Material materialDescanso;      // Material del piso (opcional)
    public GameObject bancoPrefab;         // Prefab del banco
    [Header("Ajuste del banco")]
    public float bancoAlturaExtra = 0.5f;  // Subir el banco si queda hundido
    public float bancoOffsetDerecha = 0.8f;// Cuanto adentro del borde derecho
    private bool descansoCreado = false;
    private bool pausadoPorDescanso = false;
    private float zFinDescanso = 0f;       // Z donde termina el piso de descanso
    private float yDescanso = 0f;          // Altura del descanso

    [Header("Dialogos durante la subida")]
    public SubtitleController subtitleController;
    // Cada entrada: a que escalon dispara que dialogo. El jugador NO se frena.
    public DialogoEnEscalon[] dialogos;

    [System.Serializable]
    public class DialogoEnEscalon
    {
        public int escalon;              // A que altura/escalon se dispara
        public DialogueData dialogue;    // Que dialogo reproduce
        [HideInInspector] public bool disparado = false;
    }

    // Estado interno
    private int siguienteIndice = 0;       // Proximo escalon a crear
    private int indiceMasBajo = 0;         // Escalon mas viejo que sigue vivo
    private Dictionary<int, GameObject> escalones = new Dictionary<int, GameObject>();
    private bool finalDisparado = false;

    // Cola para animar de a uno
    private Queue<int> colaGeneracion = new Queue<int>();
    private bool generando = false;

    void Start()
    {
        // Encola los primeros escalones (se crean animados de a uno)
        for (int i = 0; i < escalonesAdelante; i++)
            colaGeneracion.Enqueue(i);

        siguienteIndice = escalonesAdelante;

        StartCoroutine(ProcesarCola());
    }

    void Update()
    {
        if (player == null) return;

        // Averigua en que escalon esta el jugador (por su altura)
        int escalonActual = Mathf.FloorToInt(player.position.y / alto);

        // Si esta pausado por el descanso, espera a que el jugador
        // cruce la zona plana antes de reanudar la generacion
        if (pausadoPorDescanso)
        {
            if (player.position.z >= zFinDescanso)
            {
                // El jugador cruzo el descanso: reanuda desde aca
                pausadoPorDescanso = false;
            }
            else
            {
                // Sigue en el descanso: no genera escalones nuevos
                ChequearDialogos(escalonActual);
                return;
            }
        }

        // Encola hacia adelante (salvo que el proximo sea el descanso)
        int objetivo = escalonActual + escalonesAdelante;
        while (siguienteIndice <= objetivo && siguienteIndice <= escalonFinal)
        {
            colaGeneracion.Enqueue(siguienteIndice);

            // Si el que acabamos de encolar es el descanso, frenamos ahi
            if (siguienteIndice == escalonDescanso)
            {
                siguienteIndice++;
                break;
            }

            siguienteIndice++;
        }

        // Arranca el procesamiento si no esta corriendo
        if (!generando && colaGeneracion.Count > 0)
            StartCoroutine(ProcesarCola());

        // Recicla los de atras
        int limiteAtras = escalonActual - escalonesDetras;
        while (indiceMasBajo < limiteAtras)
        {
            if (escalones.TryGetValue(indiceMasBajo, out GameObject go))
            {
                if (go != null) Destroy(go);
                escalones.Remove(indiceMasBajo);
            }
            indiceMasBajo++;

            // Al reciclar el primer escalon, crea la pared trasera
            if (generarParedTrasera && paredTrasera == null)
                CrearParedTrasera();
        }

        // Mantiene la pared trasera detras del escalon mas viejo vivo
        if (paredTrasera != null)
            ActualizarParedTrasera();

        // El final ahora lo dispara el TriggerFinal al pisar la plataforma
        // (ver DisparadorFinal). Ya no depende de la altura.

        ChequearDialogos(escalonActual);
    }

    void ChequearDialogos(int escalonActual)
    {
        if (dialogos == null || subtitleController == null) return;

        foreach (var d in dialogos)
        {
            if (!d.disparado && d.dialogue != null && escalonActual >= d.escalon)
            {
                d.disparado = true;
                if (!subtitleController.IsDialogueActive)
                    subtitleController.PlayDialogue(d.dialogue);
            }
        }
    }

    // Procesa la cola creando escalones de a uno con animacion
    IEnumerator ProcesarCola()
    {
        generando = true;

        while (colaGeneracion.Count > 0)
        {
            int indice = colaGeneracion.Dequeue();

            if (!escalones.ContainsKey(indice) && indice <= escalonFinal)
            {
                CrearEscalon(indice);
                yield return new WaitForSeconds(retardoEntreEscalones);
            }
        }

        generando = false;
    }

    void CrearEscalon(int indice)
    {
        if (escalones.ContainsKey(indice)) return;

        // Si es el escalon del descanso, genera el piso ancho con banco
        if (indice == escalonDescanso && !descansoCreado)
        {
            CrearDescanso(indice);
            descansoCreado = true;
            return;
        }

        // Si es el escalon final, genera la plataforma del salto de fe
        if (indice == escalonFinal && !plataformaFinalCreada)
        {
            CrearPlataformaFinal(indice);
            plataformaFinalCreada = true;
            return;
        }

        // Offset en Z: los escalones despues del descanso arrancan
        // al final del piso plano (no encimados con el descanso)
        float offsetZ = 0f;
        if (descansoCreado && indice > escalonDescanso)
            offsetZ = largoDescanso - profundidad * 0.5f;

        // Posicion final: cada escalon sube 'alto' y avanza 'profundidad'
        Vector3 posFinal = transform.position + new Vector3(
            0f,
            indice * alto,
            indice * profundidad + offsetZ
        );

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localScale = new Vector3(ancho, alto, profundidad);

        // Arranca por debajo de su lugar final
        go.transform.position = posFinal - new Vector3(0f, alturaInicial, 0f);

        // Material propio (instancia) para poder hacer fade sin afectar otros
        Renderer rend = go.GetComponent<Renderer>();
        if (materialEscalon != null)
            rend.material = new Material(materialEscalon);

        go.transform.SetParent(transform);
        go.name = "Escalon_" + indice;

        escalones[indice] = go;

        // Paredes invisibles a los costados
        if (generarParedes)
            AgregarParedes(go, ancho);

        // Anima la subida + fade
        StartCoroutine(AnimarEscalon(go, posFinal, rend));
    }

    // Crea una pared invisible que cierra por detras
    void CrearParedTrasera()
    {
        // SIN parent, para no heredar rotacion/escala del transform padre
        paredTrasera = new GameObject("ParedTrasera");
        paredTrasera.transform.rotation = Quaternion.identity;

        BoxCollider col = paredTrasera.AddComponent<BoxCollider>();
        col.size = new Vector3(ancho * 2f, alturaPared, grosorPared);

        ActualizarParedTrasera();
    }

    // Posiciona la pared detras del escalon mas viejo que sigue vivo
    void ActualizarParedTrasera()
    {
        if (paredTrasera == null) return;

        Vector3 pos;

        if (escalones.TryGetValue(indiceMasBajo, out GameObject escalonViejo) && escalonViejo != null)
        {
            pos = escalonViejo.transform.position;
        }
        else
        {
            pos = transform.position + new Vector3(
                0f,
                indiceMasBajo * alto,
                indiceMasBajo * profundidad
            );
        }

        // Un poco por detras, a la altura del jugador
        pos.z -= profundidad;
        pos.y += alturaPared * 0.5f;

        paredTrasera.transform.position = pos;
        paredTrasera.transform.rotation = Quaternion.identity; // Siempre vertical
    }

    // Crea dos muros invisibles con collider a los lados del objeto dado
    void AgregarParedes(GameObject padre, float anchoBase)
    {
        float mitad = anchoBase * 0.5f;

        for (int lado = -1; lado <= 1; lado += 2)
        {
            GameObject pared = new GameObject("ParedInvisible");
            pared.transform.SetParent(padre.transform, false);

            // Posicion local: al costado del escalon, subiendo la mitad de la altura
            pared.transform.localPosition = new Vector3(
                (mitad + grosorPared * 0.5f) * lado / padre.transform.localScale.x,
                alturaPared * 0.5f / padre.transform.localScale.y,
                0f
            );

            // Collider (sin renderer, es invisible)
            BoxCollider col = pared.AddComponent<BoxCollider>();
            // El collider se escala inverso al padre para que quede del tamano real
            col.size = new Vector3(
                grosorPared / padre.transform.localScale.x,
                alturaPared / padre.transform.localScale.y,
                1f
            );
        }
    }

    void CrearPlataformaFinal(int indice)
    {
        float yBase = indice * alto;
        // Continua despues del descanso si ya paso
        float offsetZ = (descansoCreado) ? (largoDescanso - profundidad * 0.5f) : 0f;
        float zInicio = (indice - 1) * profundidad + profundidad * 0.5f + offsetZ;
        float zCentro = zInicio + largoPlataformaFinal * 0.5f;

        Vector3 posFinal = transform.position + new Vector3(0f, yBase, zCentro);

        // Piso de la plataforma final (mismo ancho que la escalera)
        GameObject piso = GameObject.CreatePrimitive(PrimitiveType.Cube);
        piso.transform.localScale = new Vector3(ancho, alto, largoPlataformaFinal);
        piso.transform.position = posFinal - new Vector3(0f, alturaInicial, 0f);

        Renderer rend = piso.GetComponent<Renderer>();
        if (materialPlataformaFinal != null)
            rend.material = new Material(materialPlataformaFinal);
        else if (materialEscalon != null)
            rend.material = new Material(materialEscalon);

        piso.transform.SetParent(transform);
        piso.name = "PlataformaFinal_" + indice;

        escalones[indice] = piso;

        // Paredes laterales (pero SIN pared al fondo, para poder saltar)
        if (generarParedes)
            AgregarParedes(piso, ancho);

        // Collider de bloqueo en el borde (evita saltar antes del dialogo)
        GameObject bloqueo = new GameObject("BloqueoSalto");
        bloqueo.transform.SetParent(piso.transform, false);
        BoxCollider colBloqueo = bloqueo.AddComponent<BoxCollider>();
        colBloqueo.center = new Vector3(0f, alturaPared * 0.5f / alto, 0.5f);
        colBloqueo.size = new Vector3(1f, alturaPared / alto, grosorPared / largoPlataformaFinal);

        if (secuenciaFinal != null)
            secuenciaFinal.colliderBloqueoSalto = colBloqueo;

        // Trigger que dispara el dialogo final cuando el jugador pisa la plataforma
        GameObject trigger = new GameObject("TriggerFinal");
        trigger.transform.SetParent(piso.transform, false);
        BoxCollider colTrigger = trigger.AddComponent<BoxCollider>();
        colTrigger.isTrigger = true;
        colTrigger.center = new Vector3(0f, alturaPared * 0.5f / alto, 0f);
        colTrigger.size = new Vector3(1f, alturaPared / alto, 1f);

        DisparadorFinal disp = trigger.AddComponent<DisparadorFinal>();
        disp.secuenciaFinal = secuenciaFinal;

        // Anima la subida de la plataforma
        StartCoroutine(AnimarEscalon(piso, posFinal, rend));
    }

    void CrearDescanso(int indice)
    {
        // El descanso arranca JUSTO donde termina el ultimo escalon.
        // El borde delantero del escalon anterior esta en (indice-1)*profundidad + profundidad/2
        float yBase = indice * alto;
        float zInicio = (indice - 1) * profundidad + profundidad * 0.5f;

        // El piso se centra a partir de ese borde, extendiendose largoDescanso hacia adelante
        float zCentroPiso = zInicio + largoDescanso * 0.5f;

        Vector3 posFinal = transform.position + new Vector3(
            0f,
            yBase,
            zCentroPiso
        );

        // Piso del descanso (plano ancho)
        GameObject piso = GameObject.CreatePrimitive(PrimitiveType.Cube);
        piso.transform.localScale = new Vector3(anchoDescanso, alto, largoDescanso);
        piso.transform.position = posFinal - new Vector3(0f, alturaInicial, 0f);

        Renderer rend = piso.GetComponent<Renderer>();
        if (materialDescanso != null)
            rend.material = new Material(materialDescanso);
        else if (materialEscalon != null)
            rend.material = new Material(materialEscalon);

        piso.transform.SetParent(transform);
        piso.name = "Descanso_" + indice;

        escalones[indice] = piso;

        // Paredes invisibles a los costados del descanso
        if (generarParedes)
            AgregarParedes(piso, anchoDescanso);

        // Coloca el banco a la DERECHA del piso, apoyado encima
        if (bancoPrefab != null)
        {
            // Derecha en X, un poco adentro del borde
            float xBanco = anchoDescanso * 0.5f - bancoOffsetDerecha;
            // Superficie del piso + altura extra ajustable (por el pivote del prefab)
            float yBanco = yBase + alto * 0.5f + bancoAlturaExtra;
            float zBanco = zCentroPiso;

            Vector3 posBanco = transform.position + new Vector3(xBanco, yBanco, zBanco);

            GameObject banco = Instantiate(bancoPrefab,
                posBanco - new Vector3(0f, alturaInicial, 0f),
                bancoPrefab.transform.rotation);
            banco.transform.SetParent(piso.transform);

            // Collider de bloqueo al final del descanso (obliga a sentarse)
            GameObject bloqueo = new GameObject("BloqueoDescanso");
            bloqueo.transform.SetParent(piso.transform, false);
            BoxCollider colBloqueo = bloqueo.AddComponent<BoxCollider>();
            // En el borde delantero del piso, cruzando el camino
            colBloqueo.center = new Vector3(0f, alturaPared * 0.5f / alto, 0.5f);
            colBloqueo.size = new Vector3(1f, alturaPared / alto, grosorPared / largoDescanso);

            // Se lo pasamos al DescansoBanco del banco para que lo desactive al levantarse
            DescansoBanco db = banco.GetComponentInChildren<DescansoBanco>();
            if (db == null) db = banco.GetComponent<DescansoBanco>();
            if (db != null)
                db.colliderBloqueo = colBloqueo;
        }

        // Anima la subida del piso (y el banco sube con el, es su hijo)
        StartCoroutine(AnimarEscalon(piso, posFinal, rend));

        // 🛑 Pausa la generacion. Guarda donde termina el piso (en Z global)
        zFinDescanso = transform.position.z + zInicio + largoDescanso;
        yDescanso = yBase;
        pausadoPorDescanso = true;
    }

    IEnumerator AnimarEscalon(GameObject go, Vector3 posFinal, Renderer rend)
    {
        Vector3 posInicial = go.transform.position;

        // Prepara el material para fade (si el shader lo soporta)
        Material mat = rend.material;
        Color colorBase = mat.color;
        float alphaFinal = colorBase.a;

        float t = 0f;
        while (t < duracionSubida)
        {
            if (go == null) yield break; // Por si se reciclo mientras subia

            t += Time.deltaTime;
            float k = t / duracionSubida;

            // Suavizado (ease out)
            float suave = 1f - Mathf.Pow(1f - k, 3f);

            // Sube a su lugar
            go.transform.position = Vector3.Lerp(posInicial, posFinal, suave);

            // Fade in
            Color c = colorBase;
            c.a = Mathf.Lerp(0f, alphaFinal, suave);
            mat.color = c;

            yield return null;
        }

        if (go != null)
        {
            go.transform.position = posFinal;
            Color c = colorBase;
            c.a = alphaFinal;
            mat.color = c;
        }
    }
}