using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class MeleeEnemyBehaviour : MonoBehaviour {

    private INode _rootNode;
    private Transform _transform;
    private MeleeEnemyBehaviour _meleeEnemyBehaviour;
    private Animator _animator;
    private EnemyAnimationsController _animationsController;
    private NavMeshAgent _navMeshAgent;

    private PlayerController _playerController;

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private LayerMask _mapLayerMask;
    [SerializeField] private float FOV;
    [SerializeField] private LayerMask _canSeePlayerLayerMask;
    [SerializeField] private float _minDistanceFromPlayer;
    [SerializeField] private float _punchReach;
    [SerializeField] private float _punchCoolDown;
    [SerializeField] private int _punchDamage;
    private float _punchCoolDownTimer = 0;

    [SerializeField] private int _maxHealth;
    private int _health;
    public int Health
    {
        get
        {
            return _health;
        }
    }

    private Vector3 _relativeVelocity;
    public Vector3 RelativeVelocity
    {
        get
        {
            return _relativeVelocity;
        }
    }

    private float _rotationSpeed;
    public float RotationSpeed
    {
        get
        {
            return _rotationSpeed;
        }
    }

    private Vector3 _targetPosition;
    private Quaternion _previousRotation;

    void Start()
    {
        _health = _maxHealth;
        _transform = GetComponent<Transform>();
        _targetPosition = _transform.position;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _meleeEnemyBehaviour = GetComponent<MeleeEnemyBehaviour>();
        _animator = GetComponent<Animator>();
        _animationsController = new EnemyAnimationsController(_transform, _meleeEnemyBehaviour, _animator, _navMeshAgent);
        _playerController = _playerTransform.GetComponent<PlayerController>();
        _previousRotation = _transform.rotation;
        //#region test

        //INode idk = new SelectorNode(new ConditionNode(IsHungry), new ConditionNode(IsHungry), new ConditionNode(IsHungry));
        //INode selectorNode = new SelectorNode(idk);
        //#endregion

        //ConditionNode.Condition hungryCondition = IsHungry;

        //ConditionNode IsHungrCondition = new ConditionNode(hungryCondition);


        INode punchPlayerAI = new SequenceNode(new ConditionNode(CanPunchPlayer), new ActionNode(PunchPlayer));
        INode followPlayerAI = new SequenceNode(new ConditionNode(SeesPlayer), new SequenceNode(new ActionNode(ChargeAtPlayer), punchPlayerAI));
        //INode sleepyyAI = new SequenceNode(new ConditionNode(IsCloseToPlayer), new ActionNode(Sleep));

        //INode SelectorRootNode = new SelectorNode(hungryAI, sleepyyAI, new ActionNode(Roam));

        //INode ParallelRootNode = new ParallelNode(ParallelAwaysSuccesPolicy, SelectorRootNode, new ActionNode(Blaah));
        //_rootNode = new ParallelNode(NSuccesIsSuccesAccumulator.Factory, SelectorRootNode, new ActionNode(Blaah));

        _rootNode = new ParallelNode(NSuccesIsSuccesAccumulator.Factory, followPlayerAI);
        //_rootNode = new SelectorNode(followPlayerAI);

        StartCoroutine(RunTree());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("rigidbody: " + _rigidBody.velocity);
        //Debug.Log("navmesh: " + _navMeshAgent.velocity);

        //Debug.Log("navmesh desired: " + _navMeshAgent.desiredVelocity);
        //Debug.Log("navmesh speed: " + _navMeshAgent.speed);


        _relativeVelocity = GetScaledRelativeVelocity();
        _rotationSpeed = GetScaledRotationSpeed();
        _animationsController.Update();
    }

    private IEnumerator<NodeResult> Roam()
    {
        Debug.Log("Roaming");
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> Blaah()
    {
        Debug.Log("Blaaaaah");
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> Sleep()
    {

        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Sleeping: " + i);
            yield return NodeResult.Running;
        }

        Debug.Log("Done Sleeping");
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> ChargeAtPlayer()
    {
        if (Vector3.Magnitude(_playerTransform.position - _transform.position) > _minDistanceFromPlayer)
            _navMeshAgent.SetDestination(_playerTransform.position);
        else _navMeshAgent.SetDestination(_transform.position);

        Debug.Log("Charging");
        yield return NodeResult.Succes;
    }

    private IEnumerator<NodeResult> PunchPlayer()
    {
        if(_playerController.Health>0)
        _playerController.TakePunch(_punchDamage);
        Debug.Log("TakePunch");
        yield return NodeResult.Succes;
    }

    IEnumerator RunTree()
    {
        while (_health>0)
        {
            yield return _rootNode.Tick();
        }
    }

    bool SeesPlayer()
    {
        Vector3 directionPlayer = _playerTransform.position - _transform.position;
        if (Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionPlayer)) < FOV / 2)
        {
            //Debug.Log("angle: " + Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionPlayer)));
            RaycastHit hit;
            if(Physics.Raycast(_transform.position+new Vector3(0,1.6f,0),directionPlayer, out hit, 1000, _canSeePlayerLayerMask))
            {
                //Debug.Log(hit.transform.name);
                if (hit.transform.gameObject.layer == 9)
                {
                    Debug.Log("I see player");
                    return true;
                }
            }
        }

        return false;
    }

    bool CanPunchPlayer()
    {
        if (Vector3.Magnitude(_playerTransform.position - _transform.position) <= _punchReach)
        {
            if (_punchCoolDownTimer > _punchCoolDown)
            {
                Debug.Log("punch");
                _punchCoolDownTimer = 0;
                return true;
            }
            _punchCoolDownTimer += Time.deltaTime;
        }
        else
        _punchCoolDownTimer = 0;
        return false;
    }



    private Vector3 GetScaledRelativeVelocity()
    { 
        Vector3 relativeVelocity = _transform.InverseTransformVector(_navMeshAgent.velocity);
        return relativeVelocity / _navMeshAgent.speed;
    }

    private float GetScaledRotationSpeed()
    {
        float angle = Quaternion.Angle(_previousRotation, _transform.rotation);
        _previousRotation = _transform.rotation;
        //Debug.Log("angle: " + (angle / Time.deltaTime) / _navMeshAgent.angularSpeed);
        return (angle/Time.deltaTime)/_navMeshAgent.angularSpeed;
    }

    private void TakeDamage(int damage)
    {
        _health -= damage;
        //_animationsController.SetHealth(_health);
    }

    public void TakePunch(int damage)
    {
        TakeDamage(damage);
        //_animationsController.TakePunch();

        Die();
    }

    public void GetShot(int damage)
    {
        TakeDamage(damage);

        Die();
    }

    private void Die()
    {
        if (_health > 0) return;
        GameObject.Destroy(gameObject);
        //_state.Die();
        //ToDeadState();
    }

    public bool IsOnOffMeshLink()
    {
        return _navMeshAgent.isOnOffMeshLink;
    }

    public float GetDistanceFromGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(_transform.position+ new Vector3(0,1f,0), Vector3.down, out hit, 1000, _mapLayerMask))
        {
            //print("I'm looking at " + hit.transform.name);
            return (hit.point - _transform.position).magnitude;
        }
        //print("I'm looking at nothing!");
        return 1000;
    }

    public bool IsGrounded()
    {
        if (GetDistanceFromGround() > .1f)
            return false;

                return true;
    }

    public void ToDeadState()
    {
        //die
    }
}
