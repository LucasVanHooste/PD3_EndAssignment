using System;
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
    private ObstacleScript _obstacleScript;
    private GameObject _obstacleCollisionChecker;
    private Transform _obstacleIKLeftHand;
    private Transform _obstacleIKRightHand;

    private Vector3 _direction;
    bool _hasHitObstacle = false;

    public PushingState(PlayerMotor physicsController, PlayerController playerController, AnimationsController animationsController)
    {
        _playerTransform = PlayerController.PlayerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;

        _triggers = _playerController.Triggers;
    }

    public override void ResetState(IInteractable obstacle)
    {
        _obstacleScript = (ObstacleScript)obstacle;
        _obstacleIKLeftHand = _playerController.ObstacleIKLeftHand;
        _obstacleIKRightHand = _playerController.ObstacleIKRightHand;

        _hasHitObstacle = false;
    }

    public override void OnStateEnter()
    {
        InteractWithObstacle();
    }

    public override void OnStateExit()
    {
    }

    public override void Update()
    {
        
    }

    public override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject == _obstacleScript.gameObject)
        {
            _hasHitObstacle = true;
        }
    }

    public void InteractWithObstacle()
    {
        _physicsController.StopMoving();
        _physicsController.Aim = Vector3.zero;
        _direction = GetDirection();

        _obstacleCollisionChecker = GameObject.Instantiate(_obstacleScript.ObstacleCollisionCheckerPrefab, _obstacleScript.transform.position + _direction.normalized, Quaternion.LookRotation(_direction));

        _playerController.StartCoroutine(RotateToObstacle());
    }

    private IEnumerator RotateToObstacle()
    {

        while(Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(_direction)) > 1f)
        {
            Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, _direction, .05f, 0.0f);

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
            Vector3 pushStartPosition = _playerTransform.position;
            _obstacleScript.transform.parent = _playerTransform;
            float distance = Vector3.Magnitude(pushStartPosition - _playerTransform.position);

            while (distance < _obstacleScript.ObstacleWidth-.02f)
            {
                distance = Vector3.Magnitude(pushStartPosition - _playerTransform.position);
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
        _obstacleScript.transform.parent = null;
        _obstacleScript.SetConstraints(RigidbodyConstraints.FreezeAll);
        _animationsController.Push(false);
        _hasHitObstacle = false;

        //if obstacle is above pit, drop
        if (_obstacleCollisionChecker.GetComponent<ObstacleCollisionCheckerScript>().GetHasGravity())
        {
            _obstacleScript.SetConstraints(RigidbodyConstraints.None);
            _obstacleScript.UseGravity(true);

            Collider[] triggers = _obstacleScript.GetComponents<Collider>();
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
        _playerController.SwitchState<NormalState>();
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = Vector3.Scale((_obstacleScript.transform.position - _playerTransform.position), new Vector3(1, 0, 1));
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
