using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickUpGunBehaviour : StateMachineBehaviour {

    public Transform Player;
    public Transform PistolHandle;
    public Transform RightHand;

    private float _iKWeight;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PistolHandle = GameObject.Find("Pistol").transform.GetChild(0);
        Player = GameObject.Find("Player").transform;
        RightHand = GameObject.Find("mixamorig:RightHand").transform;
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
        Debug.Log(PistolHandle.gameObject.name);

        //before player grabs pistol
        if (stateInfo.normalizedTime < .14f)
            _iKWeight = Mathf.Lerp(_iKWeight, 1, .05f);
        else
        {
            //when player grabs pistol
            if (PistolHandle.parent.parent == null)
            {
                PistolHandle.parent.gameObject.layer = 9;
                PistolHandle.parent.position = RightHand.position;
                PistolHandle.parent.parent = RightHand;
                PistolHandle.parent.localEulerAngles = new Vector3(0, -90, -90);
            }

            //after player grabs pistol
            _iKWeight = Mathf.Lerp(_iKWeight, 0, .05f);
        }

        //IK
        PistolHandle.rotation = Player.rotation;
        animator.SetIKPosition(AvatarIKGoal.RightHand, PistolHandle.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, PistolHandle.rotation);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);


    }
}
