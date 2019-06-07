using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CameraController))]

public class PlayerController : MonoBehaviour {

    private Transform _transform;
    private PlayerMotor _playerMotor;
    private Animator _animator;
    private PlayerController _playerController;
    private IState _state;
    private AnimationsController _animationsController;
    private CameraController _cameraController;

    private Vector3 _startPosition;
    private Quaternion _startRotation;


    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;
    public Transform RightHand
    {
        get
        {
            return _rightHand;
        }
    }
    [SerializeField ]private Transform _camRoot;
    public Transform CameraRoot
    {
        get
        {
            return _camRoot;
        }
    }
    [SerializeField] private Transform _obstacleIKLeftHand;
    [SerializeField] private Transform _obstacleIKRightHand;

    [SerializeField] private Transform _lookAtTransform;
    [SerializeField] private Transform _holsterGun1Hand;
    [SerializeField] private Transform _holsterGun2Hands;

    [Space]
    [Header("Combat Parameters")]
    [SerializeField] private int _maxHealth;
    private int _health;
    public int Health {
        get {
            return _health;
        }
    }

    [SerializeField] private float _punchCoolDown;
    public float PunchCoolDown
    {
        get
        {
            return _punchCoolDown;
        }
    }

    [SerializeField] private int _punchDamage;
    public int PunchDamage
    {
        get
        {
            return _punchDamage;
        }
    }

    [SerializeField] private float _punchRange;
    public float PunchRange
    {
        get
        {
            return _punchRange;
        }
    }

    private List<Collider> _triggers = new List<Collider>();
    public List<Collider> Triggers
    {
        get
        {
            return _triggers;
        }
        set
        {
            _triggers = value;
        }
    }

    [Space]
    [Header("Other")]
    [SerializeField] private GameObject _crossHair;
    [SerializeField] private Slider _healthbar;
    [SerializeField] private CinematicBehaviour _cinematicBehaviour;

    // Use this for initialization
    void Start () {
        _health = _maxHealth;
        _transform = transform;
        _playerMotor = GetComponent<PlayerMotor>();
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        _animationsController = new AnimationsController(_animator);

        _animationsController.HoldGunIK.Player=_transform;
        _animationsController.LookAtIK.LookAtPosition=_lookAtTransform;
        _animationsController.ClimbBottomLadderIK.LeftHand = _leftHand;
        _animationsController.ClimbBottomLadderIK.RightHand = RightHand;

        _cameraController = GetComponent<CameraController>();
        _startPosition= _transform.position;
        _startRotation = _transform.rotation;

        _state = new NormalState(_transform, _playerMotor, _playerController, _animationsController);
        SetHealthBar();
    }
	
	// Update is called once per frame
	void Update () {
        _state.Update();

        UpdateAnimations();
	}

    private void UpdateAnimations()
    {
        _animationsController.SetHorizontalMovement(_playerMotor.Movement);
        _animationsController.SetRotationSpeed(_playerMotor.Aim.x);

        _animationsController.SetIsGrounded(_playerMotor.IsGrounded());
        _animationsController.SetDistanceFromGround(_playerMotor.GetDistanceFromGround());
        _animationsController.SetVerticalVelocity(_playerMotor.GetVelocity().y);
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _state.OnControllerColliderHit(hit);
    }

    public void ToNormalState()
    {
        _state = new NormalState(_transform, _playerMotor,_playerController, _animationsController);
    }
    public void ToPushingState(GameObject _obstacle)
    {
        _state = new PushingState(_transform, _playerMotor, _playerController, _animationsController, _obstacle, _obstacleIKLeftHand, _obstacleIKRightHand);
    }
    public void ToCinematicState(GameObject _object)
    {
        _state = new CinematicState(_transform, _playerMotor, _playerController, _animationsController, _object, _cinematicBehaviour);
    }
    public void ToGunState(GameObject _gun)
    {
        _state = new GunState(_transform, _playerMotor, _playerController, _animationsController, _gun, _cameraController, _holsterGun1Hand, _holsterGun2Hands, _crossHair);
    }
    public void ToDeadState()
    {
        _state = new DeadState(_playerMotor, _playerController);
    }

    public void ToClimbingState(GameObject _ladder)
    {
        _state = new ClimbingState(_transform, _playerMotor, _playerController, _animationsController, _ladder);
    }

    public void ToTurretState(GameObject _turret)
    {
        _state = new TurretState(_transform, _playerMotor, _playerController, _animationsController, _turret, _cameraController, _crossHair);
    }

    public void PickUpGun()
    {
        _state.PickUpGun();
    }

    private void TakeDamage(int damage, Vector3 originOfDamage)
    {
        _health -= damage;
        _animationsController.TakeDamage();
        SetHealthBar();

        Die(originOfDamage);
    }

    public void TakePunch(int damage, Vector3 originOfDamage)
    {
        if (_health <= 0) return;
        TakeDamage(damage, originOfDamage);       
    }

    public void GetShot(int damage, Vector3 originOfDamage)
    {
        if (_health <= 0) return;
        TakeDamage(damage, originOfDamage);
    }

    private void Die(Vector3 originOfDamage)
    {
        if (_health > 0) return;

        Vector3 transformedOrigin = GetTransformedOrigin(originOfDamage);

        _animationsController.Die(transformedOrigin.x, transformedOrigin.z);

        _state.Die();
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

    private void SetHealthBar()
    {
        _healthbar.value = (float)_health/ _maxHealth;
    }

    public void Respawn()
    {
        _transform.position = _startPosition;
        _transform.rotation = _startRotation;
        _animationsController.ResetAnimations();
        _health = _maxHealth;
        _playerController.ToNormalState();
        SetHealthBar();
    }
}
