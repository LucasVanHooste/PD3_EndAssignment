using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CameraController))]
public class PhysicsController : MonoBehaviour {

    [Header("Locomotion Parameters")]
    [SerializeField]
    private float _mass = 75; // [kg]

    [SerializeField]
    public float MaxRunningSpeed = (30.0f * 1000) / (60 * 60);

    [SerializeField]
    private float _acceleration = 3; // [m/s^2]

    [SerializeField]
    private float _jumpHeight = 1; // [m/s^2]

    [SerializeField]
    private float _dragOnGround = 1; // []

    [SerializeField]
    private float _dragInAir = 1; // []

    [SerializeField]
    private float _dragWhileFalling = 1; // []

    private CharacterController _characterController;
    private CameraController _cameraController;
    private Transform _playerTransform;

    public Vector3 Movement { get; set; }
    public Vector3 Aim { get; set; }
    public bool Jump { get; set; }
    //public bool Jumping { get; set; }

    private Vector3 _velocity = Vector3.zero;
    private float _timeInAir = 0;
    private float _skinWidth;
    private float _prevPosY;
    private bool _hasGravity = true;

    [SerializeField] private float _horizontalRotationSpeed;
    [SerializeField] private float _verticalRotationSpeed;
    [SerializeField] private LayerMask _mapLayerMask;

    // Use this for initialization
    void Start() {
        _characterController = GetComponent<CharacterController>();
        _cameraController = GetComponent<CameraController>();
        _playerTransform = transform;

        _skinWidth = _characterController.skinWidth;
    }

    private void Update()
    {

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

        _prevPosY = _playerTransform.position.y;
        _characterController.Move(_velocity * Time.deltaTime);
        //Debug.Log("Velocity: "+_velocity);
    }

    private void ApplyMovement()
    {
        if (_characterController.isGrounded)
        {
            var relativeMovement = RelativeDirection(Movement);
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
        if (_characterController.isGrounded)
        {
            _velocity -= Vector3.Project(_velocity, Physics.gravity.normalized);
            //Jumping = false;
        }
    }

    private void ApplyGravity()
    {
        if (_hasGravity && !_characterController.isGrounded)
        {
            _velocity += Physics.gravity * Time.deltaTime; // g[m/s^2] * t[s]
            _timeInAir += Time.fixedDeltaTime;
        }

    }

    private void ApplyJump()
    {
        //https://en.wikipedia.org/wiki/Equations_of_motion
        //v^2 = v0^2  + 2*a(r - r0)
        //v = 0
        //v0 = ?
        //a = 9.81
        //r = 1
        //r0 = 0
        //v0 = sqrt(2 * 9.81 * 1) 
        //but => g is inverted

        if (Jump && _characterController.isGrounded)
        {
            _velocity += -Physics.gravity.normalized * Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);
            Jump = false;
            //Jumping = true;
            _timeInAir = 0;
        }

    }

    private void ApplyAirDrag()
    {
        if (!_characterController.isGrounded)
        {
            _velocity.x = _velocity.x * (1 - Time.deltaTime * _dragInAir);

            if (_velocity.y < 0)
                _velocity.y = _velocity.y * (1 - Time.deltaTime * _dragWhileFalling);
        }
    }

    private void ApplyGroundDrag()
    {
        if (_characterController.isGrounded)
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
        _cameraController.RotateVertically(Aim.z * _verticalRotationSpeed);
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
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

    public bool IsGrounded()
    {
        if (_characterController.isGrounded || GetDistanceFromGround() < _skinWidth + 0.01f) //.01 is padding
        {

            return true;
        }
        return false;
    }

    //public bool IsGroundedAnimationCheck()
    //{
    //    if (IsGrounded() || Mathf.Approximately((float)System.Math.Round(_prevPosY, 1), (float)System.Math.Round(_playerTransform.position.y, 1)))
    //        return true;

    //    return false;
    //}

    public void HasGravity(bool hasGravity)
    {
        _hasGravity = hasGravity;
    }
}
