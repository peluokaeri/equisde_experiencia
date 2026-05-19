using UnityEngine;
using System.Collections;

public class AtraccionFinal : MonoBehaviour
{
    [Header("Atraccion")]
    public Transform puntoDestino;          // Punto donde empiezan las teles finales
    public float fuerzaAtraccion = 5f;

    [Header("Pantallas finales")]
    public GameObject[] pantallasFinales;   // Se activan de a una rapidamente
    public float intervaloFinal = 0.15f;

    [Header("Jugador")]
    public FirstPlayer firstPlayer;

    private GameObject player;
    private bool atrayendo = false;
    private bool llegó = false;

    void OnTriggerEnter(Collider other)
    {
        if (atrayendo) return;
        if (!other.CompareTag("Player")) return;

        player = other.gameObject;
        atrayendo = true;

        // Bloquea movimiento del jugador
        if (firstPlayer != null)
            firstPlayer.canMove = false;
    }

    void Update()
    {
        if (!atrayendo || player == null || llegó) return;

        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            puntoDestino.position,
            fuerzaAtraccion * Time.deltaTime
        );
    }

    // Llamado por el PuntoAtraccionFinal al llegar
    public void LlegoAlPunto()
    {
        if (llegó) return;
        llegó = true;
        atrayendo = false;

        StartCoroutine(AparecerPantallasFinales());
    }

    private IEnumerator AparecerPantallasFinales()
    {
        foreach (var pantalla in pantallasFinales)
        {
            if (pantalla != null)
                pantalla.SetActive(true);

            yield return new WaitForSeconds(intervaloFinal);
        }

        // Mantiene el movimiento bloqueado
    }
}