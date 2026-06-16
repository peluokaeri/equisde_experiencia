using UnityEngine;

public class Escalera : MonoBehaviour
{
    [Header("Configuracion")]
    public float velocidadSubida = 4f;

    private bool playerDentro = false;
    private Rigidbody playerRb;
    private FirstPlayer firstPlayer;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerRb = other.GetComponent<Rigidbody>();
        firstPlayer = other.GetComponent<FirstPlayer>();
        playerDentro = true;

        if (playerRb != null)
            playerRb.useGravity = false;

        if (firstPlayer != null)
            firstPlayer.canMove = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerDentro = false;

        if (playerRb != null)
        {
            playerRb.useGravity = true;
            playerRb.velocity = Vector3.zero;
        }

        if (firstPlayer != null)
            firstPlayer.canMove = true;
    }

    void Update()
    {
        if (!playerDentro || playerRb == null) return;

        float input = Input.GetAxis("Vertical");
        playerRb.velocity = new Vector3(0f, input * velocidadSubida, 0f);
    }
}