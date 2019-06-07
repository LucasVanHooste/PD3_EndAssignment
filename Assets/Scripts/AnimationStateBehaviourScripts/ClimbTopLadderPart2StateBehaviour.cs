using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbTopLadderPart2StateBehaviour : StateMachineBehaviour {

    private PlayerMotor _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private EnemyBehaviour _enemyBehaviour;
    private EnemyMotor _navMeshAgentController;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerController != null)
        {
            _playerController.gameObject.layer = LayerMask.NameToLayer("Player");
            _physicsController.IsGroundedChecker.gameObject.layer=LayerMask.NameToLayer("Player");
        }
        if (_navMeshAgentController!=null)
            _navMeshAgentController.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerController != null)
        {
            _playerController.ToNormalState();
            _physicsController.HasGravity(true);
            _animationsController.Climb(false);
            animator.applyRootMotion = false;
            _playerController = null;
        }

        if (_navMeshAgentController != null)
        {
            _navMeshAgentController.RigidBody.isKinematic = true;
            _navMeshAgentController.ResetRigidbodyConstraints();
            _navMeshAgentController.UpdateTransformToNavmesh = true;
            _navMeshAgentController.Warp(_navMeshAgentController.transform.position);
            _animationsController.Climb(false);
            animator.applyRootMotion = false;
            _enemyBehaviour.StopMovementAction();
            _navMeshAgentController = null;
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    public void SetBehaviour(PlayerController playerController, PlayerMotor physicsController, AnimationsController animationsController)
    {
        _playerController = playerController;
        _physicsController = physicsController;
        _animationsController = animationsController;
    }

    public void SetBehaviour(EnemyBehaviour enemyBehaviour, EnemyMotor navMeshAgentController, AnimationsController animationsController)
    {
        _enemyBehaviour = enemyBehaviour;
        _navMeshAgentController = navMeshAgentController;
        _animationsController = animationsController;
    }
}
