using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : BasePlayerState
{
    private Transform _playerTransform;
    private PlayerMotor _playerMotor;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private List<Collider> _triggers;
    private GameObject _closestGameObject;

    private float _punchCoolDownTimer = 0;

    public NormalState(PlayerMotor playerMotor,PlayerController playerController, AnimationsController animationsController)
    {
        _playerTransform = PlayerController.PlayerTransform;
        _playerMotor = playerMotor;
        _playerController = playerController;
        _animationsController = animationsController;
        _triggers = _playerController.Triggers;
    }

    public override void ResetState(IInteractable interactable)
    {
        _punchCoolDownTimer = _playerController.PunchCoolDown;
    }

    public override void OnStateEnter()
    {

    }

    public override void OnStateExit()
    {

    }

    public override void Update()
    {
        
        if (_playerMotor.IsGrounded)
            _playerMotor.Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _playerMotor.Aim = new Vector3(Input.GetAxis("RightJoystickX"), 0, Input.GetAxis("RightJoystickY"));


        if (Input.GetButtonDown("Jump") && _playerMotor.IsGrounded)
        {
            _playerMotor.Jump = true;
            return;
        }

        
        if (Input.GetButtonDown("Interact") && _playerMotor.IsGrounded)
        {
            InteractWithObject();
            return;
        }


        if (Input.GetButtonDown("HolsterGun"))
        {
            _playerController.SwitchState<GunState>(null);
            return;
        }


        if (Input.GetButtonDown("Punch"))
        {
            if (_punchCoolDownTimer >= _playerController.PunchCoolDown)
            {
                Punch();
                _punchCoolDownTimer = 0;
            }

        }
        _punchCoolDownTimer += Time.deltaTime;
    }

    private void InteractWithObject()
    {
        if (_triggers.Count <= 0) return;

        _closestGameObject = _playerController.GetClosestTriggerObject();
        IInteractable interactable = _closestGameObject.GetComponent<IInteractable>();

        switch (interactable)
        {
            case ObstacleScript obstacle:
                {
                    _playerController.SwitchState<PushingState>(obstacle);
                }
                break;
            case GunScript gun:
                {
                    _playerController.RemoveTriggersFromList(_closestGameObject.GetComponents<Collider>());
                    _playerController.SwitchState<GunState>(gun);
                }
                break;
            case LadderScript ladder:
                {
                    if (!ladder.IsPersonClimbing)
                    {
                        _playerController.SwitchState<ClimbingState>(ladder);
                    }
                }
                break;
            case TurretScript turret:
                {
                    _playerController.SwitchState<TurretState>(turret);
                }
                break;
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
                    hit.transform.GetComponent<EnemyBehaviour>().TakeDamage(_playerController.PunchDamage, _playerTransform.position);
            }
        }
    }

   
}
