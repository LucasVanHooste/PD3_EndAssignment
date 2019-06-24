using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : BasePlayerState, IInteractor
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
            _playerMotor.Movement = new Vector3(InputController.LeftJoystickX, 0, InputController.LeftJoystickY);

        _playerMotor.Aim = new Vector3(InputController.RightJoystickX, 0, InputController.RightJoystickY);


        if (InputController.JumpButtonDown && _playerMotor.IsGrounded)
        {
            _playerMotor.Jump = true;
            return;
        }

        
        if (InputController.InteractButtonDown && _playerMotor.IsGrounded)
        {
            InteractWithObject();
            return;
        }


        if (InputController.HolsterButtonDown)
        {
            _playerController.SwitchState<GunState>(null);
            return;
        }


        if (InputController.PunchButtonDown)
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

        if (interactable != null)
        {
            interactable.Interact(this);
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

    public void ObstacleInteraction(ObstacleScript obstacle)
    {
        _playerController.SwitchState<PushingState>(obstacle);
    }

    public void GunInteraction(GunScript gun)
    {
        _playerController.RemoveTriggersFromList(_closestGameObject.GetComponents<Collider>());
        _playerController.SwitchState<GunState>(gun);
    }

    public void LadderInteraction(LadderScript ladder)
    {
        if (!ladder.IsPersonClimbing)
        {
            _playerController.SwitchState<ClimbingState>(ladder);
        }
    }

    public void TurretInteraction(TurretScript turret)
    {
        _playerController.SwitchState<TurretState>(turret);
    }
}
