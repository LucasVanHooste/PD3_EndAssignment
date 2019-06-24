using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFlatStateBehaviour : StateMachineBehaviour
{
    public GunState PlayerGunHolder { private get; set; }
    public EnemyBehaviour EnemyGunHolder { private get; set; }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PlayerGunHolder!=null)
            PlayerGunHolder.DropGun();

        if (EnemyGunHolder != null)
            EnemyGunHolder.DropGun();
    }

    
}
