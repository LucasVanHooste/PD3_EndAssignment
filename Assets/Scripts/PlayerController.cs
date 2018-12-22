using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    public GameObject _obstacleCollisionChecker;
    [SerializeField] private CinematicBehaviour _cinematicBehaviour;
    public Transform LeftHand;
    public Transform RightHand;

    private Transform _transform;
    private PhysicsController _physicsController;
    private Animator _animator;
    private PlayerController _playerController;
    private CharacterController _characterController;
    private IState _state;
    private AnimationsController _animationsController;

    [HideInInspector] public List<Collider> Triggers = new List<Collider>();

    // Use this for initialization
    void Start () {
        _transform = transform;
        _physicsController = GetComponent<PhysicsController>();
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        _animationsController = new AnimationsController(_animator, _physicsController);
        _animationsController.HoldGunIK.SetPlayer(_transform);

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
        _state = new PushingState(_transform, _physicsController, _playerController, _animationsController, _obstacle);
        Debug.Log("ToPushingState");
    }
    public void ToCinematicState(GameObject _object)
    {
        _state = new CinematicState(_transform, _physicsController, _playerController, _animationsController, _object, _cinematicBehaviour);
        Debug.Log("ToPushingState");
    }
    public void ToGunState(GameObject _object)
    {
        _state = new GunState(_transform, _physicsController, _playerController, _animationsController, _object);
        Debug.Log("ToPushingState");
    }
}
