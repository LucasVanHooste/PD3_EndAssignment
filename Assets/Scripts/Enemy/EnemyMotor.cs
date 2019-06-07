using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyMotor : MonoBehaviour {

    private NavMeshAgent _navMeshAgent;
    public Rigidbody RigidBody { get; set; }
    private Transform _transform;
    public Transform PlayerTransform;

    [Space]
    [Header("Movement Parameters")]
    [SerializeField] private float _walkingSpeedMultiplier;
    private float _normalSpeed;
    private float _walkingSpeed;
    [SerializeField] private float _idleRotationSpeed= 360;

    private float _previousNotZeroAngle = 0;
    private Vector3 _previousForward;
    private Vector3 _previousPlayerPosition;

    [SerializeField] private LayerMask _mapLayerMask;
    private float _distanceFromGround = 0;
    public float DistanceFromGround
    {
        get
        {
            return _distanceFromGround;
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

    [SerializeField] private Vector3 _jumpForce;
    [SerializeField] private Vector3 _fallForce;

    [SerializeField] private IsGroundedCheckerScript _isGroundedChecker;
    private RigidbodyConstraints _constraints;

    // Use this for initialization
    void Start () {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        RigidBody = GetComponent<Rigidbody>();
        _constraints = RigidBody.constraints;
        _transform = transform;

        _normalSpeed = _navMeshAgent.speed;
        _walkingSpeed = _normalSpeed * _walkingSpeedMultiplier;

        _previousForward = _transform.forward;
        _previousPlayerPosition = PlayerTransform.position;
    }

    // Update is called once per frame
    void Update () {
        _distanceFromGround = GetDistanceFromGround();
        _relativeVelocity = GetScaledRelativeVelocity();
        _rotationSpeed = GetScaledRotationSpeed();
    }

    private float GetDistanceFromGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(_transform.position + new Vector3(0, 1f, 0), Vector3.down, out hit, 100, _mapLayerMask))
        {
            return (hit.point - _transform.position).magnitude;
        }
        return 100;
    }

    public bool IsGrounded()
    {
        return _isGroundedChecker.IsGrounded;
    }

    public void Jump(Transform offMeshLink)
    {
        UpdateTransformToNavmesh = false;
        
        RigidBody.useGravity = true;
        RigidBody.isKinematic = false;
        if (Vector3.Angle(_transform.forward, offMeshLink.forward) > 145)
            RigidBody.AddForce(_transform.TransformVector(_jumpForce), ForceMode.Impulse);
        else
            RigidBody.AddForce((Vector3.Scale(offMeshLink.position - _transform.position, new Vector3(1, 0, 1)).normalized * _jumpForce.z) + new Vector3(0, _jumpForce.y, 0), ForceMode.Impulse);
    }

    public void Fall(Transform offMeshLink)
    {
        UpdateTransformToNavmesh = false;

        RigidBody.useGravity = true;
        RigidBody.isKinematic = false;

        RigidBody.AddForce((Vector3.Scale(offMeshLink.position - _transform.position, new Vector3(1, 0, 1)).normalized * _fallForce.z) + new Vector3(0, _fallForce.y, 0), ForceMode.Impulse);
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
        if (RigidBody.isKinematic)
        {
            if (!_navMeshAgent.updatePosition || _navMeshAgent.isStopped)
                return Vector3.zero;

            Vector3 relativeVelocity = _transform.InverseTransformVector(_navMeshAgent.velocity);
            return (relativeVelocity * (_navMeshAgent.speed / _normalSpeed)) / 2;
        }
        else
        {
            return _transform.InverseTransformVector(RigidBody.velocity);
        }

    }

    public void RotateToPlayer()
    {
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

    public bool IsOnNavMesh()
    {
        return _navMeshAgent.isOnNavMesh;
    }

    public void Stop(bool stop)
    {
        _navMeshAgent.isStopped = stop;
    }

    public void Die()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.enabled = false;
    }

    public void ResetRigidbodyConstraints()
    {
        RigidBody.constraints = _constraints;
    }

    public Vector3 RandomNavSphere(Vector3 origin, float range, int layermask)
    {
        Vector3 randomPosition = UnityEngine.Random.insideUnitSphere * range;

        randomPosition += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomPosition, out navHit, range, layermask);

        return navHit.position;
    }
}
