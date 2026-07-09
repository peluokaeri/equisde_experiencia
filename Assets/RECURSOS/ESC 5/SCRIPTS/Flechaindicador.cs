using UnityEngine;

public class FlechaIndicador : MonoBehaviour
{
    [Header("Movimiento")]
    public float amplitud = 0.15f;      // Cuanto sube y baja
    public float velocidad = 2f;        // Velocidad del movimiento

    [Header("Desactivar al interactuar")]
    public CartaInteraction cartaInteraction; // Arrastra el script de la carta

    private Vector3 posicionBase;

    void Start()
    {
        posicionBase = transform.position;
    }

    void Update()
    {
        // Desaparece cuando la carta fue abierta
        if (cartaInteraction != null && cartaInteraction.cartaAbierta)
        {
            gameObject.SetActive(false);
            return;
        }

        // Movimiento suave arriba y abajo
        float offsetY = Mathf.Sin(Time.time * velocidad) * amplitud;
        transform.position = posicionBase + new Vector3(0, offsetY, 0);
    }
}