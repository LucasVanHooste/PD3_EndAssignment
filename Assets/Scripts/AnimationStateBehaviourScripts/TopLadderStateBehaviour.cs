using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLadderStateBehaviour : StateMachineBehaviour {

    private float _iKWeight = 1f;
    private LadderScript _ladderScript;

    private bool _hasMaxed = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _hasMaxed = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_ladderScript == null) return;
        Debug.Log(_ladderScript.gameObject.name);

        //if(Mathf.Approximately(_iKWeight,1))
        //    _hasMaxed = true;

        //if (!_hasMaxed)
        //{
        //    Mathf.Lerp(_iKWeight, 1, Time.deltaTime*4);
        //}
        //else
        //    Mathf.Lerp(_iKWeight, 0, Time.deltaTime*4);

        if (stateInfo.normalizedTime > .6f)
        {
            //IK
            animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(new Vector3(20, 0, 0)));

            if (_ladderScript.RightHandIK2)
            {
                animator.SetIKPosition(AvatarIKGoal.RightHand, _ladderScript.RightHandIK2.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, _ladderScript.RightHandIK2.rotation);

                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);
            }

            if (_ladderScript.LeftHandIK2)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, _ladderScript.LeftHandIK2.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, _ladderScript.LeftHandIK2.rotation);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _iKWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _iKWeight);
            }
            return;
        }


        //IK
        animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(new Vector3(20, 0, 0)));

        if (_ladderScript.RightHandIK)
        {
            animator.SetIKPosition(AvatarIKGoal.RightHand, _ladderScript.RightHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, _ladderScript.RightHandIK.rotation);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);
        }

        if (_ladderScript.LeftHandIK)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftHand, _ladderScript.LeftHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, _ladderScript.LeftHandIK.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _iKWeight);
        }
    }

    public void SetLadderScript(LadderScript ladderScript)
    {
        _ladderScript = ladderScript;
    }
}
