using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpAction : IEnemyMovementAction
{
    EnemyMotor _enemyMotor;
    EnemyBehaviour _enemyBehaviour;
    Transform _transform;
    Transform _offMeshLink;

    public EnemyJumpAction(EnemyMotor enemyMotor,EnemyBehaviour enemyBehaviour)
    {
        _enemyMotor = enemyMotor;
        _enemyBehaviour = enemyBehaviour;
        _transform = _enemyMotor.transform;
    }

    public void OnActionEnter()
    {
        Jump();
    }

    private void Jump()
    {
        _enemyMotor.Jump(_offMeshLink);
    }

    public void Stop()
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (_enemyMotor.IsGrounded)
        {
                _enemyMotor.UpdateTransformToNavmesh = true;
                _enemyMotor.Warp(_transform.position);

                _enemyMotor.RigidBody.useGravity = false;
                _enemyMotor.RigidBody.isKinematic = true;

            _enemyBehaviour.StopMovementAction();
        }
    }

    public void OnTriggerExit(Collider other)
    {

    }

    public void ResetAction(Transform actionTrigger)
    {
        _offMeshLink = actionTrigger;
    }
}

