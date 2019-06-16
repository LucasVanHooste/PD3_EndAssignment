using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObstacleStateBehaviour : StateMachineBehaviour {

    private Transform _leftHandIK, _rightHandIK;
    private float _iKWeight=0;


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _iKWeight = 0;
    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_leftHandIK==null || _rightHandIK==null) return;

        _iKWeight += Time.deltaTime * 1.5f;
        
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
