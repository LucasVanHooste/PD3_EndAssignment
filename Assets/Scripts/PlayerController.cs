using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CameraController))]

public class PlayerController : MonoBehaviour {

    [SerializeField] private CinematicBehaviour _cinematicBehaviour;
    public Transform LeftHand;
    public Transform RightHand;
    public Transform CameraRoot;
    [SerializeField] private Transform _obstacleIKLeftHand;
    [SerializeField] private Transform _obstacleIKRightHand;

    [SerializeField] private Transform _lookAtTransform;
    [SerializeField] private Transform _holsterGun1Hand;
    [SerializeField] private Transform _holsterGun2Hands;
    [SerializeField] private GameObject _crossHair;

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

    private Transform _transform;
    private PhysicsController _physicsController;
    private Animator _animator;
    private PlayerController _playerController;
    private CharacterController _characterController;
    private IState _state;
    private AnimationsController _animationsController;
    private CameraController _cameraController;

    //private GameObject _gun;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    [HideInInspector] public List<Collider> Triggers = new List<Collider>();

    // Use this for initialization
    void Start () {
        _health = _maxHealth;
        _transform = transform;
        _physicsController = GetComponent<PhysicsController>();
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        _animationsController = new AnimationsController(_animator, _physicsController);

        _animationsController.HoldGunIK.SetPlayer(_transform);
        //_animationsController.PickUpGunIK.SetPlayer(_transform);
        _animationsController.LookAtIK.SetLookAtPosition(_lookAtTransform);
        //_animationsController.ClimbTopLadderAnimationBehaviour.SetBehaviour(_playerController, _physicsController, _animationsController);

        _cameraController = GetComponent<CameraController>();
        _startPosition= _transform.position;
        _startRotation = _transform.rotation;

        _state = new NormalState(_transform, _physicsController, _playerController, _animationsController);
    }
	
	// Update is called once per frame
	void Update () {
        _state.Update();
        _animationsController.Update();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && !Triggers.Contains(other))
            Triggers.Add(other);

        _state.OnTriggerEnter(other);
        Debug.Log("enter");
    }

    private void OnTriggerExit(Collider other)
    {
        if (Triggers.Contains(other))
            Triggers.Remove(other);

        _state.OnTriggerExit(other);
        Debug.Log("exit");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _state.OnControllerColliderHit(hit);
    }

    public void ToNormalState()
    {
        _state = new NormalState(_transform, _physicsController,_playerController, _animationsController);
        Debug.Log("ToNormalState");
    }
    public void ToPushingState(GameObject _obstacle)
    {
        _state = new PushingState(_transform, _physicsController, _playerController, _animationsController, _obstacle, _obstacleIKLeftHand, _obstacleIKRightHand);
        Debug.Log("ToPushingState");
    }
    public void ToCinematicState(GameObject _object)
    {
        _state = new CinematicState(_transform, _physicsController, _playerController, _animationsController, _object, _cinematicBehaviour);
        //_gun = _object;
        Debug.Log("ToCinematicState");
    }
    public void ToGunState(GameObject _gun)
    {
        _state = new GunState(_transform, _physicsController, _playerController, _animationsController, _gun, _cameraController, _holsterGun1Hand, _holsterGun2Hands, _crossHair);
        Debug.Log("ToGunState");
    }
    public void ToDeadState()
    {
        _state = new DeadState(_transform, _physicsController, _playerController, _animationsController);
        Debug.Log("ToDeadState");
    }

    public void ToClimbingState(GameObject _ladder)
    {
        _state = new ClimbingState(_transform, _physicsController, _playerController, _animationsController, _ladder);
        Debug.Log("ToClimbingState");
    }

    public void PickUpGun()
    {
        _state.PickUpGun();
    }

    //public void DropGun()
    //{
    //    Debug.Log("drop gun");
    //    _state.DropGun();
    //}

    private void TakeDamage(int damage)
    {
        _health -= damage;
        _animationsController.SetHealth(_health);
    }

    public void TakePunch(int damage)
    {
        TakeDamage(damage);       
        _animationsController.TakePunch();

        Die();
    }

    private void Die()
    {
        if (_health > 0) return;

        _state.Die();
        ToDeadState();
    }

    public void Respawn()
    {
        _transform.position = _startPosition;
        _transform.rotation = _startRotation;
        _animationsController.ResetAnimations();
        _health = _maxHealth;
        _playerController.ToNormalState();
    }
}
