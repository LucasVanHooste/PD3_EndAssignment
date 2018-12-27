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
        //Debug.Log(_physicsController.GetDistanceFromGround());
        if (Input.GetButtonDown("Jump") && _physicsController.IsGrounded())
        {
            _physicsController.Jump = true;
        }

        if (_physicsController.IsGrounded())
            _physicsController.Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _physicsController.Aim = new Vector3(Input.GetAxis("RightJoystickX"), 0, Input.GetAxis("RightJoystickY"));

        if (Input.GetButtonDown("Punch"))
        {
            _animationsController.Punch();
        }

        if (Input.GetButtonDown("Interact") && _physicsController.IsGrounded())
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
            case "Obstacle":
                {                 
                    _playerController.ToPushingState(_object);
                }
                break;
            case "FirstGun":
                {
                    
                    _playerController.ToCinematicState(_object);
                }
                break;
            case "Gun":
                {
                    PickUpGun();
                    RemoveTriggersFromList(_object.GetComponents<Collider>());

                    _playerController.ToGunState(_object);
                }
                break;
            case "Ladder":
                {

                    _playerController.ToClimbingState(_object);
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

    public void PickUpGun()
    {
        if (_object.GetComponent<GunScript>())
        {
            GunScript _gunScript = _object.GetComponent<GunScript>();
                _gunScript.TakeGun(_playerController.gameObject.layer, _playerController.RightHand, _playerController.CameraRoot/*, _animationsController.HoldGunIK*/);

            _animationsController.HoldGunIK.SetGun(_object.transform);
        }
    }

    public void RemoveTriggersFromList(Collider[] colliders)
    {
        for (int i = colliders.Length - 1; i >= 0; i--)
        {
            if (colliders[i].isTrigger)
            {
                if (_triggers.Contains(colliders[i]))
                    _triggers.Remove(colliders[i]);
            }

        }
    }

}
