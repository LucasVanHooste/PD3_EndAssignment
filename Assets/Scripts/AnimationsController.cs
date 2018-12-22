using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsController {

    private Animator _animator;
    private PhysicsController _physicsController;

    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimationParameter = Animator.StringToHash("HorizontalVelocity");

    private int _jumpingAnimationParameter = Animator.StringToHash("Jumping");
    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    private int _pushingAnimationParameter = Animator.StringToHash("Pushing");
    private int _pickingUpGunParameter = Animator.StringToHash("PickingUpGun");
    private int _punchParameter = Animator.StringToHash("Punch");

    public AnimationsController(Animator animator, PhysicsController physicsController)
    {
        _animator = animator;
        _physicsController = physicsController;
    }

    public void Update()
    {
        _animator.SetFloat(_verticalVelocityAnimationParameter, _physicsController.Movement.z);
        _animator.SetFloat(_horizontalVelocityAnimationParameter, _physicsController.Movement.x);
        _animator.SetBool(_jumpingAnimationParameter, _physicsController.Jumping);
        _animator.SetFloat(_horizontalRotationAnimationParameter, _physicsController.Aim.x);
    }

    public void Push(bool push)
    {
        _animator.SetBool(_pushingAnimationParameter, push);
    }

    public void PickUpGun(bool pickup)
    {
        _animator.SetBool(_pickingUpGunParameter, pickup);
    }

    public void Punch()
    {
        _animator.SetTrigger(_punchParameter);
    }

    public void SetPickUpGunStateBehaviour(Transform rightHand, Transform leftHand, Transform playerTransform, Transform gun)
    {
        _animator.GetBehaviour<PickUpGunStateBehaviour>().RightHand = rightHand;
        _animator.GetBehaviour<PickUpGunStateBehaviour>().LeftHand = leftHand;
        _animator.GetBehaviour<PickUpGunStateBehaviour>().Player = playerTransform;
        _animator.GetBehaviour<PickUpGunStateBehaviour>().Gun = gun;
    }

    internal void SetLayerWeight(int layerIndex, float weight)
    {
        _animator.SetLayerWeight(layerIndex, weight);
    }
}
