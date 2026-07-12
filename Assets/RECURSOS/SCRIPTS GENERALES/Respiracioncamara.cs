using UnityEngine;

public class RespiracionCamara : MonoBehaviour
{
    [Header("Configuracion")]
    public float intensidad = 0.006f;
    public float velocidad = 1.2f;

    private float posYoriginal;

    void Start()
    {
        posYoriginal = transform.localPosition.y;
    }

    void Update()
    {
        float h = Mathf.Abs(Input.GetAxis("Horizontal"));
        float v = Mathf.Abs(Input.GetAxis("Vertical"));
        float speed = h + v;

        if (speed < 0.1f)
        {
            float respY = Mathf.Sin(Time.time * velocidad) * intensidad;
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                posYoriginal + respY,
                transform.localPosition.z
            );
        }
    }
}