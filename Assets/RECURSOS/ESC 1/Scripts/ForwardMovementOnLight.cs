using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardMovementOnLight : MonoBehaviour
{
    [Header("Movement")]
    public Transform player;      // First Person
    public float speed = 0.5f;    // Velocidad lenta

    private bool moveForward = false;
    private bool pausado = false;

    void Update()
    {
        if (!moveForward || pausado || player == null) return;

        // Movimiento infinito hacia adelante (eje Z local)
        player.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Luz"))
        {
            moveForward = true; // 🔒 NO se vuelve a apagar nunca
        }
    }

    // ⏸️ Frena el avance sin perder el estado moveForward
    public void Pausar()
    {
        pausado = true;
    }

    // ▶️ Reanuda el avance
    public void Reanudar()
    {
        pausado = false;
    }
}