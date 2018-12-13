using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PlayerState
{
    Normal, Pushing
}


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CharacterControllerBehaviour : MonoBehaviour {

    [Header("Locomotion Parameters")]
    [SerializeField]
    private float _mass = 75; // [kg]

    [SerializeField]
    private float _maxRunningSpeed = (30.0f * 1000)  /( 60 * 60); // [m/s], 30 km/h

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

    [Header("Dependencies")]
#pragma warning disable 649 //Never Initialized
    [SerializeField]
    private Transform _relativeForward;

    [SerializeField]
    private Transform _aimHandle;

    [SerializeField]
    private Transform _aimTarget;
#pragma warning restore 649

    [SerializeField] private float _rotationSpeed;

    private CharacterController _characterController;
    private Animator _animator;

    private Vector3 _velocity = Vector3.zero;

    private Vector3 _movement;
    private bool _jump;
    private bool _jumping;
    private Vector3 _aim;
    private bool _aiming;

    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimationParameter = Animator.StringToHash("HorizontalVelocity");
    //private int _aimingAnimationParameter = Animator.StringToHash("Aiming");
    private int _jumpingAnimationParameter = Animator.StringToHash("Jumping");
    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    private int _pushingAnimationParameter = Animator.StringToHash("Pushing");

    private PlayerState _state;

    private GameObject _obstacle;
    private bool _isFacingObstacle = false;
    private bool _hasHitObstacle=false;
    private bool _isPushing=false;
    private Vector3 _pushStartPosition;
    private Rigidbody _rigidBodyObstacle;
    [SerializeField] private GameObject _obstacleCollisionChecker;
    private List<Collider> _triggers = new List<Collider>();
    // Use this for initialization
    void Start () {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _state = PlayerState.Normal;

        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        //_animator.GetBehaviour<AimPistolBehaviour>().AimTarget = _aimTarget;
    }
	
	// Update is called once per frame
	void Update () {

        switch (_state)
        {
            case PlayerState.Normal:
                {
                    if (Input.GetButtonDown("Jump") && !_jumping)
                    {
                        _jump = true;
                    }

                    if (Input.GetButtonDown("Fire1"))
                    {
                        _aiming = !_aiming;
                    }

                    if (!_jumping)
                        _movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                    _aim = new Vector3(Input.GetAxis("RightJoystickX"), 0, Input.GetAxis("RightJoystickY"));

                    //if (Input.GetButtonDown("Interact"))
                    //    StartCoroutine(MoveObstacle());
                }
                break;
            case PlayerState.Pushing:
                {
                    //if (!_isFacingObstacle)
                    //{
                    //    Vector3 direction = Vector3.Scale((_obstacle.transform.position - transform.position), new Vector3(1, 0, 1));
                    //    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                    //    {
                    //        direction.z = 0;
                    //    }
                    //    else
                    //    {
                    //        direction.x = 0;
                    //    }
                    //    Vector3 newDir = Vector3.RotateTowards(_relativeForward.forward, direction, .05f, 0.0f);
                    //    _relativeForward.rotation = Quaternion.LookRotation(newDir);
                    //    Debug.Log("false");
                    //    if (_relativeForward.rotation == Quaternion.LookRotation(direction))
                    //        _isFacingObstacle = true;
                    //}
                    //else
                    //{
                        //if(!_hasHitObstacle)
                        //_movement = Vector3.forward;
                        //else
                        //{
                        //    _movement= new Vector3(0, 0, Input.GetAxis("Vertical")<0?0:Input.GetAxis("Vertical"));
                        //    Debug.Log(_movement);
                        //    if (_movement.z > 0)
                        //    {
                        //        //_isPushing = true;
                        //    }
                        //}

                        //if (Input.GetButtonDown("Interact"))
                        //{
                        //    _animator.SetBool(_pushingAnimationParameter, false);
                        //    _hasHitObstacle = false;
                        //    _isFacingObstacle = false;
                        //    _state = PlayerState.Normal;
                        //}

                    //}


                }
                break;
        }

            
    }

    private void FixedUpdate()
    {
        ApplyGround();
        ApplyGravity();

        ApplyRotation();
        ApplyMovement();
        ApplyGroundDrag();

        ApplyAirDrag();
        ApplyJump();


        LimitMaximumRunningSpeed();

        //Quaternion forwardRotation =
        //    Quaternion.LookRotation(Vector3.Scale(_relativeForward.forward, new Vector3(1, 0, 1)));

        //Vector3 relativeMovement = forwardRotation * _movement;

        //Vector3 velocityXZ = Vector3.Scale(_velocity, new Vector3(1, 0, 1));
        //Vector3 localVelocityXZ = gameObject.transform.InverseTransformDirection(velocityXZ);
        //Vector3 localVelocityXZ = gameObject.transform.InverseTransformDirection(relativeMovement);
        Vector3 localVelocityXZ = gameObject.transform.InverseTransformDirection(_velocity);

        //_animator.SetFloat(_verticalVelocityAnimationParameter, localVelocityXZ.z);
        //_animator.SetFloat(_horizontalVelocityAnimationParameter, localVelocityXZ.x);
        _animator.SetFloat(_verticalVelocityAnimationParameter, _movement.z);
        _animator.SetFloat(_horizontalVelocityAnimationParameter, _movement.x);
        //_animator.SetBool(_aimingAnimationParameter, _aiming);
        _animator.SetBool(_jumpingAnimationParameter, _jumping);
        _animator.SetFloat(_horizontalRotationAnimationParameter, _aim.x);

        //if (_aim.magnitude > 0.5f)
        //{
        //    Vector3 relativeAim = RelativeDirection(_aim);
        //    _aimHandle.rotation = Quaternion.LookRotation(relativeAim);

        //    _aimHandle.localRotation = Quaternion.Euler(
        //        _aimHandle.localRotation.eulerAngles.x,
        //        Mathf.Clamp((_aimHandle.localRotation.eulerAngles.y + 180) % 360, 0, 180) - 180,
        //        _aimHandle.localRotation.eulerAngles.z
        //    );
        //}

        _characterController.Move(_velocity * Time.deltaTime);
    }


    private void ApplyMovement()
    {
        if (_characterController.isGrounded)
        {
            var relativeMovement = RelativeDirection(_movement);

            _velocity += relativeMovement * _acceleration * Time.deltaTime; // F(= m.a) [m/s^2] * t [s]
        }
    }

    private Vector3 RelativeDirection(Vector3 direction)
    {
        Quaternion forwardRotation =
            Quaternion.LookRotation(Vector3.Scale(_relativeForward.forward, new Vector3(1, 0, 1)));

        Vector3 relativeMovement = forwardRotation * direction;
        return relativeMovement;
    }

    private void ApplyGround()
    {
        if (_characterController.isGrounded)
        {
            _velocity -= Vector3.Project(_velocity, Physics.gravity.normalized);
            _jumping = false;
        }
    }

    private void ApplyGravity()
    {
        if (!_characterController.isGrounded)
        {
            _velocity += Physics.gravity * Time.deltaTime; // g[m/s^2] * t[s]
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

        if (_jump && _characterController.isGrounded)
        {
            _velocity += -Physics.gravity.normalized * Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);
            _jump = false;
            _jumping = true;
        }
        
    }

    private void ApplyAirDrag()
    {
        if (!_characterController.isGrounded)
        {
            _velocity.x = _velocity.x * (1 - Time.deltaTime * _dragInAir);

            if(_velocity.y < 0)
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
        Vector3 yVelocity = Vector3.Scale(_velocity, new Vector3(0,1,0));

        Vector3 xzVelocity = Vector3.Scale(_velocity, new Vector3(1, 0, 1));
        Vector3 clampedXzVelocity = Vector3.ClampMagnitude(xzVelocity, _maxRunningSpeed);

        _velocity = yVelocity + clampedXzVelocity;
    }

    private void ApplyRotation()
    {
        _relativeForward.eulerAngles += new Vector3(0, _aim.x, 0)*_rotationSpeed;
    }

    private float CalculateHorizontalMovementAnimationValue()
    {
        return _velocity.x / _maxRunningSpeed;
    }

    private float CalculateVerticalMovementAnimationValue()
    {
        return _velocity.z / _maxRunningSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && !_triggers.Contains(other))
            _triggers.Add(other);
    }

    private void OnTriggerStay(Collider other)
    {

        if (Input.GetButtonDown("Interact") && _state != PlayerState.Pushing)
        {
                _obstacle = GetClosestTriggerObject();
            if (_obstacle == null) _obstacle = other.gameObject;

            switch (_obstacle.tag)
            {
                case "Obstacle":
                    {
                        if (_state != PlayerState.Pushing)
                        {
                            _movement = Vector3.zero;
                            _aim = Vector3.zero;
                            StartCoroutine(MoveObstacle());
                        }

                    }break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_triggers.Contains(other))
            _triggers.Remove(other);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if (hit.gameObject.tag == "Obstacle")
            if (hit.gameObject==_obstacle)
        {
            if (!_hasHitObstacle && _state==PlayerState.Pushing)
            {
                _rigidBodyObstacle = hit.transform.GetComponent<Rigidbody>();
                _rigidBodyObstacle.constraints = RigidbodyConstraints.None;
                _pushStartPosition = _relativeForward.position;
                _hasHitObstacle = true;
            }

        }
    }

    private IEnumerator MoveObstacle()
    {
        _state = PlayerState.Pushing;
        GameObject _collisionCheck = GameObject.Instantiate(_obstacleCollisionChecker, _obstacle.transform.position + GetDirection().normalized, Quaternion.LookRotation(GetDirection()));

        yield return new WaitUntil(IsFacingObstacle);
        _animator.SetBool(_pushingAnimationParameter, true);
        _movement = Vector3.forward/2;
        yield return new WaitUntil(()=>_hasHitObstacle);


 
        if (!_collisionCheck.GetComponent<ObstacleCollisionCheckerScript>().GetHasCollided())
        yield return new WaitUntil(HasPushedObstacle);
        else
        {
            yield return new WaitForSeconds(1);
        }

        _rigidBodyObstacle.velocity = Vector3.zero;
        _rigidBodyObstacle.constraints = RigidbodyConstraints.FreezeAll;
        _animator.SetBool(_pushingAnimationParameter, false);
        _hasHitObstacle = false;
        _isFacingObstacle = false;
        _state = PlayerState.Normal;

        if (_collisionCheck.GetComponent<ObstacleCollisionCheckerScript>().GetHasGravity())
        {
            _rigidBodyObstacle.constraints = RigidbodyConstraints.None;
            _rigidBodyObstacle.useGravity = true;
        }

        GameObject.Destroy(_collisionCheck);
    }

    private bool IsFacingObstacle()
    {
        Vector3 direction = GetDirection();
        Vector3 newDir = Vector3.RotateTowards(_relativeForward.forward, direction, .05f, 0.0f);
        _relativeForward.rotation = Quaternion.LookRotation(newDir);

        if (Mathf.Abs(_relativeForward.rotation.eulerAngles.y - Quaternion.LookRotation(direction).eulerAngles.y) < 1)
            return true;

        return false;
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = Vector3.Scale((_obstacle.transform.position - transform.position), new Vector3(1, 0, 1));
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            direction.z = 0;
        }
        else
        {
            direction.x = 0;
        }
        return direction*10;
    }

    private bool HasPushedObstacle()
    {
        float distance = Vector3.Magnitude(_pushStartPosition - _relativeForward.position);
        while (distance < 1.98f)
        {
            _rigidBodyObstacle.velocity = _relativeForward.TransformVector(_movement);
            return false;
        }

        return true;
    }

    private GameObject GetClosestTriggerObject()
    {
        Vector3 position = _relativeForward.position;
        float distance = 100;
        GameObject closest= null;
        foreach(Collider col in _triggers)
        {
            float tempDistance = Vector3.Magnitude(position - col.transform.position);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                closest = col.gameObject;
            }

        }
        return closest;
    }

}
