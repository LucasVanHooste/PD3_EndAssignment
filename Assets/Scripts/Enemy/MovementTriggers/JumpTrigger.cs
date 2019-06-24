using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger : MonoBehaviour, IEnemyMovementActionTrigger
{
    public void TriggerAction(EnemyBehaviour enemy)
    {
        enemy.SwitchAction<EnemyJumpAction>(transform);
    }
}
