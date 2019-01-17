using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpAction : IEnemyMovementAction
{
    EnemyPhysicsController _navMeshAgentController;
    EnemyBehaviour _enemyBehaviour;
    Transform _transform;
    Transform _offMeshLink;

    Coroutine _jump;

    public EnemyJumpAction(EnemyPhysicsController navMeshAgentController,EnemyBehaviour enemyBehaviour, Transform offMeshLink)
    {
        _navMeshAgentController = navMeshAgentController;
        _enemyBehaviour = enemyBehaviour;
        _transform = _navMeshAgentController.transform;
        _offMeshLink = offMeshLink;

        _jump = _navMeshAgentController.StartCoroutine(Jump());
    }

    private IEnumerator Jump()
    {
        _navMeshAgentController.Jump(_offMeshLink);
        yield return null;
    }

    public void Stop()
    {
        if (_jump != null)
        {
            _navMeshAgentController.StopCoroutine(_jump);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (_navMeshAgentController.IsGrounded())
        {
                _navMeshAgentController.UpdateTransformToNavmesh = true;
                _navMeshAgentController.Warp(_transform.position);

                _navMeshAgentController.RigidBody.useGravity = false;
                _navMeshAgentController.RigidBody.isKinematic = true;

            _enemyBehaviour.StopMovementAction();
            Debug.Log("end jump");
        }
    }

    public void OnTriggerExit(Collider other)
    {

    }
}

