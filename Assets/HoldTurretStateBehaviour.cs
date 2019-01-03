using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldTurretStateBehaviour : StateMachineBehaviour {

    private TurretScript _turretScript;
    private bool _isAiming = false;

    private float _iKWeight = 0;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
        
    //}

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    //OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK(inverse kinematics) should be implemented here.

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_turretScript == null)
        {
            _iKWeight = 0;
            return;
        }
        //Debug.Log(_gun.gameObject.name);

            _iKWeight = Mathf.Lerp(_iKWeight, 1, .03f);

        //IK all other guns
        if (_turretScript.RightHandIK)
        {
            animator.SetIKPosition(AvatarIKGoal.RightHand, _turretScript.RightHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, _turretScript.RightHandIK.rotation);


            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);
        }

        if (_turretScript.LeftHandIK)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftHand, _turretScript.LeftHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, _turretScript.LeftHandIK.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _iKWeight);
        }

        //if (_turretScript.LeftElbowIK)
        //{
        //    animator.SetIKHintPosition(AvatarIKHint.LeftElbow, _turretScript.LeftElbowIK.position);
        //    animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1f);
        //}
        //if (_turretScript.RightElbowIK)
        //{
        //    animator.SetIKHintPosition(AvatarIKHint.RightElbow, _turretScript.RightElbowIK.position);
        //    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, .5f);
        //}

    }

    public void SetTurretScript(TurretScript turretScript)
    {
        _turretScript = turretScript;
    }
}
