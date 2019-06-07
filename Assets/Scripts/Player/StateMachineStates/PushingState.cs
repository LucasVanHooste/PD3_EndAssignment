﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingState : BasePlayerState
{
    private Transform _playerTransform;
    private PlayerMotor _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;

    private List<Collider> _triggers;
    private GameObject _obstacle;
    private ObstacleScript _obstacleScript;
    private GameObject _obstacleCollisionChecker;
    private Transform _obstacleIKLeftHand;
    private Transform _obstacleIKRightHand;

    Vector3 direction;
    bool _hasHitObstacle = false;
    private Vector3 _pushStartPosition;

    public PushingState(Transform playerTransform, PlayerMotor physicsController, PlayerController playerController, AnimationsController animationsController, 
        GameObject obstacle, Transform obstacleIKLeftHand, Transform obstacleIKRightHand)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _obstacle = obstacle;
        _obstacleScript = obstacle.GetComponent<ObstacleScript>();
        _obstacleIKLeftHand = obstacleIKLeftHand;
        _obstacleIKRightHand = obstacleIKRightHand;
        _triggers = _playerController.Triggers;

        InteractWithObstacle();
    }

    public override void Update()
    {
        
    }

    public override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject == _obstacle)
        {
            _hasHitObstacle = true;
        }
    }

    public void InteractWithObstacle()
    {
        _physicsController.StopMoving();
        _physicsController.Aim = Vector3.zero;
        direction = GetDirection();

        _obstacleCollisionChecker = GameObject.Instantiate(_obstacleScript.ObstacleCollisionCheckerPrefab, _obstacle.transform.position + direction.normalized, Quaternion.LookRotation(direction));

        _playerController.StartCoroutine(RotateToObstacle());
    }

    private IEnumerator RotateToObstacle()
    {

        while(Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(direction)) > 1f)
        {
            Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, direction, .05f, 0.0f);

            float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
            _physicsController.Aim = new Vector3(Mathf.Rad2Deg * angle / _physicsController.HorizontalRotationSpeed, _physicsController.Aim.y, _physicsController.Aim.z);

            yield return true;
        }

        _physicsController.Aim = Vector3.zero;
        _playerController.StartCoroutine(MoveToObstacle());
    }

    private IEnumerator MoveToObstacle()
    {
        _animationsController.Push(true);
        _physicsController.Movement = Vector3.forward / 3;

        yield return new WaitUntil(() => _hasHitObstacle);

        _animationsController.ObstacleIK.SetIK(_obstacleIKLeftHand, _obstacleIKRightHand);
        _physicsController.Movement = Vector3.forward / 8;

        _playerController.StartCoroutine(PushObstacle(CheckIfObstacleCanBePushed()));

    }

    private bool CheckIfObstacleCanBePushed()
    {
        if (!_obstacleCollisionChecker.GetComponent<ObstacleCollisionCheckerScript>().GetHasCollided())
            return true;
        else
        {
            return false;
        }
    }

    private IEnumerator PushObstacle(bool canPlayerPushObstacle)
    {

        if (canPlayerPushObstacle)
        {
            _obstacleScript.SetConstraints(RigidbodyConstraints.None);
            _pushStartPosition = _playerTransform.position;
            _obstacle.transform.parent = _playerTransform;
            float distance = Vector3.Magnitude(_pushStartPosition - _playerTransform.position);

            while (distance < _obstacleScript.ObstacleWidth-.02f)
            {
                distance = Vector3.Magnitude(_pushStartPosition - _playerTransform.position);
                yield return false;
            }
        }
        else
        {
            yield return new WaitForSeconds(1);
        }


        StopPushing();
    }

    private void StopPushing()
    {
        //reset
        _obstacle.transform.parent = null;
        _obstacleScript.SetConstraints(RigidbodyConstraints.FreezeAll);
        _animationsController.Push(false);
        _hasHitObstacle = false;

        //if obstacle is above pit, drop
        if (_obstacleCollisionChecker.GetComponent<ObstacleCollisionCheckerScript>().GetHasGravity())
        {
            _obstacleScript.SetConstraints(RigidbodyConstraints.None);
            _obstacleScript.UseGravity(true);

            Collider[] triggers = _obstacle.GetComponents<Collider>();
            for (int i = triggers.Length - 1; i >= 0; i--)
            {
                if (triggers[i].isTrigger)
                {
                    if (_triggers.Contains(triggers[i]))
                        _triggers.Remove(triggers[i]);
                    triggers[i].enabled = false;
                }

            }
        }

        GameObject.Destroy(_obstacleCollisionChecker);
        _playerController.ToNormalState();
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = Vector3.Scale((_obstacle.transform.position - _playerTransform.position), new Vector3(1, 0, 1));
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            direction.z = 0;
        }
        else
        {
            direction.x = 0;
        }
        return direction.normalized;
    }
}