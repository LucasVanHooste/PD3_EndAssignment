using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgentController))]
public class EnemyBehaviour : MonoBehaviour
{

    private INode _behaviourTree;
    private Transform _transform;
    private NavMeshAgentController _navMeshAgentController;
    private EnemyBehaviour _meleeEnemyBehaviour;
    private Animator _animator;
    private AnimationsController _animationsController;
    private Transform _playerTransform;
    private PlayerController _playerController;

    [SerializeField] private float FOV;
    [SerializeField] private LayerMask _canSeePlayerLayerMask;
    [SerializeField] private float _minDistanceFromPlayer;
    [SerializeField] private float _horizontalPunchReach;
    [SerializeField] private float _verticalPunchReach;
    [SerializeField] private float _punchCoolDown;
    [SerializeField] private int _punchDamage;
    private float _punchCoolDownTimer = 0;
    [SerializeField] private float _forgetAboutPlayerTime;
    private float _forgetAboutPlayerTimer = 0;
    [SerializeField] private float _hearingDistance;

    [SerializeField] private Vector2 _roamingTimeRange;
    private float _roamingTime;
    private float _roamingTimer = 0;
    private bool _hasBeenAttacked = false;

    [SerializeField] private Transform _gun;
    private GunScript _gunScript;
    private bool _fireGun = false;
    private bool _aimGun = false;
    [SerializeField] float _missingShotRange;
    [SerializeField] private Transform _anchorPoint;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _lookAtPosition;
    [SerializeField] private Transform _headTransform;

    [SerializeField] private int _maxHealth;
    private int _health;
    public int Health
    {
        get
        {
            return _health;
        }
    }

