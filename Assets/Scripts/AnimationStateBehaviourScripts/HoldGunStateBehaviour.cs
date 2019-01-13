using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldGunStateBehaviour : StateMachineBehaviour {

    private Transform _player;
    public Transform Player
    {
        set
        {
            _player = value;
        }
    }
    private Transform _gun;
    public Transform Gun
    {
        set
        {
            _gun = value;
        }
    }
    private bool _isAiming = false;
    public bool IsAiming
    {
        set
        {
            _isAiming=value;
        }
    }

    private float _iKWeight=0;
    private GunScript _gunScript;


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
        if (_gun == null || _player == null) return;
        //Debug.Log(_gun.gameObject.name);

            _gunScript = _gun.GetComponent<GunScript>();

        //IK first pistol
        if (_gun.tag == "FirstGun")
        {
            _iKWeight = animator.GetFloat("IKWeight");


            animator.SetIKPosition(AvatarIKGoal.RightHand, _gun.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, _player.rotation);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);
        }

        if (!_gunScript.IsTwoHanded && !_isAiming) return;

        _iKWeight = 1;

        //IK all other guns
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
