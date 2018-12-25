using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private List<Collider> _triggers;
    private GameObject _object;
    private Vector3 _startPosition;
    public Quaternion _startRotation;

    public DeadState(Transform playerTransform, PhysicsController physicsController, PlayerController playerController, AnimationsController animationsController, Vector3 startPosition, Quaternion startRotation)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _triggers = _playerController.Triggers;
        _startPosition = startPosition;
        _startRotation = startRotation;
        _playerController.StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        _physicsController.Aim = Vector3.zero;
        _physicsController.Movement = Vector3.zero;

        yield return new WaitForSeconds(3);
        _playerTransform.position = _startPosition;
        _playerTransform.rotation = _startRotation;
        _animationsController.ResetAnimations();
        _playerController.ToNormalState();
    }

    public void Update()
    {

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

    public void PickUpGun()
    {
    }

    //public void DropGun()
    //{
    //    throw new NotImplementedException();
    //}
}
