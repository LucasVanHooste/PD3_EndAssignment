using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbTopLadderPart1StateBehaviour : StateMachineBehaviour {

    private float _iKWeight = 1f;
    public LadderScript Ladder { private get; set; }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Ladder.IsPersonClimbing = false;
    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Ladder == null) return;

        if (stateInfo.normalizedTime > .6f)
        {
            //IK
            animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(new Vector3(20, 0, 0)));

            if (Ladder.TopLadderRightHandIK2)
            {
                animator.SetIKPosition(AvatarIKGoal.RightHand, Ladder.TopLadderRightHandIK2.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, Ladder.TopLadderRightHandIK2.rotation);

                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);
            }

            if (Ladder.TopLadderLeftHandIK2)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, Ladder.TopLadderLeftHandIK2.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, Ladder.TopLadderLeftHandIK2.rotation);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _iKWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _iKWeight);
            }
            return;
        }


        //IK
        animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(new Vector3(20, 0, 0)));

        if (Ladder.TopLadderRightHandIK)
        {
            animator.SetIKPosition(AvatarIKGoal.RightHand, Ladder.TopLadderRightHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, Ladder.TopLadderRightHandIK.rotation);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);
        }

        if (Ladder.TopLadderLeftHandIK)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftHand, Ladder.TopLadderLeftHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, Ladder.TopLadderLeftHandIK.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _iKWeight);
        }
    }

}
