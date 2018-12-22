using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : IState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private List<Collider> _triggers;
    private GameObject _object;

    //private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    //private int _horizontalVelocityAnimationParameter = Animator.StringToHash("HorizontalVelocity");
    ////private int _aimingAnimationParameter = Animator.StringToHash("Aiming");
    //private int _jumpingAnimationParameter = Animator.StringToHash("Jumping");
    //private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    //private int _pushingAnimationParameter = Animator.StringToHash("Pushing");
    //private int _pickingUpGunParameter = Animator.StringToHash("PickingUpGun");
    //private int _punchParameter = Animator.StringToHash("Punch");

    public NormalState(Transform playerTransform, PhysicsController physicsController,PlayerController playerController, AnimationsController animationsController)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
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

        if (Input.GetButtonDown("Punch"))
        {
            _animationsController.Punch();
        }

        if (Input.GetButtonDown("Interact") && !_physicsController.Jumping)
        {
            InteractWithObject();
        }

        //_animator.SetFloat(_verticalVelocityAnimationParameter, _physicsController.Movement.z);
        //_animator.SetFloat(_horizontalVelocityAnimationParameter, _physicsController.Movement.x);
        //_animator.SetBool(_jumpingAnimationParameter, _physicsController.Jumping);
        //_animator.SetFloat(_horizontalRotationAnimationParameter, _physicsController.Aim.x);
    }

    private void InteractWithObject()
    {
        Debug.Log(_triggers.Count);

        if (_triggers.Count == 0) return;

        _object = GetClosestTriggerObject();

        switch (_object.tag)
        {
            case "Obstacle":
                {                 
                    _playerController.ToPushingState(_object);
                }
                break;
            case "FirstGun":
                {
                    _animationsController.SetPickUpGunStateBehaviour(_playerController.RightHand, _playerController.LeftHand, _playerTransform, _object.transform);
                    _playerController.ToCinematicState(_object);
                }
                break;
            case "Gun":
                {
                    _object.transform.parent = _playerController.RightHand;
                    _object.transform.position = _playerController.RightHand.position;
                    _object.transform.localEulerAngles = new Vector3(0, -90, -90);
                    _object.layer = 9;
                    _playerController.ToGunState(_object);
                }
                break;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        
    }

    public void OnTriggerExit(Collider other)
    {
        
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
    }

    private GameObject GetClosestTriggerObject()
    {
        Vector3 position = _playerTransform.position;
        float distance = 100;
        GameObject closest = null;
        foreach (Collider col in _triggers)
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
