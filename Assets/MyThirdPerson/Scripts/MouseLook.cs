using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    Camera _camera;

    [Header("Values")]
    [SerializeField]
    float _mouseSensitivity = 100;

    [Range(0f,10f), SerializeField]
    float _distance = 10f;

    [SerializeField]
    float _hitOffSet = 0.2f;

    [Header("Target")]
    [SerializeField]
    Transform _myTarget;

    float _mouseX, _mouseY;

    Vector3 _camPos;
    Vector3 _direction;

    Ray _ray;
    RaycastHit _raycastHit;
    bool _isCameraBlocked;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void FixedUpdate()
    {
        _ray = new Ray(transform.position, _direction);

        _isCameraBlocked = Physics.SphereCast(_ray, 0.1f, out _raycastHit,_distance);
    }

    void LateUpdate()
    {
        transform.position = _myTarget.position;

        #region Cam Movement

        _mouseX += Input.GetAxisRaw("Mouse X") * _mouseSensitivity * Time.deltaTime;
        _mouseY += Input.GetAxisRaw("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        if (_mouseX >= 360 || _mouseX <= -360)
        {
            _mouseX -= 360 * Mathf.Sign(_mouseX);
        }

        _mouseY = Mathf.Clamp(_mouseY, -30f, 30f);

        transform.rotation = Quaternion.Euler(-_mouseY, _mouseX, 0f);

        #endregion

        #region Spring Arm

        _direction = -transform.forward;

        if (_isCameraBlocked)
        {
            _camPos = _raycastHit.point - _direction * _hitOffSet;
        }
        else
        {
            _camPos = transform.position + _direction * _distance;
        }

        _camera.transform.position = _camPos;

        _camera.transform.LookAt(transform.position);

        #endregion
    }

    #region GIZMOS

    void OnDrawGizmos()
    {
        var position = transform.position;

        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(position, 0.1f);

        Gizmos.DrawSphere(_camPos, 0.1f);

        Gizmos.color = Color.red;

        Gizmos.DrawLine(position,_camPos);    
    }

    #endregion
}