    void Start()
    {
        _health = _maxHealth;
        _transform = transform;
        _navMeshAgentController = GetComponent<NavMeshAgentController>();
        _meleeEnemyBehaviour = GetComponent<EnemyBehaviour>();
        _animator = GetComponent<Animator>();
        _animationsController = new AnimationsController(_animator, _navMeshAgentController);
        _animationsController.HoldGunIK.Player = _transform;
        _animationsController.LookAtIK.LookAtPosition = _lookAtPosition;
        _playerTransform = _navMeshAgentController.PlayerTransform;
        _playerController = _playerTransform.GetComponent<PlayerController>();

        _forgetAboutPlayerTimer = _forgetAboutPlayerTime;
        _roamingTime = UnityEngine.Random.Range(_roamingTimeRange.x, _roamingTimeRange.y);

        PickUpGun(_gun);
        //_behaviourTree = new SelectorNode(

        //    new SequenceNode(
        //        new ConditionNode(SeesPlayer),
        //        new ParallelNode(
        //            OneSuccesIsSuccesAccumulator.Factory,
        //            new SequenceNode(
        //                new ConditionNode(IsCloseToPlayer), 
        //                new ActionNode(PunchPlayer)), 
        //            new ActionNode(SetPlayerPositionAsTarget))),
        //    new SequenceNode(
        //        new ConditionNode(HasBeenAttacked), 
        //        new ActionNode(SetPlayerPositionAsTarget)),

        //    new SequenceNode(
        //        new ConditionNode(HasSeenPlayerRecently), 
        //        new ActionNode(LookForPlayer)),

        //    new ActionNode(Roam));

        _behaviourTree = new SelectorNode(
new SelectorNode(
    new SequenceNode(
        new ConditionNode(SeesPlayer),
        new SelectorNode(
            new SequenceNode(
                new ConditionNode(HasGun),
                new ActionNode(FireGunAtPlayer)),
            new ParallelNode(
                OneSuccesIsSuccesAccumulator.Factory,
                new SequenceNode(
                    new ConditionNode(IsCloseToPlayer),
                    new ActionNode(PunchPlayer)),
                new ActionNode(SetPlayerPositionAsTarget))
            )),
    new SequenceNode(
        new ConditionNode(HasBeenAttacked),
        new ActionNode(SetPlayerPositionAsTarget)),

    new SequenceNode(
        new ConditionNode(HasSeenPlayerRecently),
        new ActionNode(LookForPlayer)),

    new ActionNode(Roam))
        );



        StartCoroutine(RunTree());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("rigidbody: " + _rigidBody.velocity);
        //Debug.Log("navmesh: " + _navMeshAgent.velocity);

        //Debug.Log("navmesh desired: " + _navMeshAgent.desiredVelocity);
        //Debug.Log("navmesh speed: " + _navMeshAgent.speed);

        if (_gun != null)
        {
            FireGun(_fireGun);
            AimGun(_aimGun);
        }
        _fireGun = false;
        _aimGun = false;

        _animationsController.Update();

        _punchCoolDownTimer += Time.deltaTime;
        _forgetAboutPlayerTimer += Time.deltaTime;
        _roamingTimer += Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * 2);

    }

    IEnumerator RunTree()
    {
        while (_health > 0)
        {
            yield return _behaviourTree.Tick();
        }
    }

    private IEnumerator<NodeResult> Roam()
    {
        Debug.Log("Roaming");
        if (_roamingTimer >= _roamingTime)
        {
            _roamingTime = UnityEngine.Random.Range(_roamingTimeRange.x, _roamingTimeRange.y);
            _roamingTimer = 0;
            _navMeshAgentController.Walk();
            _navMeshAgentController.SetDestination(_navMeshAgentController.RandomNavSphere(_transform.position, 3, -1));
        }
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> LookForPlayer()
    {
        Debug.Log("LookForPlayer");
        if (!_navMeshAgentController.UpdateNavmesh)
        {
            _navMeshAgentController.Warp(_transform.position);
            _navMeshAgentController.UpdateNavmesh = true;

        }

        if (_navMeshAgentController.HasNavMeshReachedDestination())
        {
            if ((_playerTransform.position - _transform.position).sqrMagnitude < _hearingDistance * _hearingDistance)
                _navMeshAgentController.SetDestination(_navMeshAgentController.RandomNavSphere(_playerTransform.position, 4, -1));
        }
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> SetPlayerPositionAsTarget()
    {
        if (_playerController.Health > 0)
        {
            if (Vector3.Scale(_playerTransform.position - _transform.position, new Vector3(1, 0, 1)).magnitude > _minDistanceFromPlayer)
            {
                //_navMeshAgent.isStopped = false;
                _navMeshAgentController.Run();

                if (!_navMeshAgentController.UpdateNavmesh)
                {
                    _navMeshAgentController.Warp(_transform.position);
                    _navMeshAgentController.UpdateNavmesh = true;
                }
            }
            else
            {
                _navMeshAgentController.UpdateNavmesh = false;
                _navMeshAgentController.RotateToPlayer();

                //_navMeshAgent.SetDestination(_playerTransform.position);
                //_navMeshAgent.updatePosition = false;
            }

            _navMeshAgentController.SetDestination(_playerTransform.position);
        }

        Debug.Log("SetPlayerposAsTarget");
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> PunchPlayer()
    {
        if (_punchCoolDownTimer > _punchCoolDown)
        {
            if (_playerController.Health > 0)
            {
                _playerController.TakePunch(_punchDamage, _transform.position);
                Debug.Log("punch");
                _punchCoolDownTimer = 0;
                _animationsController.Punch();
            }
        }

        Debug.Log("TryPunch");
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> FireGunAtPlayer()
    {
        if (!_navMeshAgentController.IsOnOffMeshLink())
        {
            _navMeshAgentController.UpdateNavmesh = false;
            _navMeshAgentController.RotateToPlayer();

            if (_playerController.Health > 0)
            {
                _aimGun = true;
                _fireGun = true;
            }
        }

        Debug.Log("Fire gun");
        yield return NodeResult.Succes;
    }

    bool SeesPlayer()
    {
        Vector3 directionPlayer = _playerTransform.position - _transform.position;
        if (Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionPlayer)) < FOV / 2)
        {
            //Debug.Log("angle: " + Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionPlayer)));
            RaycastHit hit;
            if (Physics.Raycast(_headTransform.position, directionPlayer, out hit, 1000, _canSeePlayerLayerMask))
            {
                //Debug.Log(hit.transform.name);
                if (hit.transform.gameObject.layer == 9)
                {
                    Debug.Log("I see player");
                    _forgetAboutPlayerTimer = 0;
                    _hasBeenAttacked = false;
                    //if (_gun != null && !_navMeshAgentController.IsOnOffMeshLink())
                    //{
                    //    AimGun(true);
                    //}

                    return true;
                }
            }
        }

        //if (_gun != null|| _navMeshAgentController.IsOnOffMeshLink())
        //{
        //    AimGun(false);
        //}
        return false;
    }

    private bool HasSeenPlayerRecently()
    {
        if (_forgetAboutPlayerTimer < _forgetAboutPlayerTime)
        {
            return true;
        }
        return false;
    }

    private bool IsCloseToPlayer()
    {
        if (Vector3.Magnitude(Vector3.Scale(_playerTransform.position - _transform.position, new Vector3(1, 0, 1))) <= _horizontalPunchReach
            && _playerTransform.position.y - _transform.position.y <= _verticalPunchReach)
        {
            return true;
        }
        else
            return false;
    }

    private bool HasBeenAttacked()
    {
        if (_hasBeenAttacked)
        {
            return true;
        }
        return false;
    }

    private bool HasGun()
    {
        if (_gun != null)
        {
            return true;
        }
        return false;
    }

    private void PickUpGun(Transform gun)
    {
        if (gun != null && gun.GetComponent<GunScript>())
        {
            _gun = gun;
            _gunScript = _gun.GetComponent<GunScript>();
            _gunScript.TakeGun(_rightHandTransform, _anchorPoint);
            _animationsController.HoldGunIK.Gun = _gun.transform;
            _animationsController.IsTwoHandedGun(_gunScript.IsTwoHanded);
        }
        else
        {
            _gun = null;
        }
    }

    private void AimGun(bool aim)
    {
        _animationsController.AimGun(aim);
        _gunScript.AimGun(aim);
    }

    private void FireGun(bool fire)
    {
        Vector3 randomPosition = UnityEngine.Random.insideUnitSphere * _missingShotRange;

        _gunScript.EnemyFireGun(fire, _anchorPoint.position, (_playerTransform.position+randomPosition)-_transform.position);
    }

    private void TakeDamage(int damage, Vector3 originOfDamage)
    {
        _health -= damage;
        _animationsController.TakeDamage();
        //_animationsController.SetHealth(_health);
        Die(originOfDamage);
    }

    public void TakePunch(int damage, Vector3 originOfDamage)
    {
        _hasBeenAttacked = true;
        TakeDamage(damage, originOfDamage);
    }

    public void GetShot(int damage, Vector3 originOfDamage)
    {
        _hasBeenAttacked = true;
        TakeDamage(damage, originOfDamage);
    }

    private void Die(Vector3 originOfDamage)
    {
        if (_health > 0) return;
        //GameObject.Destroy(gameObject);

        Vector3 transformedOrigin = GetTransformedOrigin(originOfDamage);

        _animationsController.Die(transformedOrigin.x, transformedOrigin.z);
        _navMeshAgentController.Stop(true);
        gameObject.layer = LayerMask.NameToLayer("NoCollisionWithPlayer");
    }

    private Vector3 GetTransformedOrigin(Vector3 origin)
    {
        Vector3 transformedOrigin = _transform.InverseTransformPoint(origin);

        if (Mathf.Abs(transformedOrigin.x) > Mathf.Abs(transformedOrigin.z))
        {
            transformedOrigin.x = transformedOrigin.x / Mathf.Abs(transformedOrigin.x);
            transformedOrigin.z = 0;
        }
        else
        {
            transformedOrigin.z = transformedOrigin.z / Mathf.Abs(transformedOrigin.z);
            transformedOrigin.x = 0;
        }

        return transformedOrigin;
    }

    public void ToDeadState()
    {
        //die
    }

}
