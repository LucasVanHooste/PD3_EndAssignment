using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingState : PlayerState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private List<Collider> _triggers;
    private GameObject _ladder;
    private LadderScript _ladderScript;

    float _ladderPaddingDistance = 0.15f;

    public ClimbingState(Transform playerTransform, PhysicsController physicsController, PlayerController playerController, AnimationsController animationsController, GameObject ladder)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _ladder = ladder;
        _ladderScript = _ladder.GetComponent<LadderScript>();
        _triggers = _playerController.Triggers;

        _animationsController.ClimbTopLadderAnimationBehaviour.SetBehaviour(_playerController, _physicsController, _animationsController);
        _animationsController.TopLadderIK.SetLadderScript(_ladderScript);

        Climb();
    }

    public override void Update()
    {
        
    }

    private void Climb()
    {
        _physicsController.Aim = Vector3.zero;
        _physicsController.StopMoving();

        _playerController.StartCoroutine(RotateToLadder());


        //_animationsController.ApplyRootMotion(true);
        //_physicsController.HasGravity(false);
        //_animationsController.Climb(true);
        //yield return new WaitForSeconds(5);

        //_animationsController.Climb(false);
        //_playerController.ToNormalState();
    }

    private IEnumerator RotateToLadder()
    {
        Vector3 direction = GetDirection();

        while (Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(direction)) > 1)
        {
            //direction = Vector3.Scale((_ladder.transform.position - _playerTransform.position), new Vector3(1, 0, 1));
            Debug.Log("rotate");
            Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, direction, .05f, 0.0f);

            float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
            _physicsController.Aim = new Vector3(angle / Mathf.Abs(angle), _physicsController.Aim.y, _physicsController.Aim.z);

            yield return null;
        }

        Debug.Log("finish rotation");
        _physicsController.Aim = Vector3.zero;
        _playerController.StartCoroutine(MoveToLadder());
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

        _physicsController.Movement = Vector3.zero;
        ClimbLadder();

    }

    private IEnumerator RotateToClimbDirection()
    {
        Vector3 direction = GetDirection();

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
        ClimbLadder();
    }

    private void ClimbLadder()
    {
        _animationsController.ApplyRootMotion(true);
        _physicsController.HasGravity(false);
        _animationsController.Climb(true);
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = Vector3.Scale((_ladder.transform.position - _playerTransform.position), new Vector3(1, 0, 1));
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            direction.z = 0;
        }
        else
        {
            direction.x = 0;
        }
        return direction;
    }

    public override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ladder" && !_physicsController.IsGrounded())
        {
            _playerTransform.gameObject.layer = 15;

            _animationsController.ClimbTopLadder();
        }
    }
}
