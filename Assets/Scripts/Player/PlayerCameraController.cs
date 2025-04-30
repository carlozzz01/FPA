using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player _player;

    [Header("Controls")]
    [SerializeField] private float _xSensitivity = 2f;
    [SerializeField] private bool _invertX = false;
    [Space(10)]
    [SerializeField] private float _ySensitivity = 2f;
    [SerializeField] private bool _invertY = true;
    [SerializeField] private float _yMaxAngle = 90;
    [SerializeField] private float _yMinAngle = -90;

    private float _rotationHorizontal;
    private float _rotationVertical;

    private void Awake()
    {
        if (_player == null) _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        _player.OnLookInput += OnLook;
    }

    private void OnDisable()
    {
        _player.OnLookInput -= OnLook;
    }

    private void Update()
    {
        Rotate();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        _rotationHorizontal = input.x * _xSensitivity * (_invertX ? -1f : 1f);

        _rotationVertical += input.y * _ySensitivity * (_invertY ? -1f : 1f);
        _rotationVertical = Mathf.Clamp(_rotationVertical, _yMinAngle, _yMaxAngle);
    }

    private void Rotate()
    {
        _player.transform.Rotate(0f, _rotationHorizontal, 0f);

        _player.Head.localEulerAngles = new Vector3(_rotationVertical, 0f, 0f);
    }
}
