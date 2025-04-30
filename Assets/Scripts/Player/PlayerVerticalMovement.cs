using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVerticalMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player _player;

    [Header("Standing")]
    [SerializeField] private float _standingHeight = 1.25f;
    [SerializeField] private float _standingCenter = 1.175f;
    [SerializeField] private float _modelHeight = 1.8f;

    [Header("Crouch")]
    [SerializeField] private float _crouchHeight = 0.875f;
    [SerializeField] private float _crouchCenter = 0.8225f;
    [SerializeField] private float _crouchTime = 1f;

    [Header("Grounding Control")]
    [SerializeField] private float _groundCheckLength = 1.5f;
    [SerializeField] private float _floatForce = 25f;
    [SerializeField] private LayerMask _whatIsGround;

    private float _crouchTimer;
    private float _floatHeight;
    private float _headToHeightRatio;
    private Coroutine _crouchingCoroutine;
    public bool isCrouching { get; private set; }

    private void Awake()
    {
        if (_player == null) _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        _player.OnCrouchInput += OnCrouch;
    }

    private void OnDisable()
    {
        _player.OnCrouchInput -= OnCrouch;
    }

    private void Start()
    {
        _headToHeightRatio = _player.Head.localPosition.y / _modelHeight;

        _floatHeight = _standingCenter;
    }

    void FixedUpdate()
    {
        Float();
    }

    /// <summary>
    /// While detecting the ground, moves the player up or down to mantain the goal float/walk height.
    /// </summary>
    private void Float()
    {
        Vector3 worldPosition = transform.position + _player.Collider.center;

        Ray groundCheckRay = new Ray(worldPosition, Vector3.down);

        if (Physics.Raycast(groundCheckRay, out RaycastHit hit, _groundCheckLength, _whatIsGround, QueryTriggerInteraction.Ignore))
        {
            float distanceToFloatHeight = _floatHeight - hit.distance;

            if (distanceToFloatHeight == 0) return;

            float liftAmount = (distanceToFloatHeight * _floatForce) - _player.Rigidbody.linearVelocity.y;

            Vector3 liftForce = new Vector3(0, liftAmount, 0);

            _player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);

            _player.isGrounded = true;
        }
        else
        {
            _player.isGrounded = false;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (_player.state == PlayerState.Sprint) return;

        if (context.started)
        {
            isCrouching = _player.HoldToCrouch ? true : !isCrouching;
        }
        else if (context.canceled && _player.HoldToCrouch)
        {
            isCrouching = false;
        }

        if (context.started || (context.canceled && _player.HoldToCrouch))
        {
            _crouchTimer = _crouchTimer > 0 ? _crouchTime - _crouchTimer : _crouchTime;

            if (_crouchingCoroutine != null) StopCoroutine(_crouchingCoroutine);
            _crouchingCoroutine = StartCoroutine(Crouch());
        }
    }

    private IEnumerator Crouch()
    {
        while (_crouchTimer > 0)
        {
            float initialHeight = isCrouching ? _standingHeight : _crouchHeight;
            float targetHeight = isCrouching ? _crouchHeight : _standingHeight;

            float initialCenter = isCrouching ? _standingCenter : _crouchCenter;
            float targetCenter = isCrouching ? _crouchCenter : _standingCenter;

            Vector3 center = _player.Collider.center;
            Vector3 headPosition = _player.Head.localPosition;

            float t;

            _crouchTimer -= Time.deltaTime;

            t = 1 - (_crouchTimer / _crouchTime);

            // set collider height
            _player.Collider.height = Mathf.Lerp(initialHeight, targetHeight, t);

            // set collider center
            center.y = Mathf.Lerp(initialCenter, targetCenter, t);
            _player.Collider.center = center;

            // set head position
            headPosition.y = (_player.Collider.center.y + (_player.Collider.height / 2)) * _headToHeightRatio;
            _player.Head.localPosition = headPosition;

            // set float height
            _floatHeight = center.y;

            yield return null;
        }
    }
}
