using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PlayerState
{
    Normal, Pushing, Cinematic, HoldingGun
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
    //private bool _aiming;

    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimationParameter = Animator.StringToHash("HorizontalVelocity");
    //private int _aimingAnimationParameter = Animator.StringToHash("Aiming");
    private int _jumpingAnimationParameter = Animator.StringToHash("Jumping");
    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    private int _pushingAnimationParameter = Animator.StringToHash("Pushing");
    private int _pickingUpGunParameter = Animator.StringToHash("PickingUpGun");
    private int _punchParameter = Animator.StringToHash("Punch");

    private PlayerState _state;

    private GameObject _object;
    private bool _isFacingObstacle = false;
    private bool _hasHitObstacle=false;
    private bool _isPushing=false;
    private Vector3 _pushStartPosition;
    private Rigidbody _rigidBodyObstacle;
    [SerializeField] private GameObject _obstacleCollisionChecker;
    private List<Collider> _triggers = new List<Collider>();
    [SerializeField] private CinematicBehaviour _cinematicBehaviour;
    [SerializeField] private Transform PistolHandle;
    [SerializeField] private Transform RightHand;
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _camRotation;
    [SerializeField] private float _minCamAngle;
    [SerializeField] private float _maxCamAngle;
    private bool _isAiming;

    // Use this for initialization
    void Start () {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _state = PlayerState.Normal;

        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        //_animator.GetBehaviour<AimPistolBehaviour>().AimTarget = _aimTarget;
        ApplyRotation();
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

                    if (!_jumping)
                        _movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                    _aim = new Vector3(Input.GetAxis("RightJoystickX"), 0, Input.GetAxis("RightJoystickY"));

                    if (Input.GetButtonDown("Punch"))
                    {
                        _animator.SetTrigger(_punchParameter);
                    }
                }
                break;
            case PlayerState.Pushing:
                {


                }
                break;
            case PlayerState.Cinematic:
                {

                }break;
            case PlayerState.HoldingGun:
                {
                    if (Input.GetButtonDown("Jump") && !_jumping)
                    {
                        _jump = true;
                    }

                    //if (Input.GetButtonDown("Fire1"))
                    //{
                    //    _aiming = !_aiming;
                    //}

                    if (!_jumping)
                        _movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                    _aim = new Vector3(Input.GetAxis("RightJoystickX"), 0, Input.GetAxis("RightJoystickY"));

                    if (Input.GetAxis("TriggerLeft") > 0.2f && !_jumping)
                    {
                        Debug.Log("Aim");
                        _isAiming = true;
                    }
                    else _isAiming = false;

                    AimGun();
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

   
        Vector3 localVelocityXZ = Vector3.Scale(gameObject.transform.InverseTransformDirection(_velocity), new Vector3(1,0,1));

        Vector3 velocityXZNormalized = _characterController.velocity.normalized;

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
        _characterController.transform.eulerAngles += new Vector3(0, _aim.x, 0) * _rotationSpeed * Time.deltaTime;
        //vertical rotation
        _camRotation += _aim.z * _rotationSpeed * Time.deltaTime; //get vertical rotation
                                                                                  
        _camRotation = Mathf.Clamp(_camRotation, _minCamAngle, _maxCamAngle); //clamp vertical rotation
                                                                            
        _cameraRoot.eulerAngles = new Vector3(_camRotation, _cameraRoot.eulerAngles.y, _cameraRoot.eulerAngles.z);

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

        if (Input.GetButtonDown("Interact") && _state == PlayerState.Normal)
        {
                _object = GetClosestTriggerObject();
            if (_object == null) _object = other.gameObject;

            switch (_object.tag)
            {
                case "Obstacle":
                    {
                        //if (_state != PlayerState.Pushing)
                        //{
                            StartCoroutine(MoveObstacle());
                        //}

                    }break;
                case "FirstGun":
                    {
                        StartCoroutine(PickupGun());
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
            if (hit.gameObject==_object)
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
        _movement = Vector3.zero;
        _aim = Vector3.zero;
        _state = PlayerState.Pushing;
        GameObject _collisionCheck = GameObject.Instantiate(_obstacleCollisionChecker, _object.transform.position + GetDirection().normalized, Quaternion.LookRotation(GetDirection()));

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
            Collider[] triggers = _rigidBodyObstacle.GetComponents<Collider>();
            for (int i = triggers.Length-1; i >= 0; i--)
            {
                if (triggers[i].isTrigger)
                    triggers[i].enabled = false;
            }
        }

        GameObject.Destroy(_collisionCheck);
    }

    private bool IsFacingObstacle()
    {
        Vector3 direction = GetDirection();
        Vector3 newDir = Vector3.RotateTowards(_characterController.transform.forward, direction, .05f, 0.0f);
        //_characterController.transform.rotation = Quaternion.LookRotation(newDir);

        float angle = Vector3.SignedAngle(_characterController.transform.forward, newDir, Vector3.up);
        _aim.x = angle/Mathf.Abs(angle);

        if (Quaternion.Angle(_characterController.transform.rotation, Quaternion.LookRotation(direction)) < 1)
        {
            _aim = Vector3.zero;
            return true;
        }


        return false;
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = Vector3.Scale((_object.transform.position - transform.position), new Vector3(1, 0, 1));
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            direction.z = 0;
        }
        else
        {
            direction.x = 0;
        }
        return direction*1000;
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

    private IEnumerator PickupGun()
    {
        Debug.Log("Pick up gun");
        _movement = Vector3.zero;
        _aim = Vector3.zero;
        _state = PlayerState.Cinematic;
        yield return new WaitUntil(IsFacingObject);
        yield return new WaitForSeconds(.2f);
        StartCoroutine(_cinematicBehaviour.PlayCinematicScene("PickUpFirstGun"));
        //PistolHandle.SetParent(_relativeForward);

        yield return new WaitForSeconds(1);
        _animator.SetBool(_pickingUpGunParameter, true);
        _animator.SetLayerWeight(1, 1);
        //StartCoroutine(LerpLayerWeight(1, 1, .02f));

        yield return new WaitUntil(_cinematicBehaviour.GetIsSceneFinished);
        _animator.SetBool(_pickingUpGunParameter, false);
        _animator.SetLayerWeight(1, 0);
        //StartCoroutine(LerpLayerWeight(1, 0, .03f));
        _state = PlayerState.HoldingGun;
    }

    private bool IsFacingObject()
    {
        Vector3 direction = Vector3.Scale((_object.transform.position - transform.position), new Vector3(1, 0, 1));
        Vector3 newDir = Vector3.RotateTowards(_characterController.transform.forward, direction, .05f, 0.0f);
        //_characterController.transform.rotation = Quaternion.LookRotation(newDir);

        //float angle = Quaternion.Angle(_characterController.transform.rotation, Quaternion.LookRotation(newDir));
        float angle = Vector3.SignedAngle(_characterController.transform.forward, newDir, Vector3.up);
        _aim.x = angle / Mathf.Abs(angle);
        Debug.Log("angle: " + angle);

        if (Quaternion.Angle(_characterController.transform.rotation, Quaternion.LookRotation(direction))  < 1)
        {
            _aim = Vector3.zero;
            return true;
        }


        return false;
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

    private IEnumerator LerpLayerWeight(int layerIndex, float targetWeight, float speed)
    {
        while(!Mathf.Approximately(_animator.GetLayerWeight(layerIndex), targetWeight))
        {
            float weight = _animator.GetLayerWeight(layerIndex);
            weight = Mathf.Lerp(weight, targetWeight, speed);
            _animator.SetLayerWeight(layerIndex, weight);
            yield return null;
        }


    }

    void AimGun()
    {
        if (_isAiming)
        {
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _cameraRoot.GetChild(1).position, .2f);
            _cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, _cameraRoot.GetChild(1).rotation, .2f);
        }
        else
        {
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _cameraRoot.GetChild(0).position, .2f);
            _cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, _cameraRoot.GetChild(0).rotation, .2f);
        }
    }
}
