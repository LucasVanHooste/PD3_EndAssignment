using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunState : IState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private Animator _animator;
    private List<Collider> _triggers;
    private GameObject _object;

    private bool _isAiming;

    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimationParameter = Animator.StringToHash("HorizontalVelocity");

    private int _jumpingAnimationParameter = Animator.StringToHash("Jumping");
    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");

    private int _punchParameter = Animator.StringToHash("Punch");

    public GunState(Transform playerTransform, PhysicsController physicsController, PlayerController playerController, Animator animator)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animator = animator;
        _triggers = _playerController.Triggers;
    }

    public void Update()
    {
        if (Input.GetButtonDown("Jump") && !_physicsController.Jumping)
        {
            _physicsController.Jump = true;
        }

        if (!_physicsController.Jumping)
            _physicsController.Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _physicsController.Aim = new Vector3(Input.GetAxis("RightJoystickX"), 0, Input.GetAxis("RightJoystickY"));

        if (Input.GetAxis("TriggerLeft") > 0.2f && !_physicsController.Jumping)
        {
            Debug.Log("Aim");
            _isAiming = true;
        }
        else _isAiming = false;

        AimGun();

        _animator.SetFloat(_verticalVelocityAnimationParameter, _physicsController.Movement.z);
        _animator.SetFloat(_horizontalVelocityAnimationParameter, _physicsController.Movement.x);
        _animator.SetBool(_jumpingAnimationParameter, _physicsController.Jumping);
        _animator.SetFloat(_horizontalRotationAnimationParameter, _physicsController.Aim.x);
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        
    }

    public void OnTriggerExit(Collider other)
    {
        
    }

    void AimGun()
    {
        if (_isAiming)
        {
            //_cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _cameraRoot.GetChild(1).position, .2f);
            //_cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, _cameraRoot.GetChild(1).rotation, .2f);
        }
        else
        {
            //_cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _cameraRoot.GetChild(0).position, .2f);
            //_cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, _cameraRoot.GetChild(0).rotation, .2f);
        }
    }
}
