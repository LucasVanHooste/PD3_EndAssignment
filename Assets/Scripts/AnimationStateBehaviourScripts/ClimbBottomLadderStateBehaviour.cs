using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbBottomLadderStateBehaviour : StateMachineBehaviour {

    private float _IKWeightLeftHand, _IKWeightRightHand;
    private Transform _closestIKToHand;

    public Transform[] LadderIKHands { private get; set; }
    public Transform LeftHand { private get; set; } 
    public Transform RightHand { private get; set; }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _IKWeightLeftHand = 0;
        _IKWeightRightHand = 0;
    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //parameters get value from animation curve
        _IKWeightRightHand = animator.GetFloat("LadderIKWeightRightHand");
        _IKWeightLeftHand = animator.GetFloat("LadderIKWeightLeftHand");

        GetClosestIKToHand(LeftHand);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, _closestIKToHand.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, _closestIKToHand.rotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _IKWeightLeftHand);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _IKWeightLeftHand);

        GetClosestIKToHand(RightHand);
        animator.SetIKPosition(AvatarIKGoal.RightHand, _closestIKToHand.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, _closestIKToHand.rotation);

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _IKWeightRightHand);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _IKWeightRightHand);
    }

    private void GetClosestIKToHand(Transform hand)
    {
        if (_closestIKToHand == null)
            _closestIKToHand = LadderIKHands[0];

        foreach (Transform handIK in LadderIKHands)
        {
            if ((hand.position - handIK.position).sqrMagnitude < (hand.position - _closestIKToHand.position).sqrMagnitude)
            {
                _closestIKToHand = handIK;
            }
        }
    }
}
