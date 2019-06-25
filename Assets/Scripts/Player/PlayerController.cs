using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CameraController))]

public class PlayerController : MonoBehaviour, IDamageable {

    public static Transform PlayerTransform { get; private set; }

    private Transform _transform;
    private PlayerMotor _playerMotor;
    private Animator _animator;
    private IState _state;
    private PlayerStateManager _playerStateManager;
    private AnimationsController _animationsController;
    public CameraController cameraController { get; private set; }

    private Vector3 _startPosition;
    private Quaternion _startRotation;


    [SerializeField] private Transform _leftHand = null;
    [SerializeField] private Transform _rightHand = null;
    public Transform RightHand { get => _rightHand; }

    [SerializeField] private Transform _gunAnchor = null;
    public Transform GunAnchor { get => _gunAnchor; }

    [SerializeField] private Transform _obstacleIKLeftHand = null;
    public Transform ObstacleIKLeftHand { get => _obstacleIKLeftHand; }
    [SerializeField] private Transform _obstacleIKRightHand = null;
    public Transform ObstacleIKRightHand { get => _obstacleIKRightHand; }

    [SerializeField] private Transform _lookAtTransform = null;
    [SerializeField] private Transform _holsterGun1Hand = null;
    public Transform HolsterGun1Hand { get => _holsterGun1Hand; }
    [SerializeField] private Transform _holsterGun2Hands = null;
    public Transform HolsterGun2Hands { get => _holsterGun2Hands; }

    [Space]
    [Header("Combat Parameters")]
    [SerializeField] private int _maxHealth = 0;
    public int Health { get; private set; }

    [SerializeField] private float _punchCoolDown = 0;
    public float PunchCoolDown { get => _punchCoolDown; }

    [SerializeField] private int _punchDamage;
    public int PunchDamage { get => _punchDamage; }

    [SerializeField] private float _punchRange;
    public float PunchRange { get => _punchRange; }

    [SerializeField] private LayerMask _bulletLayerMask;
    public LayerMask BulletLayerMask { get => _bulletLayerMask; }

    [Space]
    [Header("Other")]
    [SerializeField] private GameObject _crossHair=null;
    public GameObject CrossHair { get => _crossHair; }
    [SerializeField] private Slider _healthbar=null;

    public List<Collider> Triggers { get; private set; } = new List<Collider>();

    private void Awake()
    {
        _transform = transform;
        PlayerTransform = _transform;
    }

    void Start () {
        Health = _maxHealth;
        _playerMotor = GetComponent<PlayerMotor>();
        _animator = GetComponent<Animator>();
        _animationsController = new AnimationsController(_animator);

        SetUpAnimationIK();

        cameraController = GetComponent<CameraController>();
        _startPosition= _transform.position;
        _startRotation = _transform.rotation;

        _playerStateManager = new PlayerStateManager(_playerMotor, this, _animationsController);
        SwitchState<NormalState>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetHealthBar();
    }

    private void SetUpAnimationIK()
    {
        _animationsController.HoldGunIK.Player = _transform;
        _animationsController.LookAtIK.LookAtPosition = _lookAtTransform;
        _animationsController.ClimbBottomLadderIK.LeftHand = _leftHand;
        _animationsController.ClimbBottomLadderIK.RightHand = RightHand;
    }

    public void SwitchState<T>(IInteractable interactableObject = null) where T : IState
    {
        if(_state!=null)
        _state.OnStateExit();

        _state = _playerStateManager.GetState<T>(interactableObject);
        _state.OnStateEnter();
    }

    void Update () {
        _state.Update();

        UpdateAnimations();
	}

    private void UpdateAnimations()
    {
        _animationsController.SetHorizontalMovement(_playerMotor.Movement);
        _animationsController.SetRotationSpeed(_playerMotor.Aim.x);

        _animationsController.SetIsGrounded(_playerMotor.IsGrounded);
        _animationsController.SetDistanceFromGround(_playerMotor.GetDistanceFromGround());
        _animationsController.SetVerticalVelocity(_playerMotor.Velocity.y);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && !Triggers.Contains(other))
            Triggers.Add(other);

        _state.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (Triggers.Contains(other))
            Triggers.Remove(other);

        _state.OnTriggerExit(other);
    }

    public void RemoveTriggersFromList(Collider[] colliders)
    {
        for (int i = colliders.Length - 1; i >= 0; i--)
        {
            if (colliders[i].isTrigger)
            {
                 Triggers.Remove(colliders[i]);
            }

        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _state.OnControllerColliderHit(hit);
    }


    public GameObject GetClosestTriggerObject()
    {
        Vector3 position = _transform.position;
        float distance = Mathf.Infinity;
        GameObject closest = null;
        foreach (Collider col in Triggers)
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

    public void TakeDamage(int damage, Vector3 originOfDamage)
    {
        Health -= damage;
        _animationsController.TakeDamage();
        SetHealthBar();

        if (Health <= 0)
            DieFromHit(originOfDamage);
    }

    private void Die()
    {
        _state.Die();
        SwitchState<DeadState>();
    }

    private void DieFromHit(Vector3 originOfDamage)
    {
        Vector3 transformedOrigin = _transform.InverseTransformPoint(originOfDamage);
        //added this because the directional death animations didn't blend well
        transformedOrigin = transformedOrigin.TransformToHorizontalAxisVector();

        _animationsController.Die(transformedOrigin.x, transformedOrigin.z);

        Die();
    }

    private void DieFromFalling()
    {
        Die();
    }

    public void Respawn()
    {
        cameraController.ResetCameraAnchorAndPositions();
        _transform.SetPositionAndRotation(_startPosition, _startRotation);
        _animationsController.ResetAnimations();
        Health = _maxHealth;
        SwitchState<NormalState>();
        SetHealthBar();
    }

    private void SetHealthBar()
    {
        _healthbar.value = (float)Health/ _maxHealth;
    }
}
