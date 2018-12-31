using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : PlayerState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private List<Collider> _triggers;
    private GameObject _object;

    public DeadState(Transform playerTransform, PhysicsController physicsController, PlayerController playerController, AnimationsController animationsController)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _triggers = _playerController.Triggers;
        _playerController.StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        _physicsController.Aim = Vector3.zero;
        _physicsController.Movement = Vector3.zero;

        yield return new WaitForSeconds(3);
        _playerController.Respawn();
    }

    public override void Update()
    {

    }
}
