using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicState : IState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private List<Collider> _triggers;
    private GameObject _object;

    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimationParameter = Animator.StringToHash("HorizontalVelocity");

    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    private int _pickingUpGunParameter = Animator.StringToHash("PickingUpGun");

    private CinematicBehaviour _cinematicBehaviour;

    public CinematicState(Transform playerTransform, PhysicsController physicsController, PlayerController playerController, AnimationsController animationsController, GameObject triggerObject, CinematicBehaviour cinematicBehaviour)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _object = triggerObject;
        _triggers = _playerController.Triggers;
        _cinematicBehaviour = cinematicBehaviour;

        if(_object.tag=="FirstGun")
        PickupFirstGun();
    }

    public void Update()
    {
        //_animationsController.SetFloat(_verticalVelocityAnimationParameter, _physicsController.Movement.z);
        //_animationsController.SetFloat(_horizontalVelocityAnimationParameter, _physicsController.Movement.x);
        //_animationsController.SetFloat(_horizontalRotationAnimationParameter, _physicsController.Aim.x);
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        
    }

    public void OnTriggerExit(Collider other)
    {
        
    }

    private void PickupFirstGun()
    {
        Debug.Log("Pick up gun");
        _physicsController.Movement = Vector3.zero;
        _physicsController.Aim = Vector3.zero;

        _playerController.StartCoroutine(RotateToObject());
    }

    private IEnumerator RotateToObject()
    {
        Vector3 direction = Vector3.Scale((_object.transform.position - _playerTransform.position), new Vector3(1, 0, 1));

        while (Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(direction)) > 1)
        {
            Debug.Log("rotate");
            Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, direction, .05f, 0.0f);

            float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
            _physicsController.Aim = new Vector3(angle / Mathf.Abs(angle), _physicsController.Aim.y, _physicsController.Aim.z);

            yield return null;
        }

        Debug.Log("finish rotation");
        _physicsController.Aim = Vector3.zero;
        yield return new WaitForSeconds(.2f);
        _playerController.StartCoroutine(PickUpGun());
    }

    private IEnumerator PickUpGun()
    {
        _cinematicBehaviour.PlayCinematicScene("PickUpFirstGun");
        yield return new WaitForSeconds(1);

        _animationsController.PickUpGun(true);
        _animationsController.SetLayerWeight(1, 1);
        //StartCoroutine(LerpLayerWeight(1, 1, .02f));

        yield return new WaitUntil(_cinematicBehaviour.GetIsSceneFinished);
        _animationsController.PickUpGun(false);
        _animationsController.SetLayerWeight(1, 0);
        //StartCoroutine(LerpLayerWeight(1, 0, .03f));
        _playerController.ToGunState(_object);
    }
}
