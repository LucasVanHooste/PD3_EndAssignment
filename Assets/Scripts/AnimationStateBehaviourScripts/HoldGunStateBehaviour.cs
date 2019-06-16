using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldGunStateBehaviour : StateMachineBehaviour {

    public Transform Player { private get; set; }   
    public Transform Gun { private get; set; }
    public bool IsAiming { private get; set; }

    private float _iKWeight=0;
    private GunScript _gunScript;



    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Gun == null || Player == null) return;

        _gunScript = Gun.GetComponent<GunScript>();

        if (!_gunScript.IsTwoHanded && !IsAiming) return;

        _iKWeight = 1;


        if (_gunScript.RightHandIK)
        {
            animator.SetIKPosition(AvatarIKGoal.RightHand, _gunScript.RightHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, _gunScript.RightHandIK.rotation);
           

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);
        }

        if (_gunScript.LeftHandIK)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftHand, _gunScript.LeftHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, _gunScript.LeftHandIK.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _iKWeight);
        }

        if (_gunScript.LeftElbowIK)
        {
            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, _gunScript.LeftElbowIK.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1f);
        }
        if (_gunScript.RightElbowIK)
        {
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, _gunScript.RightElbowIK.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, .5f);
        }

    }

}
