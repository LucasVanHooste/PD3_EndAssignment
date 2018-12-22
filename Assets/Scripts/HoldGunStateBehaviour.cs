using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldGunStateBehaviour : StateMachineBehaviour {

    public Transform Player;
    public Transform Gun;
    public Transform RightHand;
    public Transform LeftHand;

    private float _iKWeight;
    private GunScript _gunScript;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _iKWeight = 0;
        _gunScript = Gun.GetComponent<GunScript>();
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

    //OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK(inverse kinematics) should be implemented here.

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log(Gun.gameObject.name);

        _iKWeight = 1;

            //when player grabs pistol
        if (Gun.parent == null)
        {
            if (_gunScript.IsTwoHanded)
            {
                _gunScript.TakeGun(Player.gameObject.layer, Player);
            }
            else
            {
                _gunScript.TakeGun(Player.gameObject.layer, RightHand);
                //Gun.gameObject.layer = 9;
                //Gun.position = RightHand.position;
                //Gun.parent = RightHand;
                //Gun.localEulerAngles = new Vector3(0, -90, -90);
            }

        }

        //IK
        _gunScript.RightHand.rotation = Player.rotation;
        animator.SetIKPosition(AvatarIKGoal.RightHand, Gun.GetComponent<GunScript>().RightHand.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, Gun.GetComponent<GunScript>().RightHand.rotation);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);

        if (_gunScript.IsTwoHanded)
        {
            _gunScript.LeftHand.rotation = Player.rotation;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, Gun.GetComponent<GunScript>().LeftHand.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, Gun.GetComponent<GunScript>().LeftHand.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _iKWeight);
        }
    }
}
