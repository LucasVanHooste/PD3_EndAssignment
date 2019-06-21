using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbTopLadderPart2StateBehaviour : StateMachineBehaviour {

    private PlayerMotor _playerMotor;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private EnemyBehaviour _enemyBehaviour;
    private EnemyMotor _enemyMotor;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerController != null)
        {
            _playerController.gameObject.layer = LayerMask.NameToLayer("Player");
            _playerMotor.IsGroundedChecker.gameObject.layer=LayerMask.NameToLayer("Player");
        }
        if (_enemyMotor!=null)
            _enemyMotor.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerController != null)
        {
            _playerController.SwitchState<NormalState>();
            _playerMotor.HasGravity=true;
            _animationsController.Climb(false);
            animator.applyRootMotion = false;
            _playerController = null;
        }

        if (_enemyMotor != null)
        {
            _enemyMotor.RigidBody.isKinematic = true;
            _enemyMotor.ResetRigidbodyConstraints();
            _enemyMotor.UpdateTransformToNavmesh = true;
            _enemyMotor.Warp(_enemyMotor.transform.position);
            _animationsController.Climb(false);
            animator.applyRootMotion = false;
            _enemyBehaviour.StopMovementAction();
            _enemyMotor = null;
        }
    }


    public void SetBehaviour(PlayerController playerController, PlayerMotor playerMotor, AnimationsController animationsController)
    {
        _playerController = playerController;
        _playerMotor = playerMotor;
        _animationsController = animationsController;
    }

    public void SetBehaviour(EnemyBehaviour enemyBehaviour, EnemyMotor enemyMotor, AnimationsController animationsController)
    {
        _enemyBehaviour = enemyBehaviour;
        _enemyMotor = enemyMotor;
        _animationsController = animationsController;
    }
}
