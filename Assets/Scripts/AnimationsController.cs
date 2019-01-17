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
    private PlayerPhysicsController _physicsController;
    private EnemyPhysicsController _navMeshAgentController;

    private int _zMovementAnimationParameter = Animator.StringToHash("ZMovement");
    private int _xMovementAnimationParameter = Animator.StringToHash("XMovement");

    private int _isGroundedAnimationParameter = Animator.StringToHash("IsGrounded");
    private int _distanceFromGroundParameter = Animator.StringToHash("DistanceFromGround");

    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");

    private int _pushingAnimationParameter = Animator.StringToHash("Pushing");
    private int _pickingUpGunParameter = Animator.StringToHash("PickingUpGun");

    private int _punchParameter = Animator.StringToHash("Punch");
    private int _isAimingGunParameter = Animator.StringToHash("IsAiming");
    private int _isTwoHandedGunParameter = Animator.StringToHash("IsTwoHandedGun");

    private int _climbingAnimationParameter = Animator.StringToHash("Climbing");
    private int _climbTopAnimationParameter = Animator.StringToHash("ClimbTopLadder");

    private int _takeDamageAnimationParameter = Animator.StringToHash("TakeDamage");
    private int _hitOriginXParameter = Animator.StringToHash("HitOriginX");
    private int _hitOriginZParameter = Animator.StringToHash("HitOriginZ");
    private int _deathParameter = Animator.StringToHash("Die");

    private int _resetParameter = Animator.StringToHash("Reset");

    private AnimationsController(Animator animator)
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

    public AnimationsController(Animator animator, EnemyPhysicsController navMeshAgentController) : this(animator)
    {       
        _navMeshAgentController = navMeshAgentController;
    }
    public AnimationsController(Animator animator, PlayerPhysicsController physicsController) : this(animator)
    {
        _physicsController = physicsController;
    }

    public void Update()
    {
        if (_physicsController != null)
        {
            _animator.SetFloat(_zMovementAnimationParameter, _physicsController.Movement.z);
            _animator.SetFloat(_xMovementAnimationParameter, _physicsController.Movement.x);
            _animator.SetFloat(_horizontalRotationAnimationParameter, _physicsController.Aim.x);

            _animator.SetBool(_isGroundedAnimationParameter, _physicsController.IsGrounded());
            _animator.SetFloat(_distanceFromGroundParameter, _physicsController.GetDistanceFromGround());
            _animator.SetFloat(_verticalVelocityAnimationParameter, _physicsController.GetVelocity().y);
        }
        if (_navMeshAgentController != null)
        {
            _animator.SetFloat(_zMovementAnimationParameter, _navMeshAgentController.RelativeVelocity.z);
            _animator.SetFloat(_xMovementAnimationParameter, _navMeshAgentController.RelativeVelocity.x);
            _animator.SetFloat(_horizontalRotationAnimationParameter, _navMeshAgentController.RotationSpeed);

            _animator.SetFloat(_verticalVelocityAnimationParameter, _navMeshAgentController.RelativeVelocity.y);
            _animator.SetBool(_isGroundedAnimationParameter, _navMeshAgentController.IsGrounded());
            _animator.SetFloat(_distanceFromGroundParameter, _navMeshAgentController.DistanceFromGround);
        }
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
