using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationsController{

    public readonly HoldGunStateBehaviour HoldGunIK;
    public readonly LookAtStateBehaviour LookAtIK;
    public readonly ClimbTopLadderPart2StateBehaviour ClimbTopLadderAnimationBehaviour;
    public readonly ClimbTopLadderPart1StateBehaviour TopLadderIK;

    private Transform _enemyTransform;
    private Animator _animator;
    private NavMeshAgentController _navMeshAgentController;
    private MeleeEnemyBehaviour _enemyBehaviour;

    private int _zMovementAnimationParameter = Animator.StringToHash("ZMovement");
    private int _xMovementAnimationParameter = Animator.StringToHash("XMovement");

    private int _isGroundedAnimationParameter = Animator.StringToHash("IsGrounded");

    private int _horizontalRotationAnimationParameter = Animator.StringToHash("HorizontalRotation");
    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");

    private int _punchParameter = Animator.StringToHash("Punch");
    private int _isAimingGunParameter = Animator.StringToHash("IsAiming");
    private int _isTwoHandedGunParameter = Animator.StringToHash("IsTwoHandedGun");
    private int _distanceFromGroundParameter = Animator.StringToHash("DistanceFromGround");
    private int _climbingAnimationParameter = Animator.StringToHash("Climbing");
    private int _climbTopAnimationParameter = Animator.StringToHash("ClimbTopLadder");

    private int _takeDamageAnimationParameter = Animator.StringToHash("TakeDamage");
    private int _enemyHealthParameter = Animator.StringToHash("Health");


    private int _resetParameter = Animator.StringToHash("Reset");

    public EnemyAnimationsController(Transform enemyTransform, MeleeEnemyBehaviour enemyBehaviour, Animator animator, NavMeshAgentController navMeshAgentController)
    {
        _animator = animator;
        _navMeshAgentController= navMeshAgentController;
        _enemyBehaviour = enemyBehaviour;

        HoldGunIK = _animator.GetBehaviour<HoldGunStateBehaviour>();
        LookAtIK = _animator.GetBehaviour<LookAtStateBehaviour>();
        ClimbTopLadderAnimationBehaviour = _animator.GetBehaviour<ClimbTopLadderPart2StateBehaviour>();
        TopLadderIK = _animator.GetBehaviour<ClimbTopLadderPart1StateBehaviour>();
    }

    public void Update()
    {
        _animator.SetFloat(_zMovementAnimationParameter, _navMeshAgentController.RelativeVelocity.z);
        _animator.SetFloat(_xMovementAnimationParameter, _navMeshAgentController.RelativeVelocity.x);
        //_animator.SetBool(_isGroundedAnimationParameter, !_enemyBehaviour.IsOnOffMeshLink());
        _animator.SetBool(_isGroundedAnimationParameter, _enemyBehaviour.IsGrounded());

        //Debug.Log("Distance: "+_enemyBehaviour.GetDistanceFromGround());
        _animator.SetFloat(_horizontalRotationAnimationParameter, _navMeshAgentController.RotationSpeed);
        //Debug.Log("velocity: " + _enemyBehaviour.RelativeVelocity.y);
        //_animator.SetFloat(_verticalVelocityAnimationParameter, _enemyBehaviour.RelativeVelocity.y);

        _animator.SetFloat(_distanceFromGroundParameter, _enemyBehaviour.GetDistanceFromGround());
        //Debug.Log("velocity z: " + _enemyBehaviour.RelativeVelocity.z);
        //Debug.Log("velocity x: " + _enemyBehaviour.RelativeVelocity.x);
        //Debug.Log("animation angle: " + _navMeshAgentController.RotationSpeed);

    }

    public void AimGun(bool aimGun)
    {
        HoldGunIK.IsAiming=aimGun;
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

    public void Climb(bool climb)
    {
        _animator.SetBool(_climbingAnimationParameter, climb);
    }
    public void ClimbTopLadder()
    {
        _animator.SetTrigger(_climbTopAnimationParameter);
    }

    public void SetHealth(int health)
    {
        _animator.SetInteger(_enemyHealthParameter, health);
    }

    public void SetLayerWeight(int layerIndex, float weight)
    {
        _animator.SetLayerWeight(layerIndex, weight);
    }

    public void ResetAnimations()
    {
        _animator.SetBool(_punchParameter, false);
        _animator.SetBool(_isAimingGunParameter, false);
        _animator.SetTrigger(_resetParameter);
    }

    public void ApplyRootMotion(bool apply)
    {
        _animator.applyRootMotion = apply;
    }
}
