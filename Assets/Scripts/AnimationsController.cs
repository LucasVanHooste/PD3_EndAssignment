using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsController {

    public readonly HoldGunStateBehaviour HoldGunIK;
    public readonly LookAtStateBehaviour LookAtIK;
    public readonly ClimbBottomLadderStateBehaviour ClimbBottomLadderIK;
    public readonly ClimbTopLadderPart2StateBehaviour ClimbTopLadderPart2IK;
    public readonly ClimbTopLadderPart1StateBehaviour ClimbTopLadderPart1IK;
    public readonly PushObstacleStateBehaviour ObstacleIK;
    public readonly HoldTurretStateBehaviour TurretIK;

    private Animator _animator;

    private static int _zMovementAnimationParameter = Animator.StringToHash("ZMovement");
    private static int _xMovementAnimationParameter = Animator.StringToHash("XMovement");

    private static int _isGroundedAnimationParameter = Animator.StringToHash("IsGrounded");
    private static int _distanceFromGroundParameter = Animator.StringToHash("DistanceFromGround");

    private static int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    private static int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");

    private static int _pushingAnimationParameter = Animator.StringToHash("Pushing");
    private static int _pickingUpGunParameter = Animator.StringToHash("PickingUpGun");

    private static int _punchParameter = Animator.StringToHash("Punch");
    private static int _isAimingGunParameter = Animator.StringToHash("IsAiming");
    private static int _isTwoHandedGunParameter = Animator.StringToHash("IsTwoHandedGun");

    private static int _climbingAnimationParameter = Animator.StringToHash("Climbing");
    private static int _climbTopAnimationParameter = Animator.StringToHash("ClimbTopLadder");

    private static int _takeDamageAnimationParameter = Animator.StringToHash("TakeDamage");
    private static int _hitOriginXParameter = Animator.StringToHash("HitOriginX");
    private static int _hitOriginZParameter = Animator.StringToHash("HitOriginZ");
    private static int _deathParameter = Animator.StringToHash("Die");

    private static int _resetParameter = Animator.StringToHash("Reset");

    public AnimationsController(Animator animator)
    {
        _animator = animator;

        HoldGunIK = _animator.GetBehaviour<HoldGunStateBehaviour>();
        LookAtIK = _animator.GetBehaviour<LookAtStateBehaviour>();
        ClimbBottomLadderIK = _animator.GetBehaviour<ClimbBottomLadderStateBehaviour>();
        ClimbTopLadderPart2IK = _animator.GetBehaviour<ClimbTopLadderPart2StateBehaviour>();
        ClimbTopLadderPart1IK = _animator.GetBehaviour<ClimbTopLadderPart1StateBehaviour>();
        ObstacleIK = _animator.GetBehaviour<PushObstacleStateBehaviour>();
        TurretIK = _animator.GetBehaviour<HoldTurretStateBehaviour>();
    }


    public void SetHorizontalMovement(Vector3 movement)
    {
        _animator.SetFloat(_zMovementAnimationParameter, movement.z);
        _animator.SetFloat(_xMovementAnimationParameter, movement.x);
    }

    public void SetVerticalVelocity(float velocity)
    {
        _animator.SetFloat(_verticalVelocityAnimationParameter, velocity);
    }

    public void SetRotationSpeed(float speed)
    {
        _animator.SetFloat(_horizontalRotationAnimationParameter, speed);
    }

    public void SetIsGrounded(bool isGrounded)
    {
        _animator.SetBool(_isGroundedAnimationParameter, isGrounded);
    }

    public void SetDistanceFromGround(float distance)
    {
        _animator.SetFloat(_distanceFromGroundParameter, distance);
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
        HoldGunIK.IsAiming = aimGun;
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

    public void TakeDamage()
    {
        _animator.SetTrigger(_takeDamageAnimationParameter);
    }

    public void Die(float localXCoordinate, float localZCoordinate)
    {
        _animator.SetFloat(_hitOriginXParameter, localXCoordinate);
        _animator.SetFloat(_hitOriginZParameter, localZCoordinate);
        _animator.SetTrigger(_deathParameter);
    }

    public void Climb(bool climb)
    {
        _animator.SetBool(_climbingAnimationParameter, climb);
    }
    public void ClimbTopLadder()
    {
        _animator.SetTrigger(_climbTopAnimationParameter);
    }

    public void SetLayerWeight(int layerIndex, float weight)
    {
        _animator.SetLayerWeight(layerIndex, weight);
    }

    public void ResetAnimations()
    {
        _animator.SetBool(_isAimingGunParameter, false);
        _animator.SetTrigger(_resetParameter);
        //other parameters are reset on update
    }

    public void ApplyRootMotion(bool apply)
    {
        _animator.applyRootMotion = apply;
    }
}
