using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunState : PlayerState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private List<Collider> _triggers;
    private GameObject _object;
    private CameraController _cameraController;
    private Transform _holsterGun1Hand;
    private Transform _holsterGun2Hands;
    private GameObject _crossHair;

    private bool _isAiming;
    private bool _isShooting;
    private GameObject _gun;
    private GunScript _gunScript;

    private float _dropGunTime = 1;
    private float _dropGunTimer=0;
    private float _punchCoolDownTimer = 0;

    public GunState(Transform playerTransform, PhysicsController physicsController, PlayerController playerController, AnimationsController animationsController, GameObject gun, 
        CameraController cameraController, Transform holsterGun1Hand, Transform holsterGun2Hands, GameObject crossHair)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _triggers = _playerController.Triggers;
        _cameraController = cameraController;
        _holsterGun1Hand = holsterGun1Hand;
        _holsterGun2Hands = holsterGun2Hands;
        _crossHair = crossHair;

        if (gun == null)
        {
            GameObject tempGun = GetGunInHolster();
            if (tempGun != null)
                TakeGunFromHolster(tempGun);
            
        }
        else
        {
            _gun = gun;
            _gunScript = _gun.GetComponent<GunScript>();
        }

        _punchCoolDownTimer = _playerController.PunchCoolDown;
    }

    public override void Update()
    {
        if (_gun == null)
        {
            Debug.Log("to normal please");
            _playerController.ToNormalState();
            return;
        }

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

        FireGun();

        if (Input.GetButtonDown("Punch") && !_gunScript.IsTwoHanded && !_isAiming)
        {
            if (_punchCoolDownTimer >= _playerController.PunchCoolDown)
            {
                _animationsController.Punch();
                _punchCoolDownTimer = 0;
            }

        }
        _punchCoolDownTimer += Time.deltaTime;

        if (Input.GetButtonDown("Interact") && _physicsController.IsGrounded() && !_isAiming)
        {
            InteractWithObject();
        }

        if (Input.GetButton("Interact"))
        {
            _dropGunTimer += Time.deltaTime;
        }
        else
        {
            _dropGunTimer = 0;
        }

        if (_dropGunTimer >= _dropGunTime)
        {
            _isAiming = false;
            AimGun();
            DropGun();
        }

        if (_physicsController.GetVelocity().y < -6.5f)
            DropGun();

        if(Input.GetButtonDown("HolsterGun") && !_isAiming)
        {
            HolsterGun();
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
        _physicsController.IsWalking=_isAiming;

        _animationsController.AimGun(_isAiming);
        _animationsController.IsTwoHandedGun(_gunScript.IsTwoHanded);

        _crossHair.SetActive(_isAiming);
        _cameraController.AimGun(_isAiming);
        _gunScript.AimGun(_isAiming);
    }

    private void FireGun()
    {
        _gunScript.FireGun(_isShooting);
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

    public override void PickUpGun()
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

    public void DropGun()
    {
        _gunScript.DropGun();

        _animationsController.HoldGunIK.SetGun(null);

        _playerController.ToNormalState();
    }

    private void HolsterGun()
    {

        GameObject tempGun = GetGunInHolster();

        if (_gunScript.IsTwoHanded)
        {
            _gun.transform.parent = _holsterGun2Hands;
            _gun.transform.position = _holsterGun2Hands.position;
            _gun.transform.rotation = _holsterGun2Hands.rotation;
        }
        else
        {
            _gun.transform.parent = _holsterGun1Hand;
            _gun.transform.position = _holsterGun1Hand.position;
            _gun.transform.rotation = _holsterGun1Hand.rotation;
        }

        _animationsController.HoldGunIK.SetGun(null);


        if (tempGun!=null)
        {
            TakeGunFromHolster(tempGun);
        }
        else
        {
            _playerController.ToNormalState();
        }



    }

    private GameObject GetGunInHolster()
    {
        GameObject tempGun = null;

        if (_holsterGun1Hand.childCount > 0)
            tempGun = _holsterGun1Hand.GetChild(0).gameObject;

        if (_holsterGun2Hands.childCount > 0)
            tempGun = _holsterGun2Hands.GetChild(0).gameObject;

        return tempGun;
    }

    private void TakeGunFromHolster(GameObject gun)
    {
        _object = gun;

        PickUpGun();
        _gun = _object;
    }

    public override void Die()
    {
        DropGun();
    }
}
