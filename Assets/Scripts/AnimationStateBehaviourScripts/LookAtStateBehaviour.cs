using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtStateBehaviour : StateMachineBehaviour {

    public Transform LookAtPosition { private get; set; }


    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (LookAtPosition)
        {
            animator.SetLookAtPosition(LookAtPosition.position);
            animator.SetLookAtWeight(1);
        }

    }
}
