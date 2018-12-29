using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObstacleStateBehaviour : StateMachineBehaviour {

    private Transform _leftHandIK, _rightHandIK;
    private float _iKWeight=0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _iKWeight = 0;
    }

// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
//
//}

// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_leftHandIK==null || _rightHandIK==null) return;

        _iKWeight = Mathf.Lerp(_iKWeight, 0.8f, .02f);

            animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandIK.rotation);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);

            animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandIK.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _iKWeight);
    }

    public void SetIK(Transform obstacleIKLeftHand, Transform obstacleIKRightHand)
    {
        _leftHandIK = obstacleIKLeftHand;
        _rightHandIK = obstacleIKRightHand;
    }
}
