using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputs;

public class Player : MonoBehaviour, IPlayerActions
{
    [Header("Components")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private Transform _head;

    public Rigidbody Rigidbody => _rigidbody;
    public CapsuleCollider Collider => _collider;
    public Transform Head => _head;

    public Action<InputAction.CallbackContext> OnCrouchInput;

    private void Awake()
    {
        if (_collider == null) _collider = GetComponent<CapsuleCollider>();
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        OnCrouchInput.Invoke(context);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
    }

    public void OnJump(InputAction.CallbackContext context)
    {
    }

    public void OnLook(InputAction.CallbackContext context)
    {
    }

    public void OnMove(InputAction.CallbackContext context)
    {
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
    }


}
