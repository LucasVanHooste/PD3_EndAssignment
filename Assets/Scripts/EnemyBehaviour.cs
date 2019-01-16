﻿using System;
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
    private EnemyBehaviour _enemyBehaviour;
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
    [SerializeField]private Transform _holsterGun1Hand;
    [SerializeField]private Transform _holsterGun2Hands;
    private GunScript _gunScript;
    private bool _fireGun = false;
    private bool _aimGun = false;
    [SerializeField] float _missingShotRange;
    [SerializeField] private Transform _anchorPoint;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;
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

    private List<Collider> _triggers = new List<Collider>();
    private GameObject _object;
    public bool IsInteracting { get; set; }
    private Coroutine _climbLadder;
    private float _ladderPaddingDistance = 0.15f;

    [SerializeField] private Vector3 _jumpForce;
    [SerializeField] private float _maxDistancefromGun;
    [SerializeField] private RangeTriggerCheckerScript _rangeTriggerChecker;
    [SerializeField] private Vector3 _fallForce;
    private GameObject _targetGun;

    void Start()
    {
        _health = _maxHealth;
        _transform = transform;
        _navMeshAgentController = GetComponent<NavMeshAgentController>();
        _enemyBehaviour = GetComponent<EnemyBehaviour>();
        _animator = GetComponent<Animator>();
        _animationsController = new AnimationsController(_animator, _navMeshAgentController);
        _animationsController.HoldGunIK.Player = _transform;
        _animationsController.LookAtIK.LookAtPosition = _lookAtPosition;
        _animationsController.ClimbBottomLadderIK.LeftHand = _leftHand;
        _animationsController.ClimbBottomLadderIK.RightHand =_rightHand;
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

        _behaviourTree = new SequenceNode(
new ConditionNode(IsNotInteracting),
new SelectorNode(
    new SequenceNode(
        new ConditionNode(SeesPlayer),
        new SelectorNode(
            new SequenceNode(
                new ConditionNode(HasGun),
                new ActionNode(FireGunAtPlayer)),
            new ParallelNode(
                OneRunningIsRunningAccumulator.Factory,
                new SequenceNode(
                    new ConditionNode(IsWithinPunchRangeOfPlayer),
                    new ActionNode(PunchPlayer)),
                new SelectorNode(
                    new SequenceNode(
                        new ConditionNode(SeesGun),
                        new ConditionNode(IsGunCloserThanPlayer),
                        new ActionNode(RunForGun)),
                    new ActionNode(SetPlayerPositionAsTarget)))
            )),
    new SequenceNode(
        new ConditionNode(HasBeenAttacked),
        new ActionNode(SetPlayerPositionAsTarget)),
    new SequenceNode(
        new ConditionNode(SeesGun),
        new ActionNode(RunForGun)),
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
        if (_health <= 0) return;

        if (_navMeshAgentController.RigidBody.velocity.y < -6.5f)
            DropGun();

        if (_gun != null)
        {
            FireGun(_fireGun);
            AimGun(_aimGun);
        }
        _fireGun = false;
        _aimGun = false;

        if (_navMeshAgentController.IsOnOffMeshLink()&&!IsInteracting&& _triggers.Count > 0)
        {
            IsInteracting = true;
            _object = GetClosestTriggerObject();

            switch (_object.tag)
            {
                case "Ladder":
                    {
                        Debug.Log("ladder");
                        _climbLadder= StartCoroutine(InteractWithLadder());
                    }break;
                case "Jump":
                    {
                        Jump();
                    }break;
                case "Fall":
                    {
                        Fall();
                    }
                    break;
                default: IsInteracting = false; break;
            }
        }
        

        _animationsController.Update();

        _punchCoolDownTimer += Time.deltaTime;
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
        if (!_navMeshAgentController.UpdateTransformToNavmesh)
        {
            _navMeshAgentController.Warp(_transform.position);
            _navMeshAgentController.UpdateTransformToNavmesh = true;
            _navMeshAgentController.SetDestination(_navMeshAgentController.RandomNavSphere(_playerTransform.position, 4, -1));
        }

        if (_navMeshAgentController.HasNavMeshReachedDestination())
        {
            if ((_playerTransform.position - _transform.position).sqrMagnitude < _hearingDistance * _hearingDistance)
                _navMeshAgentController.SetDestination(_navMeshAgentController.RandomNavSphere(_playerTransform.position, 4, -1));
            else
                _forgetAboutPlayerTimer = _forgetAboutPlayerTime;
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

                if (!_navMeshAgentController.UpdateTransformToNavmesh)
                {
                    _navMeshAgentController.Warp(_transform.position);
                    _navMeshAgentController.UpdateTransformToNavmesh = true;
                }
            }
            else
            {
                _navMeshAgentController.UpdateTransformToNavmesh = false;
                _navMeshAgentController.RotateToPlayer();

                //_navMeshAgent.SetDestination(_playerTransform.position);
                //_navMeshAgent.updatePosition = false;
            }

            _navMeshAgentController.SetDestination(_playerTransform.position);
        }

        //Debug.Log("SetPlayerposAsTarget");
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
            _navMeshAgentController.UpdateTransformToNavmesh = false;
            _navMeshAgentController.RotateToPlayer();

            if (_playerController.Health > 0)
            {
                _aimGun = true;
                _fireGun = true;
            }
        }

        //Debug.Log("Fire gun");
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> RunForGun()
    {
        //if (_gun != null || GetGunInHolster() != null)
        //    yield return NodeResult.Failure;

        //if (_targetGun == null)
        //{
        //    _targetGun = _rangeTriggerChecker.GetClosestTriggerObjectWithTag("Gun");
        //    if (_targetGun != null)
        //    {
        //        .
        //        Debug.Log("set target ("+gameObject.name+")");
        //        _navMeshAgentController.Run();
        //        _navMeshAgentController.SetDestination(_targetGun.transform.position);
        //        yield return NodeResult.Running;
        //    }
        //}
        //else
        //{
        //    //pick up gun
        //    if (_triggers.Contains(_targetGun.GetComponent<Collider>()))
        //    {
        //        Debug.Log("gun in triggers");
        //        PickUpGun(_targetGun.transform);
        //        RemoveTriggersFromList(_targetGun.GetComponents<Collider>());
        //        _rangeTriggerChecker.RemoveTriggersFromList(_targetGun.GetComponents<Collider>());
        //        _targetGun = null;
        //        yield return NodeResult.Succes;
        //    }

        //    if (_navMeshAgentController.HasNavMeshReachedDestination() || _targetGun.transform.parent!=null)
        //    {
        //        _rangeTriggerChecker.RemoveTriggersFromList(_targetGun.GetComponents<Collider>());
        //        _targetGun = null;
        //        _roamingTime = _roamingTimer;
        //    }
        //    else
        //    {
        //        yield return NodeResult.Running;
        //    }
        //}

        //yield return NodeResult.Failure;

        //pick up gun
        if (_triggers.Contains(_targetGun.GetComponent<Collider>()))
        {
            Debug.Log("gun in triggers");
            PickUpGun(_targetGun.transform);
            RemoveTriggersFromList(_targetGun.GetComponents<Collider>());
            _rangeTriggerChecker.RemoveTriggersFromList(_targetGun.GetComponents<Collider>());
            _targetGun = null;
        }
        else
        {
            Debug.Log("set target (" + gameObject.name + ")");
            _navMeshAgentController.Run();
            _navMeshAgentController.SetDestination(_targetGun.transform.position);
        }
        yield return NodeResult.Succes;
    }

    private bool IsNotInteracting()
    {
        return !IsInteracting;
    }

    private bool SeesPlayer()
    {
        Vector3 directionPlayer = _playerTransform.position - _transform.position;
        if (Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionPlayer)) < FOV / 2)
        {
            //Debug.Log("angle: " + Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionPlayer)));
            RaycastHit hit;
            if (Physics.Raycast(_headTransform.position, directionPlayer, out hit, 100, _canSeePlayerLayerMask))
            {
                //Debug.Log(hit.transform.name);
                if (hit.transform.gameObject.layer == 9)
                {
                    Debug.Log("I see player");
                    _forgetAboutPlayerTimer = 0;
                    _hasBeenAttacked = false;
                    _anchorPoint.localEulerAngles = new Vector3(Vector3.SignedAngle(Vector3.Scale(_playerTransform.position - _transform.position,new Vector3(1,0,1)), _playerTransform.position - _transform.position, _transform.right), 0, 0);


                    return true;
                }
            }
        }
        _anchorPoint.localEulerAngles = Vector3.zero;

        return false;
    }

    private bool HasSeenPlayerRecently()
    {
        _forgetAboutPlayerTimer += Time.deltaTime;
        if (_forgetAboutPlayerTimer < _forgetAboutPlayerTime)
        {
            Debug.Log("has seen recently");
            return true;
        }
        return false;
    }

    private bool IsWithinPunchRangeOfPlayer()
    {
        if (Vector3.Magnitude(Vector3.Scale(_playerTransform.position - _transform.position, new Vector3(1, 0, 1))) <= _horizontalPunchReach
            && _playerTransform.position.y - _transform.position.y <= _verticalPunchReach)
        {
            return true;
        }
        else
            return false;
    }

    private bool IsGunCloserThanPlayer()
    {
        if (_targetGun != null)
        {
            if ((_playerTransform.position - _transform.position).sqrMagnitude > (_targetGun.transform.position - _transform.position).sqrMagnitude
                && (_targetGun.transform.position - _transform.position).sqrMagnitude<_maxDistancefromGun*_maxDistancefromGun)
            {
                Debug.Log("Gun is closer");
                return true;
            }
        }
        return false;
    }

    private bool SeesGun()
    {
        if (GetGunInHolster() != null) return false;

        //get all guns that are closeby
        List<GameObject> _targetGuns = _rangeTriggerChecker.GetTriggerObjectsWithTag("Gun");
        if (_targetGuns.Count <= 0) return false;

        //get the closest gun the player can see
        float distance = 100;
        foreach (GameObject gun in _targetGuns)
        {
            RaycastHit hit;
            if (Physics.Raycast(_headTransform.position, gun.transform.position - _headTransform.position, out hit, 100, _canSeePlayerLayerMask))
            {
                if (hit.collider.CompareTag("Gun"))
                {
                    float tempDistance = Vector3.SqrMagnitude(gun.transform.position- _transform.position);
                    if (tempDistance < distance)
                    {
                        distance = tempDistance;
                        _targetGun = gun.gameObject;
                    }
                    Debug.Log("I see gun");
                }
            }
        }

        if (_targetGun != null) return true;
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

        if (GetGunInHolster() != null)
        {
            TakeGunFromHolster(GetGunInHolster());
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
            _gunScript.TakeGun(_rightHand, _anchorPoint);
            _animationsController.HoldGunIK.Gun = _gun.transform;
            _animationsController.IsTwoHandedGun(_gunScript.IsTwoHanded);
        }
        else
        {
            _gun = null;
        }
    }

    private void DropGun()
    {
        if (_gun == null) return;

        _aimGun = false;
        AimGun(_aimGun);

        _gunScript.DropGun();

        _animationsController.HoldGunIK.Gun = null;
        _gun = null;
    }

    private void DropHolsterGun()
    {
        if (GetGunInHolster() != null)
            GetGunInHolster().GetComponent<GunScript>().DropGun();
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
    private void HolsterGun()
    {
        if (_gun == null) return;

        GameObject tempGun = GetGunInHolster();

        if (_gunScript.IsTwoHanded)
        {
            _gun.transform.parent = _holsterGun2Hands;
            _gun.transform.position = _holsterGun2Hands.position;
            _gun.transform.rotation = _holsterGun2Hands.rotation;
        }
        else
        {
            _gun.transform.parent = _holsterGun1Hand;
            _gun.transform.position = _holsterGun1Hand.position;
            _gun.transform.rotation = _holsterGun1Hand.rotation;
        }

        _animationsController.HoldGunIK.Gun = null;


        if (tempGun != null)
        {
            TakeGunFromHolster(tempGun);
        }
        else
        {
            _gun = null;
        }
    }

    private GameObject GetGunInHolster()
    {
        GameObject tempGun = null;

        if (_holsterGun1Hand.childCount > 0)
            tempGun = _holsterGun1Hand.GetChild(0).gameObject;

        if (_holsterGun2Hands.childCount > 0)
            tempGun = _holsterGun2Hands.GetChild(0).gameObject;

        return tempGun;
    }

    private void TakeGunFromHolster(GameObject gun)
    {
        _gun = gun.transform;

        PickUpGun(_gun);
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

        if (_climbLadder != null)
        {
            StopCoroutine(_climbLadder);
        }

        if (_object!=null&& _object.GetComponent<LadderScript>())
            _object.GetComponent<LadderScript>().IsPersonClimbing = false;

        Vector3 transformedOrigin = GetTransformedOrigin(originOfDamage);

        _animationsController.Die(transformedOrigin.x, transformedOrigin.z);
        _animationsController.Climb(false);
        _animationsController.ApplyRootMotion(false);
        _navMeshAgentController.RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        _navMeshAgentController.RigidBody.isKinematic = false;  
        _navMeshAgentController.RigidBody.useGravity = true;

        DropGun();
        DropHolsterGun();

        ToDeadState();
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
        _health = 0;
        _navMeshAgentController.Stop(true);
        gameObject.layer = LayerMask.NameToLayer("NoCollisionWithPlayer");
    }

    private void Jump()
    {
        _navMeshAgentController.UpdateTransformToNavmesh = false;

        _navMeshAgentController.RigidBody.useGravity = true;
        _navMeshAgentController.RigidBody.isKinematic = false;
        if(Vector3.Angle(_transform.forward, _object.transform.forward) > 145)
            _navMeshAgentController.RigidBody.AddForce(_transform.TransformVector(_jumpForce), ForceMode.Impulse);
        else
            _navMeshAgentController.RigidBody.AddForce((Vector3.Scale(_object.transform.position - _transform.position, new Vector3(1,0,1)).normalized*_jumpForce.z)+new Vector3(0,_jumpForce.y,0),ForceMode.Impulse);
        Debug.Log("angle: " + Vector3.Angle(_transform.forward, _object.transform.forward));
    }

    private void Fall()
    {
        _navMeshAgentController.UpdateTransformToNavmesh = false;

        _navMeshAgentController.RigidBody.useGravity = true;
        _navMeshAgentController.RigidBody.isKinematic = false;

        _navMeshAgentController.RigidBody.AddForce((Vector3.Scale(_object.transform.position - _transform.position, new Vector3(1, 0, 1)).normalized * _fallForce.z) + new Vector3(0, _fallForce.y, 0), ForceMode.Impulse);
    }

    private IEnumerator InteractWithLadder()
    {
        LadderScript ladderScript = _object.GetComponent<LadderScript>();

        _animationsController.ClimbBottomLadderIK.LadderIKHands = ladderScript.BottomLadderIKHands;
        _animationsController.ClimbTopLadderPart1IK.Ladderscript = ladderScript;
        _animationsController.ClimbTopLadderPart2IK.SetBehaviour(_enemyBehaviour, _navMeshAgentController, _animationsController);

        _navMeshAgentController.UpdateTransformToNavmesh = false;
        _navMeshAgentController.RigidBody.isKinematic = false;
        _navMeshAgentController.RigidBody.useGravity = false;

        while (ladderScript.IsPersonClimbing)
        {
            Debug.Log("wait for climb");
            yield return null;
        }

        HolsterGun();
        ladderScript.IsPersonClimbing = true;
        _climbLadder = StartCoroutine(RotateToLadder());
    }

    private IEnumerator RotateToLadder()
    {
        Vector3 direction = -_object.transform.forward;

        while (Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(direction)) > 1)
        {
            Debug.Log("rotate");
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime* 90f);
            //Vector3 newDir = Vector3.RotateTowards(_transform.forward, direction, .05f, 0.0f);

            //float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
            ////_physicsController.Aim = new Vector3(Mathf.Rad2Deg * angle / _physicsController.HorizontalRotationSpeed, _physicsController.Aim.y, _physicsController.Aim.z);

            //_transform.eulerAngles += new Vector3(0, angle, 0);
            yield return null;
        }

        Debug.Log("finish rotation");
        _navMeshAgentController.RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        _climbLadder = StartCoroutine(MoveToLadder());
    }

    private IEnumerator MoveToLadder()
    {
        Vector3 ladderPosition = new Vector3(_object.GetComponent<LadderScript>().TakeOfPoint.position.x, _transform.position.y, _object.GetComponent<LadderScript>().TakeOfPoint.position.z);


        while (Vector3.Scale(ladderPosition - _transform.position, new Vector3(1, 0, 1)).sqrMagnitude > _ladderPaddingDistance*_ladderPaddingDistance)
        {
            Debug.Log("move to ladder");
            Vector3 direction = ladderPosition - _transform.position;
            _navMeshAgentController.RigidBody.velocity = direction.normalized;
            yield return null;
        }

        _navMeshAgentController.RigidBody.velocity = Vector3.zero;
        //_navMeshAgentController.RigidBody.isKinematic = true;
        ClimbLadder();

    }

    private void ClimbLadder()
    {
        _animationsController.ApplyRootMotion(true);
        _animationsController.Climb(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && !_triggers.Contains(other))
            _triggers.Add(other);

        Debug.Log("enter "+other.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_triggers.Contains(other))
            _triggers.Remove(other);
        Debug.Log("exit "+other.gameObject.name);
        Debug.Log("is grounded: "+_navMeshAgentController.IsGrounded());
        if (IsInteracting && _health>0 && other.gameObject.CompareTag("Ladder") && !_navMeshAgentController.IsGrounded())
        {
            _transform.gameObject.layer = LayerMask.NameToLayer("NoCollisions");

            _animationsController.ClimbTopLadder();
        }
    }

    public void RemoveTriggersFromList(Collider[] colliders)
    {
        for (int i = colliders.Length - 1; i >= 0; i--)
        {
            if (colliders[i].isTrigger)
            {
                if (_triggers.Contains(colliders[i]))
                    _triggers.Remove(colliders[i]);
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_object != null  && _navMeshAgentController.IsGrounded())
        {
            if(_object.CompareTag("Jump") || _object.CompareTag("Fall"))
            {
                _navMeshAgentController.UpdateTransformToNavmesh = true;
                _navMeshAgentController.Warp(_transform.position);

                _navMeshAgentController.RigidBody.useGravity = false;
                _navMeshAgentController.RigidBody.isKinematic = true;

                IsInteracting = false;
                Debug.Log("end jump");
            }

        }
    }

    private GameObject GetClosestTriggerObject()
    {
        Vector3 position = _playerTransform.position;
        float distance = 100;
        GameObject closest = null;
        foreach (Collider col in _triggers)
        {
            float tempDistance = Vector3.Magnitude(position - col.transform.position);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                closest = col.gameObject;
            }

        }
        return closest;
    }
}
