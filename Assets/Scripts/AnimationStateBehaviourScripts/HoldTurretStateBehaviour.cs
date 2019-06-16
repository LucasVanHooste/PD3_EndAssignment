using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldTurretStateBehaviour : StateMachineBehaviour {

    public TurretScript Turretscript { private get; set; }

    private float _iKWeight = 0;


    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Turretscript == null)
        {
            _iKWeight = 0;
            return;
        }

        _iKWeight +=Time.deltaTime*2f;


        if (Turretscript.RightHandIK)
        {
            animator.SetIKPosition(AvatarIKGoal.RightHand, Turretscript.RightHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, Turretscript.RightHandIK.rotation);


            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);
        }

        if (Turretscript.LeftHandIK)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftHand, Turretscript.LeftHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, Turretscript.LeftHandIK.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _iKWeight);
        }
    }
}
