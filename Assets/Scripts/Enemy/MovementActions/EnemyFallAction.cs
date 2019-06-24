using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFallAction : IEnemyMovementAction
{
    EnemyMotor _enemyMotor;
    EnemyBehaviour _enemyBehaviour;
    Transform _transform;
    Transform _offMeshLink;

    public EnemyFallAction(EnemyMotor enemyMotor,EnemyBehaviour enemyBahvriour)
    {
        _enemyMotor = enemyMotor;
        _enemyBehaviour = enemyBahvriour;
        _transform = _enemyMotor.transform;
    }

    public void OnActionEnter()
    {
        Fall();
    }

    private void Fall()
    {
        _enemyMotor.Fall(_offMeshLink);
    }

    public void OnTriggerExit(Collider other)
    {
        
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

    public void ResetAction(Transform actionTrigger)
    {
        _offMeshLink = actionTrigger;
    }
}
