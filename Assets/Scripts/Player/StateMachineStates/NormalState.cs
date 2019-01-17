using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : BasePlayerState
{
    private Transform _playerTransform;
    private PlayerPhysicsController _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private List<Collider> _triggers;
    private GameObject _object;

    private float _punchCoolDownTimer = 0;

    public NormalState(Transform playerTransform, PlayerPhysicsController physicsController,PlayerController playerController, AnimationsController animationsController)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _triggers = _playerController.Triggers;

        _punchCoolDownTimer = _playerController.PunchCoolDown;
    }

    public override void Update()
    {
        if (Input.GetButtonDown("Jump") && _physicsController.IsGrounded())
        {
            _physicsController.Jump = true;
        }

        if (_physicsController.IsGrounded())
            _physicsController.Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _physicsController.Aim = new Vector3(Input.GetAxis("RightJoystickX"), 0, Input.GetAxis("RightJoystickY"));

        if (Input.GetButtonDown("Punch"))
        {
            if (_punchCoolDownTimer >= _playerController.PunchCoolDown)
            {
                Punch();
                _punchCoolDownTimer = 0;
            }

        }
        _punchCoolDownTimer += Time.deltaTime;

        if (Input.GetButtonDown("Interact") && _physicsController.IsGrounded())
        {
            InteractWithObject();
        }

        if (Input.GetButtonDown("HolsterGun"))
        {
            _playerController.ToGunState(null);
        }

    }

    private void InteractWithObject()
    {
        if (_triggers.Count <= 0) return;

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
                    if(!_object.GetComponent<LadderScript>().IsPersonClimbing)
                    _playerController.ToClimbingState(_object);
                }
                break;
            case "Turret":
                {
                    _playerController.ToTurretState(_object);
                }break;
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

    public override void PickUpGun()
    {
        if (_object.GetComponent<GunScript>())
        {
            GunScript _gunScript = _object.GetComponent<GunScript>();
                _gunScript.TakeGun(_playerController.RightHand, _playerController.CameraRoot);

            _animationsController.HoldGunIK.Gun=_object.transform;
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

    private void Punch()
    {
        _animationsController.Punch();
        RaycastHit hit;
        if (Physics.Raycast(_playerTransform.position+ new Vector3(0,1.5f,0), _playerTransform.forward, out hit, _playerController.PunchRange))
        {
            if (hit.transform.gameObject.layer == 16)
            {
                if (hit.transform.GetComponent<EnemyBehaviour>())
                    hit.transform.GetComponent<EnemyBehaviour>().TakePunch(_playerController.PunchDamage, _playerTransform.position);
            }
        }
    }
}
