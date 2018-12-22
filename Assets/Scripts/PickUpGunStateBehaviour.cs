using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickUpGunStateBehaviour : StateMachineBehaviour {

    public Transform Player;
    public Transform Gun;
    public Transform RightHand;
    public Transform LeftHand;

    private float _iKWeight;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _iKWeight = 0;
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

        //before player grabs pistol
        if (stateInfo.normalizedTime < .14f)
            _iKWeight = Mathf.Lerp(_iKWeight, 1, .05f);
        else
        {
            //when player grabs pistol
            if (Gun.parent == null)
            {
                Gun.gameObject.layer = 9;
                Gun.position = RightHand.position;
                Gun.parent = RightHand;
                Gun.localEulerAngles = new Vector3(0, -90, -90);
            }

            //after player grabs pistol
            _iKWeight = Mathf.Lerp(_iKWeight, 0, .05f);
        }

        //IK
        Gun.GetComponent<GunScript>().RightHand.rotation = Player.rotation;
        animator.SetIKPosition(AvatarIKGoal.RightHand, Gun.GetComponent<GunScript>().RightHand.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, Gun.GetComponent<GunScript>().RightHand.rotation);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);


    }
}
