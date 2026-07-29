using UnityEngine;

// Mueve un grupo de objetos (plataforma, cubo, marco) en diagonal
// hacia abajo a medida que el jugador sube, para exagerar la sensacion
// de altura ganada. Arranca despues de cierta altura del jugador.
public class ParallajeDescenso : MonoBehaviour
{
    [Header("Jugador")]
    public Transform player;

    [Header("Grupo a mover")]
    // Poner aca el objeto padre que contiene plataforma + cubo + marco.
    // Si estan sueltos, agrupalos bajo un GameObject vacio y asignalo aca.
    public Transform grupoFondo;

    [Header("Cuando arranca")]
    public float alturaInicio = 5f;     // A que altura del player empieza el efecto

    [Header("Movimiento diagonal")]
    // Por cada unidad que sube el jugador, cuanto se mueve el grupo.
    public float factorBajada = 1.5f;   // Cuanto baja (Y)
    public float factorAlejamiento = 0.8f; // Cuanto se aleja/avanza (Z)
    public float factorLateral = 0f;    // Cuanto se corre a un lado (X), opcional

    private Vector3 posicionInicialGrupo;
    private float alturaBase;
    private bool iniciado = false;

    void Start()
    {
        if (grupoFondo != null)
            posicionInicialGrupo = grupoFondo.position;
    }

    void Update()
    {
        if (player == null || grupoFondo == null)
        {
            Debug.LogError("Falta asignar Player o Grupo Fondo en el inspector");
            return;
        }

        // Espera a que el jugador supere la altura de inicio
        if (!iniciado)
        {
            if (player.position.y >= alturaInicio)
            {
                iniciado = true;
                alturaBase = player.position.y;  // Desde aca se mide el efecto
                Debug.Log("Parallaje INICIADO a altura " + alturaBase);
            }
            else
            {
                return;
            }
        }

        // Cuanto subio el jugador desde que arranco el efecto
        float subida = player.position.y - alturaBase;
        if (subida < 0f) subida = 0f;

        // Mueve el grupo en diagonal hacia abajo
        grupoFondo.position = posicionInicialGrupo + new Vector3(
            -subida * factorLateral,
            -subida * factorBajada,
            -subida * factorAlejamiento
        );
    }
}