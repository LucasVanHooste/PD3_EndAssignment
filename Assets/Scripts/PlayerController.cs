using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    [SerializeField] public GameObject _obstacleCollisionChecker;

    private PhysicsController _physicsController;
    private Animator _animator;
    private PlayerController _playerController;
    private CharacterController _characterController;
    private IState _state;

    public List<Collider> Triggers = new List<Collider>();

    // Use this for initialization
    void Start () {
        _physicsController = GetComponent<PhysicsController>();
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        _state = new NormalState(transform, _physicsController, _playerController, _animator);
	}
	
	// Update is called once per frame
	void Update () {
        _state.Update();
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
        _state = new NormalState(transform, _physicsController,_playerController, _animator);
        Debug.Log("ToNormalState");
    }
    public void ToPushingState(GameObject _obstacle)
    {
        _state = new PushingState(transform, _physicsController, _playerController, _animator, _obstacle);
        Debug.Log("ToPushingState");
    }
}
