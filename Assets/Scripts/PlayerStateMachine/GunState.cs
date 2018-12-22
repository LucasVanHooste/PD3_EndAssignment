using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunState : IState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private List<Collider> _triggers;
    private GameObject _object;

    private bool _isAiming;
    private GameObject _gun;

    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimationParameter = Animator.StringToHash("HorizontalVelocity");

    private int _jumpingAnimationParameter = Animator.StringToHash("Jumping");
    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");

    private int _punchParameter = Animator.StringToHash("Punch");

    public GunState(Transform playerTransform, PhysicsController physicsController, PlayerController playerController, AnimationsController animationsController, GameObject gun)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _triggers = _playerController.Triggers;
        _gun = gun;
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

        if (Input.GetButtonDown("Interact") && !_physicsController.Jumping && !_isAiming)
        {
            InteractWithObject();
        }
    }

    private void InteractWithObject()
    {
        Debug.Log(_triggers.Count);

        if (_triggers.Count == 0) return;

        _object = GetClosestTriggerObject();

        switch (_object.tag)
        {
            case "Gun":
                {
                    DropGun();
                    //_animator.GetBehaviour<PickUpGunStateBehaviour>().RightHand = _playerController.RightHand;
                    //_animator.GetBehaviour<PickUpGunStateBehaviour>().LeftHand = _playerController.LeftHand;
                    //_animator.GetBehaviour<PickUpGunStateBehaviour>().Player = _playerTransform;
                    //_animator.GetBehaviour<PickUpGunStateBehaviour>().Gun = _object.transform;
                    GunScript _gunScript = _object.GetComponent<GunScript>();
                    if (_gunScript.IsTwoHanded)
                    {
                        _gunScript.TakeGun(_playerTransform.gameObject.layer, _playerTransform);
                    }
                    else
                    {
                        _gunScript.TakeGun(_playerTransform.gameObject.layer, _playerController.RightHand);

                    }
                    //_object.transform.parent = _playerController.RightHand;
                    //_object.transform.position = _playerController.RightHand.position;
                    //_object.transform.localEulerAngles = new Vector3(0, -90, -90);

                    _gun = _object;
                    //_gun.layer = 9;
                }
                break;
        }
    }

    private void DropGun()
    {
            _gun.transform.parent = null;
            _gun.layer = 0;
            _gun.tag = "Gun";
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
