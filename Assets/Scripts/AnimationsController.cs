using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsController {

    public readonly HoldGunStateBehaviour HoldGunIK;
    public readonly LookAtStateBehaviour LookAtIK;

    private Animator _animator;
    private PhysicsController _physicsController;

    private int _zMovementAnimationParameter = Animator.StringToHash("ZMovement");
    private int _xMovementVelocityAnimationParameter = Animator.StringToHash("XMovement");

    private int _isGroundedAnimationParameter = Animator.StringToHash("IsGrounded");
    //private int _jumpingAnimationParameter = Animator.StringToHash("JumpingTrigger");

    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");
    //private int _timeInAirAnimationParameter = Animator.StringToHash("TimeInAir");
    private int _pushingAnimationParameter = Animator.StringToHash("Pushing");
    private int _pickingUpGunParameter = Animator.StringToHash("PickingUpGun");
    private int _punchParameter = Animator.StringToHash("Punch");
    private int _isAimingGunParameter = Animator.StringToHash("IsAiming");
    private int _isTwoHandedGunParameter = Animator.StringToHash("IsTwoHandedGun");
    private int _distanceFromGroundParameter = Animator.StringToHash("DistanceFromGround");

    private int _resetParameter = Animator.StringToHash("Reset");

    public AnimationsController(Animator animator, PhysicsController physicsController)
    {
        _animator = animator;
        _physicsController = physicsController;

        HoldGunIK = _animator.GetBehaviour<HoldGunStateBehaviour>();
        LookAtIK = _animator.GetBehaviour<LookAtStateBehaviour>();
    }

    public void Update()
    {
        _animator.SetFloat(_zMovementAnimationParameter, _physicsController.Movement.z);
        _animator.SetFloat(_xMovementVelocityAnimationParameter, _physicsController.Movement.x);
        _animator.SetBool(_isGroundedAnimationParameter, _physicsController.IsGrounded());
        //_animator.SetBool(_jumpingAnimationParameter, _physicsController.Jumping);

        _animator.SetFloat(_horizontalRotationAnimationParameter, _physicsController.Aim.x);
        _animator.SetFloat(_verticalVelocityAnimationParameter, _physicsController.GetVelocity().y);

        //_animator.SetFloat(_timeInAirAnimationParameter, _physicsController.GetTimeInAir());
        _animator.SetFloat(_distanceFromGroundParameter, _physicsController.GetDistanceFromGround());
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

    public void ResetAnimations()
    {
        //_animator.SetFloat(_zMovementAnimationParameter, 0);
        //_animator.SetFloat(_xMovementVelocityAnimationParameter, 0);

        //_animator.SetFloat(_horizontalRotationAnimationParameter, 0);
        //_animator.SetFloat(_verticalVelocityAnimationParameter, 0);
        //_animator.SetFloat(_distanceFromGroundParameter, _physicsController.GetDistanceFromGround());

        //_animator.SetBool(_jumpingAnimationParameter, true);
        //_animator.SetBool(_pushingAnimationParameter, false);
        //_animator.SetBool(_pickingUpGunParameter, false);
        _animator.SetBool(_punchParameter, false);
        _animator.SetBool(_isAimingGunParameter, false);
        _animator.SetTrigger(_resetParameter);
    }
}
