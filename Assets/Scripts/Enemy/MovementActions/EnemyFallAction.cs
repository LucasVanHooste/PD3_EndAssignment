using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFallAction : IEnemyMovementAction
{
    EnemyPhysicsController _navMeshAgentController;
    EnemyBehaviour _enemyBehaviour;
    Transform _transform;
    Transform _offMeshLink;

    Coroutine _jump;

    public EnemyFallAction(EnemyPhysicsController navMeshAgentController,EnemyBehaviour enemyBahvriour, Transform offMeshLink)
    {
        _navMeshAgentController = navMeshAgentController;
        _enemyBehaviour = enemyBahvriour;
        _transform = _navMeshAgentController.transform;
        _offMeshLink = offMeshLink;

        _jump = _navMeshAgentController.StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        _navMeshAgentController.Fall(_offMeshLink);
        yield return null;
    }

    public void OnTriggerExit(Collider other)
    {
        
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
        }
    }
}
