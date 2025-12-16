using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPlayerCamera : MonoBehaviour
{
    [SerializeField] float _yMaxRotation, _yMinRotation;

    float _mouseY;

    Transform _playersHead;
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
        transform.position = _playersHead.position;
    }

    public void Rotate(float xAxis, float yAxis)
    {
        _mouseY += yAxis;

        _mouseY = Mathf.Clamp(_mouseY, _yMinRotation, _yMaxRotation);

        transform.rotation = Quaternion.Euler(-_mouseY, xAxis, 0);
    }
}
