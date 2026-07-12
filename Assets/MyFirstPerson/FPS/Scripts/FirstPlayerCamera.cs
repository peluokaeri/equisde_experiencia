using UnityEngine;

public class FirstPlayerCamera : MonoBehaviour
{
    [SerializeField] float _yMaxRotation, _yMinRotation;

    [Header("Camera Bob")]
    public float bobIntensidad = 0.04f;
    public float bobVelocidad = 10f;
    public float bobLateral = 0.015f;

    [Header("Respiracion")]
    public float respIntensidad = 0.006f;
    public float respVelocidad = 1.2f;

    float _mouseY;
    Transform _playersHead;
    float _bobTimer = 0f;

    private void LateUpdate()
    {
        Movement();
    }

    public void SetPlayersHead(Transform playersHead)
    {
        _playersHead = playersHead;
    }

    void Movement()
    {
        // Posicion base desde la cabeza del player
        Vector3 pos = _playersHead.position;

        float h = Mathf.Abs(Input.GetAxis("Horizontal"));
        float v = Mathf.Abs(Input.GetAxis("Vertical"));
        float speed = h + v;

        if (speed > 0.1f)
        {
            // Camera bob al caminar
            _bobTimer += Time.deltaTime * bobVelocidad;
            pos.y += Mathf.Sin(_bobTimer) * bobIntensidad;
            pos.x += Mathf.Sin(_bobTimer * 0.5f) * bobLateral;
        }
        else
        {
            // Respiracion cuando esta quieto
            _bobTimer = Mathf.Lerp(_bobTimer, 0f, Time.deltaTime * 5f);
            pos.y += Mathf.Sin(Time.time * respVelocidad) * respIntensidad;
        }

        transform.position = pos;
    }

    public void Rotate(float xAxis, float yAxis)
    {
        _mouseY += yAxis;
        _mouseY = Mathf.Clamp(_mouseY, _yMinRotation, _yMaxRotation);
        transform.rotation = Quaternion.Euler(-_mouseY, xAxis, 0);
    }
}