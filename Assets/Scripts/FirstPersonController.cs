using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _head;

    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 5f;
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

    void Update()
    {
        Controls();
        Rotation();
        Movement();
    }

    private void Controls()
    {
        _moveHorizontal = Input.GetAxisRaw("Horizontal");
        _moveVertical = Input.GetAxisRaw("Vertical");

        _rotationHorizontal = Input.GetAxisRaw("Mouse X") * _sensitivity;
        _rotationVertical += Input.GetAxisRaw("Mouse Y") * _sensitivity * (_invertYAxis ? -1 : 1);
    }

    private void Movement()
    {
        _movement.Set(_moveHorizontal, 0f, _moveVertical);

        // Quaternion * Vector3 esentially rotates the Vector3 towards the Quaternion direction
        _movement = transform.rotation * _movement;

        _characterController.Move(_movement * _walkSpeed * Time.deltaTime);
    }

    private void Rotation()
    {
        transform.Rotate(0f, _rotationHorizontal, 0f);

        _rotationVertical = Mathf.Clamp(_rotationVertical, _yMinAngle, _yMaxAngle);

        _head.localEulerAngles = new Vector3(_rotationVertical, 0f, 0f);
    }
}