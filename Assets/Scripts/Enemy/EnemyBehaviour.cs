using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyPhysicsController))]
public class EnemyBehaviour : MonoBehaviour
{

    private INode _behaviourTree;
    private Coroutine _treeCoroutine;
    private IEnemyMovementAction _enemyMovementAction;
    private Transform _transform;
    private EnemyPhysicsController _navMeshAgentController;
    private Animator _animator;
    private AnimationsController _animationsController;
    private Transform _playerTransform;
    private PlayerController _playerController;

    [Header("Combat Parameters")]
    [SerializeField] private int _maxHealth;
    private int _health;
    public int Health
    {
        get
        {
            return _health;
        }
    }
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

    [Space]
    [Header("Roaming Parameters")]
    [SerializeField] private Vector2 _roamingTimeRange;
    private float _roamingTime;
    private float _roamingTimer = 0;
    private bool _hasBeenAttacked = false;

    [Space]
    [Header("Gun Parameters")]
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
    [SerializeField] private Vector3 _eyesPosition;
    [SerializeField] private float _maxDistancefromGun;
    private GameObject _targetGun;

    [Space]
    [Header("Other")]
    [SerializeField] private RangeTriggerCheckerScript _rangeTriggerChecker;

    private List<Collider> _triggers = new List<Collider>();
    public bool IsInMovementAction {
        get
        {
            return _enemyMovementAction != null ? true : false;
        }
    }

    void Start()
    {
        _health = _maxHealth;
        _transform = transform;
        _navMeshAgentController = GetComponent<EnemyPhysicsController>();

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

        _behaviourTree = new SequenceNode(
            new ConditionNode(IsNotInMovementAction),
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
                                new ActionNode(SetPlayerPositionAsTarget))))),
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

        _treeCoroutine= StartCoroutine(RunTree());
    }

    // Update is called once per frame
    void Update()
    {
        if (_health <= 0) return;

        if (_navMeshAgentController.RigidBody.velocity.y < -7f)
            DropGun();

        if (_gun != null)
        {
            FireGun(_fireGun);
            AimGun(_aimGun);
        }
        _fireGun = false;
        _aimGun = false;

        if (!IsInMovementAction && _navMeshAgentController.IsOnOffMeshLink() && _triggers.Count > 0)
        {
             GameObject _object = GetClosestTriggerObject();

            switch (_object.tag)
            {
                case "Ladder":
                    {
                        _enemyMovementAction = new EnemyLadderAction(_animationsController, _navMeshAgentController, this, _object.transform);
                    }break;
                case "Jump":
                    {
                        _enemyMovementAction = new EnemyJumpAction(_navMeshAgentController,this, _object.transform);
                    }break;
                case "Fall":
                    {
                        _enemyMovementAction = new EnemyFallAction(_navMeshAgentController,this, _object.transform);
                    }
                    break;
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

    #region BehaviourTree Actions

    private IEnumerator<NodeResult> Roam()
    {
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
        if (!_navMeshAgentController.UpdateTransformToNavmesh)
        {
            _navMeshAgentController.Warp(_transform.position);
            _navMeshAgentController.UpdateTransformToNavmesh = true;
            _navMeshAgentController.SetDestination(_navMeshAgentController.RandomNavSphere(_playerTransform.position, 4, -1));
        }

        _navMeshAgentController.Run();
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
            }

            _navMeshAgentController.SetDestination(_playerTransform.position);
        }

        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> PunchPlayer()
    {
        if (_punchCoolDownTimer > _punchCoolDown)
        {
            if (_playerController.Health > 0)
            {
                _playerController.TakePunch(_punchDamage, _transform.position);
                _punchCoolDownTimer = 0;
                _animationsController.Punch();
            }
        }
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

        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> RunForGun()
    {
        //pick up gun
        if (_triggers.Contains(_targetGun.GetComponent<Collider>()))
        {
            PickUpGun(_targetGun.transform);
            RemoveTriggersFromList(_targetGun.GetComponents<Collider>());
            _rangeTriggerChecker.RemoveTriggersFromList(_targetGun.GetComponents<Collider>());
            _targetGun = null;
        }
        else
        {
            _navMeshAgentController.Run();
            _navMeshAgentController.SetDestination(_targetGun.transform.position);
        }
        yield return NodeResult.Succes;
    }

    #endregion

    #region BehaviourTree Conditions

    private bool IsNotInMovementAction()
    {
        return !IsInMovementAction;
    }

    private bool SeesPlayer()
    {
        Vector3 directionPlayer = _playerTransform.position - _transform.position;
        if (Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionPlayer)) < FOV / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(_transform.position + _eyesPosition, directionPlayer, out hit, 100, _canSeePlayerLayerMask))
            {
                if (hit.transform.gameObject.layer == 9)
                {
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
                return true;
            }
        }
        return false;
    }

    private bool SeesGun()
    {
        _targetGun = null;

        if (GetGunInHolster() != null || _gun!=null) return false;

        //get all guns that are closeby
        List<GameObject> _targetGuns = _rangeTriggerChecker.GetTriggerObjectsWithTag("Gun");
        if (_targetGuns.Count <= 0) return false;

        //get the closest gun the player can see
        float distance = 100;
        foreach (GameObject gun in _targetGuns)
        {
            RaycastHit hit;
            if (Physics.Raycast(_transform.position+_eyesPosition, gun.transform.position - (_transform.position+ _eyesPosition), out hit, 100, _canSeePlayerLayerMask))
            {
                if (hit.collider.CompareTag("Gun"))
                {
                    if (gun.transform.parent != null)
                    {
                        _rangeTriggerChecker.RemoveTriggersFromList(gun.GetComponents<Collider>());
                        _roamingTimer = _roamingTime;
                    }
                    else
                    {
                        float tempDistance = Vector3.SqrMagnitude(gun.transform.position - _transform.position);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            _targetGun = gun.gameObject;
                        }
                    }

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

    #endregion

    #region Gun Interaction

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
        //decrease chance of hitting if player is further away
        float offset = Mathf.Clamp((_playerTransform.position - _transform.position).sqrMagnitude, 1, _missingShotRange);
        Vector3 randomPosition = UnityEngine.Random.insideUnitSphere * offset;

        _gunScript.EnemyFireGun(fire, _anchorPoint.position, (_playerTransform.position+randomPosition)-_transform.position);
    }
    public void HolsterGun()
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

    #endregion

    private void TakeDamage(int damage, Vector3 originOfDamage)
    {
        _health -= damage;
        _animationsController.TakeDamage();

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

        if(IsInMovementAction)
        _enemyMovementAction.Stop();

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
        _navMeshAgentController.Die();
        gameObject.layer = LayerMask.NameToLayer("NoCollisionWithPlayer");
        StopCoroutine(_treeCoroutine);
        GameObject.Destroy(this);
    }

    public void StopMovementAction()
    {
        if (IsInMovementAction)
        {
            _enemyMovementAction.Stop();
            _enemyMovementAction = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && !_triggers.Contains(other))
            _triggers.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_triggers.Contains(other))
            _triggers.Remove(other);

        if (IsInMovementAction)
            _enemyMovementAction.OnTriggerExit(other);
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
        if (IsInMovementAction)
            _enemyMovementAction.OnCollisionEnter(collision);
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
