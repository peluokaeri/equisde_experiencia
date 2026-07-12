using UnityEngine;

public class CameraBob : MonoBehaviour
{
    [Header("Configuracion")]
    public float intensidad = 0.04f;
    public float velocidad = 10f;
    public float intensidadLateral = 0.02f;

    private float timer = 0f;
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

        if (speed > 0.1f)
        {
            timer += Time.deltaTime * velocidad;
            float bobY = Mathf.Sin(timer) * intensidad;
            float posY = transform.localPosition.y;
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                posYoriginal + bobY,
                transform.localPosition.z
            );
        }
        else
        {
            timer = 0f;
        }
    }
}