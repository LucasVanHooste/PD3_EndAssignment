using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentController : MonoBehaviour {

    private NavMeshAgent _navMeshAgent;
    private Transform _transform;
    public Transform PlayerTransform;
    [SerializeField] private float _walkingSpeedMultiplier;
    private float _normalSpeed;
    private float _walkingSpeed;
    [SerializeField] private float _idleRotationSpeed= 360;

    private float _previousNotZeroAngle = 0;
    private Vector3 _previousForward;
    private Vector3 _previousPlayerPosition;

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

    private bool _updateTransformToNavmesh=true;
    public bool UpdateTransformToNavmesh
    {
        get
        {
            return _updateTransformToNavmesh;
        }
        set
        {
            _navMeshAgent.updatePosition = value;
            _navMeshAgent.updateRotation = value;
            _updateTransformToNavmesh = value;
        }
    }
    // Use this for initialization
    void Start () {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _transform = transform;

        _normalSpeed = _navMeshAgent.speed;
        _walkingSpeed = _normalSpeed * _walkingSpeedMultiplier;

        _previousForward = _transform.forward;
        _previousPlayerPosition = PlayerTransform.position;
    }

    // Update is called once per frame
    void Update () {
        _relativeVelocity = GetScaledRelativeVelocity();
        _rotationSpeed = GetScaledRotationSpeed();
    }

    public void Walk()
    {
        _navMeshAgent.speed = _walkingSpeed;
    }
    public void Run()
    {
        _navMeshAgent.speed = _normalSpeed;
    }

    private Vector3 GetScaledRelativeVelocity()
    {
        if (!_navMeshAgent.updatePosition || _navMeshAgent.isStopped)
            return Vector3.zero;

        Vector3 relativeVelocity = _transform.InverseTransformVector(_navMeshAgent.velocity);
        //Debug.Log("relative velocity: " + relativeVelocity * (_navMeshAgent.speed / _normalSpeed) / 2);
        //Debug.Log("speed: " + _navMeshAgent.speed);
        return (relativeVelocity * (_navMeshAgent.speed / _normalSpeed)) / 2;
    }

    public void RotateToPlayer()
    {
        //Vector3 newDir = Vector3.RotateTowards(_transform.forward, Vector3.Scale(_playerTransform.position - _transform.position, new Vector3(1, 0, 1)), .1f, 0.0f);
        //    _transform.rotation = Quaternion.LookRotation(newDir);
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.LookRotation(Vector3.Scale(PlayerTransform.position - _transform.position, new Vector3(1, 0, 1))), Time.deltaTime * _idleRotationSpeed);

    }



    private float GetScaledRotationSpeed()
    {
        float angle = Vector3.SignedAngle(_previousForward, _transform.forward, Vector3.up);

        if (!UpdateTransformToNavmesh)
        {
            if (angle == 0)
            {
                if (_previousPlayerPosition != PlayerTransform.position)
                    angle = _previousNotZeroAngle;
            }
            else
                _previousNotZeroAngle = angle;
        }

        _previousPlayerPosition = PlayerTransform.position;
        _previousForward = _transform.forward;
        //Debug.Log("angle: " + angle);
        return angle;
    }

    public bool HasNavMeshReachedDestination()
    {
        if (!_navMeshAgent.pathPending)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude <= 0f)
                {
                    Debug.Log("reached destination");
                    return true;
                }
            }
        }
        return false;
    }

    public void Warp(Vector3 position)
    {
        _navMeshAgent.Warp(position);
    }

    public void SetDestination(Vector3 position)
    {
        _navMeshAgent.SetDestination(position);
    }

    public bool IsOnOffMeshLink()
    {
        return _navMeshAgent.isOnOffMeshLink;
    }

    public Vector3 RandomNavSphere(Vector3 origin, float range, int layermask)
    {
        Vector3 randomPosition = UnityEngine.Random.insideUnitSphere * range;

        randomPosition += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomPosition, out navHit, range, layermask);
        Debug.Log("navpos: " + navHit.position);
        return navHit.position;
    }
}
