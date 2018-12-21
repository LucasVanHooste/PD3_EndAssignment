using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingState : IState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private Animator _animator;
    private GameObject _obstacleCollisionChecker;

    private List<Collider> _triggers;
    private GameObject _obstacle;

    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimationParameter = Animator.StringToHash("HorizontalVelocity");

    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    private int _pushingAnimationParameter = Animator.StringToHash("Pushing");

    bool _hasHitObstacle = false;
    private Rigidbody _rigidBodyObstacle;
    private Vector3 _pushStartPosition;
    private bool _isFacingObstacle = false;

    public PushingState(Transform playerTransform, PhysicsController physicsController,PlayerController playerController, Animator animator, GameObject obstacle)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animator = animator;
        _obstacle = obstacle;
        _obstacleCollisionChecker = _playerController._obstacleCollisionChecker;
        _triggers = _playerController.Triggers;

        PushObstacle(playerController);
    }

    public void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void OnTriggerExit(Collider other)
    {

    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if (hit.gameObject.tag == "Obstacle")
        if (hit.gameObject == _obstacle)
        {
            if (!_hasHitObstacle)
            {
                _rigidBodyObstacle = hit.transform.GetComponent<Rigidbody>();
                _rigidBodyObstacle.constraints = RigidbodyConstraints.None;
                _pushStartPosition = _playerTransform.position;
                _hasHitObstacle = true;
            }

        }
    }

    public void PushObstacle(MonoBehaviour mono)
    {
        mono.StartCoroutine(MoveObstacle());
    }

    private IEnumerator MoveObstacle()
    {
        _physicsController.Movement = Vector3.zero;
        _physicsController.Aim = Vector3.zero;
        //_state = PlayerState.Pushing;
        GameObject _collisionCheck = GameObject.Instantiate(_playerController._obstacleCollisionChecker, _obstacle.transform.position + GetDirection().normalized, Quaternion.LookRotation(GetDirection()));

        yield return new WaitUntil(IsFacingObstacle);
        _animator.SetBool(_pushingAnimationParameter, true);
        _physicsController.Movement = Vector3.forward / 2;
        yield return new WaitUntil(() => _hasHitObstacle);

        if (!_collisionCheck.GetComponent<ObstacleCollisionCheckerScript>().GetHasCollided())
            yield return new WaitUntil(HasPushedObstacle);
        else
        {
            yield return new WaitForSeconds(1);
        }

        _rigidBodyObstacle.velocity = Vector3.zero;
        _rigidBodyObstacle.constraints = RigidbodyConstraints.FreezeAll;
        _animator.SetBool(_pushingAnimationParameter, false);
        _hasHitObstacle = false;
        _isFacingObstacle = false;
        //_state = PlayerState.Normal;

        if (_collisionCheck.GetComponent<ObstacleCollisionCheckerScript>().GetHasGravity())
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

        GameObject.Destroy(_collisionCheck);
        _playerController.ToNormalState();
    }

    private bool IsFacingObstacle()
    {
        Vector3 direction = GetDirection();
        Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, direction, .05f, 0.0f);
        //_characterController.transform.rotation = Quaternion.LookRotation(newDir);

        float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
        _physicsController.Aim = new Vector3(angle / Mathf.Abs(angle),_physicsController.Aim.y, _physicsController.Aim.z);

        if (Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(direction)) < 1)
        {
            _physicsController.Aim = Vector3.zero;
            return true;
        }


        return false;
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
        return direction * 1000;
    }

    private bool HasPushedObstacle()
    {
        float distance = Vector3.Magnitude(_pushStartPosition - _playerTransform.position);
        while (distance < 1.98f)
        {
            _rigidBodyObstacle.velocity = _playerTransform.TransformVector(_physicsController.Movement);
            return false;
        }

        return true;
    }
}
