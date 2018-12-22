using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicState : IState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private Animator _animator;
    private List<Collider> _triggers;
    private GameObject _object;

    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimationParameter = Animator.StringToHash("HorizontalVelocity");

    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    private int _pickingUpGunParameter = Animator.StringToHash("PickingUpGun");

    private CinematicBehaviour _cinematicBehaviour;

    public CinematicState(Transform playerTransform, PhysicsController physicsController, PlayerController playerController, Animator animator, GameObject triggerObject, CinematicBehaviour cinematicBehaviour)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animator = animator;
        _object = triggerObject;
        _triggers = _playerController.Triggers;
        _cinematicBehaviour = cinematicBehaviour;

        if(_object.tag=="FirstGun")
        PickupFirstGun();
    }

    public void Update()
    {
        _animator.SetFloat(_verticalVelocityAnimationParameter, _physicsController.Movement.z);
        _animator.SetFloat(_horizontalVelocityAnimationParameter, _physicsController.Movement.x);
        _animator.SetFloat(_horizontalRotationAnimationParameter, _physicsController.Aim.x);
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

    //private IEnumerator PickupGun()
    //{
    //    Debug.Log("Pick up gun");
    //    _physicsController.Movement = Vector3.zero;
    //    _physicsController.Aim = Vector3.zero;

    //    yield return new WaitUntil(IsFacingObject);
    //    yield return new WaitForSeconds(.2f);
    //    StartCoroutine(_cinematicBehaviour.PlayCinematicScene("PickUpFirstGun"));
    //    //PistolHandle.SetParent(_relativeForward);

    //    yield return new WaitForSeconds(1);
    //    _animator.SetBool(_pickingUpGunParameter, true);
    //    _animator.SetLayerWeight(1, 1);
    //    //StartCoroutine(LerpLayerWeight(1, 1, .02f));

    //    yield return new WaitUntil(_cinematicBehaviour.GetIsSceneFinished);
    //    _animator.SetBool(_pickingUpGunParameter, false);
    //    _animator.SetLayerWeight(1, 0);
    //    //StartCoroutine(LerpLayerWeight(1, 0, .03f));
    //    _state = PlayerState.HoldingGun;
    //}

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

        _animator.SetBool(_pickingUpGunParameter, true);
        _animator.SetLayerWeight(1, 1);
        //StartCoroutine(LerpLayerWeight(1, 1, .02f));

        yield return new WaitUntil(_cinematicBehaviour.GetIsSceneFinished);
        _animator.SetBool(_pickingUpGunParameter, false);
        _animator.SetLayerWeight(1, 0);
        //StartCoroutine(LerpLayerWeight(1, 0, .03f));
        _playerController.ToGunState();
    }

    //private bool IsFacingObject()
    //{
    //    Vector3 direction = Vector3.Scale((_object.transform.position - transform.position), new Vector3(1, 0, 1));
    //    Vector3 newDir = Vector3.RotateTowards(_characterController.transform.forward, direction, .05f, 0.0f);
    //    //_characterController.transform.rotation = Quaternion.LookRotation(newDir);

    //    //float angle = Quaternion.Angle(_characterController.transform.rotation, Quaternion.LookRotation(newDir));
    //    float angle = Vector3.SignedAngle(_characterController.transform.forward, newDir, Vector3.up);
    //    _aim.x = angle / Mathf.Abs(angle);
    //    Debug.Log("angle: " + angle);

    //    if (Quaternion.Angle(_characterController.transform.rotation, Quaternion.LookRotation(direction)) < 1)
    //    {
    //        _aim = Vector3.zero;
    //        return true;
    //    }


    //    return false;
    //}
}
