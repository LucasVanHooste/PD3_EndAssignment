using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : BasePlayerState
{
    private PlayerMotor _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;

    private GameObject _object;

    public DeadState(PlayerMotor physicsController, PlayerController playerController, AnimationsController animationsController)
    {
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        
    }

    public override void ResetState(IInteractable interactable)
    {
        
    }

    public override void OnStateEnter()
    {
        _animationsController.ApplyRootMotion(true);
        _playerController.StartCoroutine(RestartGame());
    }

    public override void OnStateExit()
    {
        _animationsController.ApplyRootMotion(false);
    }

    private IEnumerator RestartGame()
    {
        _physicsController.Aim = Vector3.zero;
        _physicsController.Movement = Vector3.zero;

        yield return new WaitForSeconds(4);
        _playerController.Respawn();
    }

    public override void Update()
    {

    }

}
