using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : PlayerState
{
    private PhysicsController _physicsController;
    private PlayerController _playerController;

    private GameObject _object;

    public DeadState(PhysicsController physicsController, PlayerController playerController)
    {
        _physicsController = physicsController;
        _playerController = playerController;

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
