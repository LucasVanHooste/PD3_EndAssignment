using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour, IInteractable, IEnemyMovementActionTrigger {

    public Transform TakeOfPoint;
    public Transform TopLadderLeftHandIK;
    public Transform TopLadderRightHandIK;
    public Transform TopLadderLeftHandIK2;
    public Transform TopLadderRightHandIK2;

    public Transform[] BottomLadderIKHands;

    public bool IsPersonClimbing { get; set; }


    public void Interact(IInteractor interactor)
    {
        interactor.LadderInteraction(this);
    }

    public void TriggerAction(EnemyBehaviour enemy)
    {
        enemy.SwitchAction<EnemyClimbAction>(transform);
    }
}
