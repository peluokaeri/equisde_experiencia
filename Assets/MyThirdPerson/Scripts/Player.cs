using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    Rigidbody _rgbd;
    Animator _animator;

    Vector3 _direction;

    [SerializeField] float _speed;

    Transform _cameraTransform;

    Vector3 _cameraForward;
    Vector3 _cameraRight;

    Vector3 _newDirection;

    void Awake()
    {
        _rgbd = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        _direction.x = Input.GetAxis("Horizontal");
        _direction.z = Input.GetAxis("Vertical");

        _animator.SetFloat("zAxi", _direction.z);
    }

    void FixedUpdate()
    {
        if (_direction.x != 0 || _direction.z != 0)
        {
            Movement();
        }
    }

    void Movement()
    {
        _cameraForward = _cameraTransform.forward;
        _cameraRight = _cameraTransform.right;
        
        _cameraForward.y = 0;
        _cameraRight.y = 0;

        Rotate(_cameraForward.normalized);

        if (_direction.sqrMagnitude > 1)
        {
            _direction.Normalize();
        }

        _newDirection = (_cameraForward * _direction.z + _cameraRight * _direction.x);

        //                 Posicion actual +   Direccion   *  velocidad  * tiempo
        _rgbd.MovePosition((_rgbd.position + _newDirection * (_speed * Time.fixedDeltaTime)));
    }

    void Rotate(Vector3 direction)
    {
        transform.forward = direction;
    }
}
