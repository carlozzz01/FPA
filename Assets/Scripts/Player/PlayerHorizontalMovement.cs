using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHorizontalMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player _player;

    [Header("Speeds")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _sprintSpeed;

    private Vector3 _moveInput;
    private float _speed;

    private void Awake()
    {
        if (_player == null) _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        _player.OnMoveInput += OnMove;
        _player.OnCrouchInput += (context) => UpdateSpeed();
        _player.OnSprintInput += (context) => UpdateSpeed();
    }

    private void OnDisable()
    {
        _player.OnMoveInput -= OnMove;
    }

    private void Start()
    {
        UpdateSpeed();
    }

    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Reads Move input from Player
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        _moveInput.Set(input.x, 0f, input.y);
    }

    /// <summary>
    /// Moves the Player if it's grounded.
    /// </summary>
    private void Move()
    {
        if (_player.isGrounded)
        {
            // Quaternion * Vector3 essentially rotates the Vector3 towards the Quaternion direction
            Vector3 moveDirection = _player.transform.rotation * _moveInput;

            Vector3 currentHorizontalVelocity = _player.Rigidbody.linearVelocity;
            currentHorizontalVelocity.y = 0f;

            Vector3 targetHorizontalVelocity = _speed * moveDirection;

            _player.Rigidbody.AddForce(targetHorizontalVelocity - currentHorizontalVelocity, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// Updates the Player's speed depending on it's current state: Walk, Crouch, Sprint
    /// </summary>
    private void UpdateSpeed()
    {
        switch (_player.state)
        {
            case PlayerState.Walk:
                _speed = _walkSpeed;
                break;
            case PlayerState.Crouch:
                _speed = _crouchSpeed;
                break;
            case PlayerState.Sprint:
                _speed = _sprintSpeed;
                break;
            default:
                _speed = 0f;
                break;
        }
    }
}
