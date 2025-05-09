using System;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _head;

    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _crouchSpeed = 2.5f;
    private float _moveHorizontal;
    private float _moveVertical;
    private Vector3 _movement = new Vector3();

    [Header("Rotation")]
    [SerializeField] private float _sensitivity = 2f;
    [SerializeField] private bool _invertYAxis = false;
    [SerializeField] private float _yMaxAngle;
    [SerializeField] private float _yMinAngle;
    private float _rotationHorizontal;
    private float _rotationVertical;

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 4f;
    [SerializeField] private float _maxVerticalVelocity = 50f;
    private float _verticalVelocity;

    [Header("Crouch")]
    [SerializeField] private float _standingHeight;
    [SerializeField] private float _crouchHeight;
    [SerializeField] private float _crouchTime;
    private float _crouchTimer;
    private bool _isCrouched = false;

    private void Start()
    {

    }

    void Update()
    {
        Controls();
        ApplyGravity();
        Rotation();
        Movement();
        Crouch();
    }

    private void Controls()
    {
        _moveHorizontal = Input.GetAxisRaw("Horizontal");
        _moveVertical = Input.GetAxisRaw("Vertical");

        _rotationHorizontal = Input.GetAxisRaw("Mouse X") * _sensitivity;
        _rotationVertical += Input.GetAxisRaw("Mouse Y") * _sensitivity * (_invertYAxis ? -1 : 1);

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonDown("Crouch") && (_isCrouched && CanStandUp() || !_isCrouched))
        {
            _isCrouched = !_isCrouched;
            
            _crouchTimer = (_crouchTimer > 0) ? _crouchTime - _crouchTimer : _crouchTime;
        }
    }

    private void Movement()
    {
        _movement.Set(_moveHorizontal, 0f, _moveVertical);

        // Quaternion * Vector3 esentially rotates the Vector3 towards the Quaternion direction
        _movement = transform.rotation * _movement;

        float speed = _isCrouched ? _crouchSpeed : _walkSpeed;

        _characterController.Move(_movement * speed * Time.deltaTime);
    }

    private void Rotation()
    {
        transform.Rotate(0f, _rotationHorizontal, 0f);

        _rotationVertical = Mathf.Clamp(_rotationVertical, _yMinAngle, _yMaxAngle);

        _head.localEulerAngles = new Vector3(_rotationVertical, 0f, 0f);
    }

    private void ApplyGravity()
    {
        _characterController.Move(Vector3.up * _verticalVelocity * Time.deltaTime);

        if (_characterController.isGrounded)
        {
            _verticalVelocity = 0;
        }
        else
        {
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
            _verticalVelocity = Mathf.Clamp(_verticalVelocity, -_maxVerticalVelocity, _maxVerticalVelocity);
        }
    }

    private void Jump()
    {
        if (_characterController.isGrounded)
        {
            _verticalVelocity = _jumpForce;
        }
    }

    private void Crouch()
    {
        if (_crouchTimer <= 0f)
        {
            
            return;
        }

        _crouchTimer -= Time.deltaTime;

        float initialHeight = _isCrouched ? _standingHeight : _crouchHeight;
        float targetHeight = _isCrouched ? _crouchHeight : _standingHeight;

        float initialCenter = initialHeight / 2f;
        float targetCenter = targetHeight / 2f;

        float t = 1 - (_crouchTimer / _crouchTime);

        _characterController.height = Mathf.Lerp(initialHeight, targetHeight, t);

        Vector3 center = _characterController.center;

        center.y = Mathf.Lerp(initialCenter, targetCenter, t);

        _characterController.center = center;

        Vector3 headPosition = _head.localPosition;
        headPosition.y = _characterController.height * .85f;
        _head.localPosition = headPosition;
    }

    private bool CanStandUp()
    {
        Vector3 initialPoint = transform.position + Vector3.up * _crouchHeight;
        Vector3 endPoint = initialPoint + Vector3.up * (_standingHeight - _crouchHeight);

        bool canStandUp = !Physics.Linecast(initialPoint, endPoint);

        return canStandUp;
    }
}