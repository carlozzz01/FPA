using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player _player;

    [Header("Configuration")]
    [SerializeField] private LayerMask _whatIsInteractable;
    [SerializeField] private float _range;

    private Interactable _currentInteractable;

    private void OnEnable()
    {
        _player.OnInteractInput += OnInteract;
    }

    private void OnDisable()
    {
        _player.OnInteractInput -= OnInteract;
    }

    private void FixedUpdate()
    {
        CheckForInteractable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_currentInteractable != null)
            {
                Interact();
            }
        }
    }

    private void Interact()
    {
        _currentInteractable.Interact();
    }

    private void CheckForInteractable()
    {
        RaycastHit hit;

        if (Physics.Raycast(_player.Head.position, _player.Head.position, out hit, _range, _whatIsInteractable) && hit.collider.TryGetComponent(out Interactable interactable))
        {
            _currentInteractable = interactable;
        }
        else if (_currentInteractable != null)
        {
            _currentInteractable = null;
        }
    }
}
