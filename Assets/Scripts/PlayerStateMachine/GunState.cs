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
    private CameraController _cameraController;

    private bool _isAiming;
    private bool _isShooting;
    private GameObject _gun;
    private GunScript _gunScript;

    public GunState(Transform playerTransform, PhysicsController physicsController, PlayerController playerController, AnimationsController animationsController, GameObject gun, CameraController cameraController)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _triggers = _playerController.Triggers;
        _gun = gun;
        _gunScript = _gun.GetComponent<GunScript>();
        _cameraController = cameraController;

    }

    public void Update()
    {
        if (Input.GetButtonDown("Jump") && _physicsController.IsGrounded())
        {
            _physicsController.Jump = true;
        }

        if (_physicsController.IsGrounded())
            _physicsController.Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _physicsController.Aim = new Vector3(Input.GetAxis("RightJoystickX"), 0, Input.GetAxis("RightJoystickY"));

        if (Input.GetAxis("TriggerLeft") > 0.2f && _physicsController.IsGrounded())
        {
            //Debug.Log("Aim");
            _isAiming = true;
        }
        else _isAiming = false;

        AimGun();

        if (Input.GetAxis("TriggerRight") > 0.2f && _isAiming)
        {
            //Debug.Log("Shoot");
            _isShooting = true;
        }
        else _isShooting = false;

        ShootGun();

        if (Input.GetButtonDown("Interact") && _physicsController.IsGrounded() && !_isAiming)
        {
            InteractWithObject();
        }

        if (_physicsController.GetVelocity().y < -6.5f)
            DropGun();
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
                    _gunScript.DropGun();

                    PickUpGun();
                    RemoveTriggersFromList(_object.GetComponents<Collider>());
                    _gun = _object;
                    //_gunScript = _gun.GetComponent<GunScript>();

                }
                break;
        }
    }

    void AimGun()
    {
        _animationsController.AimGun(_isAiming);
        _animationsController.IsTwoHandedGun(_gunScript.IsTwoHanded);

        _cameraController.AimGun(_isAiming);
        _gunScript.AimGun(_isAiming);
    }

    private void ShootGun()
    {
        _gunScript.ShootGun(_isShooting);
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
                _gunScript = _object.GetComponent<GunScript>();
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

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {

    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void OnTriggerExit(Collider other)
    {

    }

    public void DropGun()
    {
        _gunScript.DropGun();

        _animationsController.HoldGunIK.SetGun(null);

        _playerController.ToNormalState();
    }
}
