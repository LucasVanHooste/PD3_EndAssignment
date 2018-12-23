using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsController {

    public readonly HoldGunStateBehaviour HoldGunIK;
    public readonly LookAtStateBehaviour LookAtIK;

    private Animator _animator;
    private PhysicsController _physicsController;

    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    private int _horizontalVelocityAnimationParameter = Animator.StringToHash("HorizontalVelocity");

    private int _jumpingAnimationParameter = Animator.StringToHash("Jumping");
    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    private int _pushingAnimationParameter = Animator.StringToHash("Pushing");
    private int _pickingUpGunParameter = Animator.StringToHash("PickingUpGun");
    private int _punchParameter = Animator.StringToHash("Punch");
    private int _isAimingGunParameter = Animator.StringToHash("IsAiming");
    private int _isTwoHandedGunParameter = Animator.StringToHash("IsTwoHandedGun");

    public AnimationsController(Animator animator, PhysicsController physicsController)
    {
        _animator = animator;
        _physicsController = physicsController;

        HoldGunIK = _animator.GetBehaviour<HoldGunStateBehaviour>();
        LookAtIK = _animator.GetBehaviour<LookAtStateBehaviour>();
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

    public void AimGun(bool aimGun)
    {
        HoldGunIK.SetIsAiming(aimGun);
        _animator.SetBool(_isAimingGunParameter, aimGun);
    }

    public void IsTwoHandedGun(bool isTwoHandedGun)
    {
        _animator.SetBool(_isTwoHandedGunParameter, isTwoHandedGun);
    }

    public void Punch()
    {
        _animator.SetTrigger(_punchParameter);
    }

    public void SetLayerWeight(int layerIndex, float weight)
    {
        _animator.SetLayerWeight(layerIndex, weight);
    }
}
