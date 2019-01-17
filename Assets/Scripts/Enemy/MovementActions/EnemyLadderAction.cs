using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLadderAction : IEnemyMovementAction
{
    AnimationsController _animationsController;
    EnemyPhysicsController _navMeshAgentController;
    EnemyBehaviour _enemyBehaviour;
    Transform _transform;
    Transform _ladder;
    LadderScript _ladderScript;
    Coroutine _climbLadder;

    const float _ladderPaddingDistance = 0.15f;

    public EnemyLadderAction(AnimationsController animationsController, EnemyPhysicsController navMeshAgentController, EnemyBehaviour enemyBehaviour, Transform ladder)
    {
        _animationsController = animationsController;
        _navMeshAgentController = navMeshAgentController;
        _enemyBehaviour = enemyBehaviour;
        _transform = _enemyBehaviour.transform;
        _ladder = ladder;
        _ladderScript = _ladder.GetComponent<LadderScript>();

        _climbLadder = _enemyBehaviour.StartCoroutine(InteractWithLadder());
    }

    private IEnumerator InteractWithLadder()
    {
        LadderScript ladderScript = _ladder.GetComponent<LadderScript>();
        
        //animaitons
        _animationsController.ClimbBottomLadderIK.LadderIKHands = ladderScript.BottomLadderIKHands;
        _animationsController.ClimbTopLadderPart1IK.Ladderscript = ladderScript;
        _animationsController.ClimbTopLadderPart2IK.SetBehaviour(_enemyBehaviour, _navMeshAgentController, _animationsController);

        //physics
        _navMeshAgentController.UpdateTransformToNavmesh = false;
        _navMeshAgentController.RigidBody.isKinematic = false;
        _navMeshAgentController.RigidBody.useGravity = false;

        while (ladderScript.IsPersonClimbing)
        {
            Debug.Log("wait for climb");
            yield return null;
        }

        _enemyBehaviour.HolsterGun();
        ladderScript.IsPersonClimbing = true;
        _enemyBehaviour.gameObject.layer = LayerMask.NameToLayer("NoCollisionWithEnemy");
        _climbLadder = _enemyBehaviour.StartCoroutine(RotateToLadder());
    }

    private IEnumerator RotateToLadder()
    {
        Vector3 direction = -_ladder.forward;

        while (Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(direction)) > 1)
        {
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 90f);
            yield return null;
        }

        _navMeshAgentController.RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        _climbLadder = _enemyBehaviour.StartCoroutine(MoveToLadder());
    }

    private IEnumerator MoveToLadder()
    {
        Vector3 ladderPosition = new Vector3(_ladder.GetComponent<LadderScript>().TakeOfPoint.position.x, _transform.position.y, _ladder.GetComponent<LadderScript>().TakeOfPoint.position.z);

        while (Vector3.Scale(ladderPosition - _transform.position, new Vector3(1, 0, 1)).sqrMagnitude > _ladderPaddingDistance * _ladderPaddingDistance)
        {
            Vector3 direction = ladderPosition - _transform.position;
            _navMeshAgentController.RigidBody.velocity = direction.normalized;
            yield return null;
        }
        _navMeshAgentController.RigidBody.velocity = Vector3.zero;
        ClimbLadder();

    }

    private void ClimbLadder()
    {
        _animationsController.ApplyRootMotion(true);
        _animationsController.Climb(true);
    }

    public void Stop()
    {
        _ladderScript.IsPersonClimbing = false;
        if (_climbLadder != null)
        {
            _enemyBehaviour.StopCoroutine(_climbLadder);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (_enemyBehaviour.Health > 0 && other.gameObject.CompareTag("Ladder"))
        {
            _transform.gameObject.layer = LayerMask.NameToLayer("NoCollisions");

            _animationsController.ClimbTopLadder();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {

    }
}
