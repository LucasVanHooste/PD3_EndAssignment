using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingState : IState
{
    public const float ObstacleWidth=2;

    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;

    private List<Collider> _triggers;
    private GameObject _obstacle;

    //private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    //private int _horizontalVelocityAnimationParameter = Animator.StringToHash("HorizontalVelocity");

    //private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    //private int _pushingAnimationParameter = Animator.StringToHash("Pushing");

    private GameObject _obstacleCollisionChecker;
    bool _hasHitObstacle = false;
    private Rigidbody _rigidBodyObstacle;
    private Vector3 _pushStartPosition;
    private bool _isFacingObstacle = false;

    public PushingState(Transform playerTransform, PhysicsController physicsController,PlayerController playerController, AnimationsController animationsController, GameObject obstacle)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _obstacle = obstacle;
        _triggers = _playerController.Triggers;

        InteractWithObstacle();
    }

    public void Update()
    {
        //_animationsController.SetFloat(_verticalVelocityAnimationParameter, _physicsController.Movement.z);
        //_animationsController.SetFloat(_horizontalVelocityAnimationParameter, _physicsController.Movement.x);
        //_animationsController.SetFloat(_horizontalRotationAnimationParameter, _physicsController.Aim.x);
    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void OnTriggerExit(Collider other)
    {

    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject == _obstacle)
        {
            if (!_hasHitObstacle)
            {
                _rigidBodyObstacle = hit.transform.GetComponent<Rigidbody>();
                _hasHitObstacle = true;
            }

        }
    }
    public void InteractWithObstacle()
    {
        _physicsController.Movement = Vector3.zero;
        _physicsController.Aim = Vector3.zero;
        _obstacleCollisionChecker = GameObject.Instantiate(_playerController._obstacleCollisionChecker, _obstacle.transform.position + GetDirection().normalized, Quaternion.LookRotation(GetDirection()));

        _playerController.StartCoroutine(RotateToObstacle());
    }

    private IEnumerator RotateToObstacle()
    {
        Vector3 direction = GetDirection();

        while(Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(direction)) > 1)
        {
            Debug.Log("rotate");
            Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, direction, .05f, 0.0f);

            float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
            _physicsController.Aim = new Vector3(angle / Mathf.Abs(angle), _physicsController.Aim.y, _physicsController.Aim.z);

            yield return true;
        }

        Debug.Log("finish rotation");
        _physicsController.Aim = Vector3.zero;
        _playerController.StartCoroutine(MoveToObstacle());
    }

    private IEnumerator MoveToObstacle()
    {
        _animationsController.Push(true);
        _physicsController.Movement = Vector3.forward / 2;
        yield return new WaitUntil(() => _hasHitObstacle);
        Debug.Log("obstacle hit");
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

        _rigidBodyObstacle.constraints = RigidbodyConstraints.None;
        _pushStartPosition = _playerTransform.position;
        //Vector3 _targetPosition = _playerTransform.position + GetDirection().normalized * 2;
        //Debug.Log(_pushStartPosition);
        //Debug.Log(_targetPosition);
        if (canPlayerPushObstacle)
        {
            float distance = Vector3.Magnitude(_pushStartPosition - _playerTransform.position);
            Debug.Log("can push");
            while (distance < ObstacleWidth-.02f)
            {
                distance = Vector3.Magnitude(_pushStartPosition - _playerTransform.position);
                Debug.Log(distance);
                _rigidBodyObstacle.velocity = _playerTransform.TransformVector(_physicsController.Movement);
                yield return false;
            }
            //while (!Mathf.Approximately(_playerTransform.position.magnitude, _targetPosition.magnitude))
            //{ 
            //    Vector3 lerp= Vector3.MoveTowards(_playerTransform.position, _targetPosition, 1f);
            //    Debug.Log(_playerTransform.position);
            //    //_rigidBodyObstacle.position+=lerp-_playerTransform.position;
            //    _physicsController.Movement = new Vector3(0, 0, (lerp-_playerTransform.position).magnitude);
            //    _rigidBodyObstacle.velocity = _playerTransform.TransformVector(_physicsController.Movement);
            //    yield return false;
            //}
        }
        else
        {
            yield return new WaitForSeconds(1);
        }


        StopPushing();
    }

    private void StopPushing()
    {
        _rigidBodyObstacle.velocity = Vector3.zero;
        _rigidBodyObstacle.constraints = RigidbodyConstraints.FreezeAll;
        _animationsController.Push(false);
        _hasHitObstacle = false;
        _isFacingObstacle = false;

        if (_obstacleCollisionChecker.GetComponent<ObstacleCollisionCheckerScript>().GetHasGravity())
        {
            _rigidBodyObstacle.constraints = RigidbodyConstraints.None;
            _rigidBodyObstacle.useGravity = true;
            Collider[] triggers = _rigidBodyObstacle.GetComponents<Collider>();
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
        return direction;
    }

    public void PickUpGun()
    {

    }
}
