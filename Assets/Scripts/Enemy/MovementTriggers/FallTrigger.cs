using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTrigger : MonoBehaviour, IEnemyMovementActionTrigger
{
    public void TriggerAction(EnemyBehaviour enemy)
    {
        enemy.SwitchAction<EnemyFallAction>(transform);
    }
}
