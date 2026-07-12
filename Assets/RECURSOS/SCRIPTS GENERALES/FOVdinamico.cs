using UnityEngine;

public class FOVDinamico : MonoBehaviour
{
    public float fovBase = 60f;
    public float fovMovimiento = 65f;
    public float velocidadTransicion = 3f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
            cam.fieldOfView = fovBase;
    }

    void Update()
    {
        if (cam == null) return;

        float h = Mathf.Abs(Input.GetAxis("Horizontal"));
        float v = Mathf.Abs(Input.GetAxis("Vertical"));
        float speed = h + v;

        float targetFOV = speed > 0.1f ? fovMovimiento : fovBase;

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFOV,
            Time.deltaTime * velocidadTransicion
        );
    }
}