using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingState : BasePlayerState
{
    private Transform _playerTransform;
    private PlayerMotor _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;

    private LadderScript _ladderScript;

    private const float _ladderPaddingDistance = 0.15f;
    private Coroutine _climbLadder;

    public ClimbingState(PlayerMotor physicsController, PlayerController playerController, AnimationsController animationsController)
    {
        _playerTransform = PlayerController.PlayerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
    }

    public override void ResetState(IInteractable ladder)
    {
        _ladderScript = (LadderScript)ladder;

        _animationsController.ClimbBottomLadderIK.LadderIKHands = _ladderScript.BottomLadderIKHands;
        _animationsController.ClimbTopLadderPart1IK.Ladder = _ladderScript;
        _animationsController.ClimbTopLadderPart2IK.SetBehaviour(_playerController, _physicsController, _animationsController);
    }

    public override void OnStateEnter()
    {
        Climb();
    }

    public override void OnStateExit()
    {
        
    }

    public override void Update()
    {

    }

    private void Climb()
    {
        _physicsController.Aim = Vector3.zero;
        _physicsController.StopMoving();

        _ladderScript.IsPersonClimbing = true;
        _climbLadder= _playerController.StartCoroutine(RotateToLadder());
    }

    private IEnumerator RotateToLadder()
    {
        Vector3 direction = -_ladderScript.transform.forward;

        while (Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(direction)) > 1)
        {
            Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, direction, .05f, 0.0f);

            float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
            _physicsController.Aim = new Vector3(Mathf.Rad2Deg * angle / _physicsController.HorizontalRotationSpeed, _physicsController.Aim.y, _physicsController.Aim.z);
            yield return null;
        }

        _physicsController.Aim = Vector3.zero;
        _climbLadder= _playerController.StartCoroutine(MoveToLadder());
    }

    private IEnumerator MoveToLadder()
    {
        Vector3 ladderPosition = new Vector3(_ladderScript.TakeOfPoint.position.x, _playerTransform.position.y, _ladderScript.TakeOfPoint.position.z);
        while(Vector3.Scale(ladderPosition- _playerTransform.position, new Vector3(1, 0, 1)).magnitude > _ladderPaddingDistance)
        {
            Vector3 lerp = Vector3.Lerp(_playerTransform.position, ladderPosition, .8f) - _playerTransform.position;
            _physicsController.Movement = _playerTransform.InverseTransformVector(new Vector3(lerp.x, 0, lerp.z));
            yield return null;
        }

        _physicsController.StopMoving();
        ClimbLadder();

    }


    private void ClimbLadder()
    {
        _animationsController.ApplyRootMotion(true);
        _physicsController.HasGravity=false;
        _animationsController.Climb(true);
    }

    public override void OnTriggerExit(Collider other)
    {
        if (_playerController.Health>0 && other.gameObject.CompareTag("Ladder") && !_physicsController.IsGrounded)
        {
            int mask = LayerMask.NameToLayer("NoCollisions");

            _playerTransform.gameObject.layer = mask;
            _physicsController.IsGroundedChecker.gameObject.layer = mask;
            _animationsController.ClimbTopLadder();
        }
    }

    public override void Die()
    {
        if(_climbLadder!=null)
        _playerController.StopCoroutine(_climbLadder);

        _ladderScript.IsPersonClimbing = false;
        _physicsController.HasGravity=true;
        _animationsController.Climb(false);
        _animationsController.ApplyRootMotion(false);
        _playerController.gameObject.layer = LayerMask.NameToLayer("Player");
    }


}
