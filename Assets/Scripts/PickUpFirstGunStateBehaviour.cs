using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickUpFirstGunStateBehaviour : StateMachineBehaviour {

    private Transform _player;
    private Transform _gun;
    private Transform _rightHand;

    private float _iKWeight;
    private GunScript _gunScript;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _iKWeight = 0;
    }

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
        if (_gun != null)
            _gunScript = _gun.GetComponent<GunScript>();

        Debug.Log(_gun.gameObject.name);

        //before player grabs pistol
        //if (stateInfo.normalizedTime < .14f)
            _iKWeight = Mathf.Lerp(_iKWeight, 1, .02f);
        //else
        //{
        //    //when player grabs pistol
        //    if (_gun.parent == null)
        //    {
        //        _gunScript.TakeFirstGun(layerIndex, _rightHand, _player.GetComponent<PlayerController>().CameraRoot);
        //    }

        //    //after player grabs pistol
        //    _iKWeight = Mathf.Lerp(_iKWeight, 0, .05f);
        //}

        //IK
        if (_gun.parent != null && _gunScript.RightHandIK)
        {
            _gun.GetComponent<GunScript>().RightHandIK.rotation = _player.rotation;
            animator.SetIKPosition(AvatarIKGoal.RightHand, _gun.GetComponent<GunScript>().RightHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, _gun.GetComponent<GunScript>().RightHandIK.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _iKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _iKWeight);
        }

    }

    public void SetGun(Transform gun)
    {
        _gun = gun;
    }
    public void SetPlayer(Transform player)
    {
        _player = player;
        
    }
    public void SetRightHand(Transform rightHand)
    {
        _rightHand = rightHand;
    }
}
