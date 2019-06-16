using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CameraController))]
public class PlayerMotor : MonoBehaviour {

    [Header("Locomotion Parameters")]

    [SerializeField] public float MaxRunningSpeed = (30.0f * 1000) / (60 * 60);

    [SerializeField] private float _acceleration = 3; // [m/s^2]

    [SerializeField] private float _jumpHeight = 1; // [m/s^2]

    [SerializeField] private float _dragOnGround = 1;

    [SerializeField] private float _dragInAir = 1;

    [SerializeField] private float _walkingSpeedMultiplier;

    private CharacterController _characterController;
    private CameraController _cameraController;
    private Transform _playerTransform;

    private Vector3 _movement;
    private Vector3 _aim;

    public bool IsWalking { get; set; }
    public bool Jump { get; set; }
    public bool IsGrounded { get => _isGroundedChecker.IsGrounded; }
    public bool HasGravity { get; set; } = true;
    public Vector3 Movement
    {
        get
        {
            if (IsWalking)
                return Vector3.ClampMagnitude(_movement, _walkingSpeedMultiplier);
            return _movement;
        }
        set { _movement = value; }
    }
    public Vector3 Aim
    {
        get
        {
            if (IsWalking)
                return Vector3.ClampMagnitude(_aim, _walkingSpeedMultiplier);
            return _aim;
        }
        set { _aim = value; }
    }

    private Vector3 _velocity = Vector3.zero;
    public Vector3 Velocity { get => _velocity; }

    [SerializeField] private float _horizontalRotationSpeed;
    public float HorizontalRotationSpeed{ get => _horizontalRotationSpeed; }

    [SerializeField] private float _verticalRotationSpeed;
    public float VerticalRotationSpeed{ get => _verticalRotationSpeed; }

    [SerializeField] private LayerMask _mapLayerMask;
    [SerializeField] private IsGroundedCheckerScript _isGroundedChecker;
    public IsGroundedCheckerScript IsGroundedChecker { get=> _isGroundedChecker;}

    // Use this for initialization
    void Start() {
        _characterController = GetComponent<CharacterController>();
        _cameraController = GetComponent<CameraController>();
        _playerTransform = transform;
    }

    // Update is called once per frame
    void FixedUpdate() {
        ApplyGround();
        ApplyGravity();

        ApplyRotation();
        ApplyMovement();
        ApplyGroundDrag();

        ApplyAirDrag();
        ApplyJump();

        LimitMaximumRunningSpeed();

        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        if (IsGrounded)
        {
                Vector3 relativeMovement = RelativeDirection(Movement);
                _velocity += relativeMovement * _acceleration * Time.deltaTime; // F(= m.a) [m/s^2] * t [s]
        }
    }

    private Vector3 RelativeDirection(Vector3 direction)
    {
        Quaternion forwardRotation =
            Quaternion.LookRotation(Vector3.Scale(_playerTransform.forward, new Vector3(1, 0, 1)));

        Vector3 relativeMovement = forwardRotation * direction;
        return relativeMovement;
    }

    private void ApplyGround()
    {
        if (IsGrounded)
        {
            _velocity -= Vector3.Project(_velocity, Physics.gravity.normalized);
        }
    }

    private void ApplyGravity()
    {
        if (HasGravity && !IsGrounded)
        {
            _velocity += Physics.gravity * Time.deltaTime; // g[m/s^2] * t[s]
        }

    }

    private void ApplyJump()
    {
        if (Jump && IsGrounded)
        {
            _velocity += -Physics.gravity.normalized * Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);
            Jump = false;
        }

    }

    private void ApplyAirDrag()
    {
        if (!IsGrounded)
        {
            Vector3 xzVelocity = Vector3.Scale(_velocity, new Vector3(1, 0, 1));
            xzVelocity = xzVelocity * (1 - Time.deltaTime * _dragInAir);

            xzVelocity.y = _velocity.y;
            _velocity = xzVelocity;
        }
    }

    private void ApplyGroundDrag()
    {
        if (IsGrounded)
        {
            _velocity = _velocity * (1 - Time.deltaTime * _dragOnGround);
        }
    }

    private void LimitMaximumRunningSpeed()
    {
        Vector3 yVelocity = Vector3.Scale(_velocity, new Vector3(0, 1, 0));

        Vector3 xzVelocity = Vector3.Scale(_velocity, new Vector3(1, 0, 1));
        Vector3 clampedXzVelocity = Vector3.ClampMagnitude(xzVelocity, MaxRunningSpeed);

        _velocity = yVelocity + clampedXzVelocity;
    }

    public void ApplyRotation()
    {
        _characterController.transform.eulerAngles += new Vector3(0, Aim.x, 0) * _horizontalRotationSpeed * Time.deltaTime;
        _cameraController.RotateVertically(Aim.z * _verticalRotationSpeed * Time.deltaTime);
    }

    public void StopMoving()
    {
        Movement = Vector3.zero;
        _velocity = Vector3.zero;
    }

    public float GetDistanceFromGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(_playerTransform.position, Vector3.down, out hit, 1000, _mapLayerMask))
        {
            //print("I'm looking at " + hit.transform.name);
            return (hit.point - _playerTransform.position).magnitude;
        }
        //print("I'm looking at nothing!");
        return 1000;
    }

    public void SetPosition(Vector3 position)
    {
        _playerTransform.position = position;
    }

}
