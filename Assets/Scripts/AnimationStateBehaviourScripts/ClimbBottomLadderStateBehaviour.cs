using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbBottomLadderStateBehaviour : StateMachineBehaviour {

    private float _IKWeightLeftHand, _IKWeightRightHand;
    private Transform _closestIKToHand;

    private Transform[] _ladderIKHands;
    public Transform[] LadderIKHands
    {
        set
        {
            _ladderIKHands = value;
        }
    }

    private Transform _leftHand;
    public Transform LeftHand
    {
        set
        {
            _leftHand = value;
        }
    }
    private Transform _rightHand;
    public Transform RightHand
    {
        set
        {
            _rightHand = value;
        }
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _IKWeightLeftHand = 0;
        _IKWeightRightHand = 0;
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
        //parameters get value from animation curve
        _IKWeightRightHand = animator.GetFloat("LadderIKWeightRightHand");
        _IKWeightLeftHand = animator.GetFloat("LadderIKWeightLeftHand");

        GetClosestIKToHand(_leftHand);
        Debug.Log(_closestIKToHand.name);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, _closestIKToHand.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, _closestIKToHand.rotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _IKWeightLeftHand);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _IKWeightLeftHand);

        GetClosestIKToHand(_rightHand);
        Debug.Log(_closestIKToHand.name);
        animator.SetIKPosition(AvatarIKGoal.RightHand, _closestIKToHand.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, _closestIKToHand.rotation);

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _IKWeightRightHand);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _IKWeightRightHand);
    }

    private void GetClosestIKToHand(Transform hand)
    {
        if (_closestIKToHand == null)
            _closestIKToHand = _ladderIKHands[0];

        foreach (Transform handIK in _ladderIKHands)
        {
            if ((hand.position - handIK.position).sqrMagnitude < (hand.position - _closestIKToHand.position).sqrMagnitude)
            {
                _closestIKToHand = handIK;
            }
        }
    }
}
