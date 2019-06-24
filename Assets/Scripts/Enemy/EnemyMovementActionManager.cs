using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementActionManager
{
    private List<IEnemyMovementAction> _movementActions = new List<IEnemyMovementAction>();

    private EnemyBehaviour _enemyBehaviour;
    private EnemyMotor _enemyMotor;
    private AnimationsController _animationsController;

    public EnemyMovementActionManager(EnemyBehaviour enemyBehaviour, EnemyMotor enemyMotor, AnimationsController animationsController)
    {
        _enemyBehaviour = enemyBehaviour;
        _enemyMotor = enemyMotor;
        _animationsController = animationsController;

        CreateActions();
    }

    private void CreateActions()
    {
        _movementActions.Add(new EnemyJumpAction(_enemyMotor, _enemyBehaviour));
        _movementActions.Add(new EnemyFallAction(_enemyMotor, _enemyBehaviour));
        _movementActions.Add(new EnemyClimbAction(_enemyMotor, _enemyBehaviour, _animationsController));
    }

    public IEnemyMovementAction GetMovementAction<T>(Transform actionTrigger) where T : IEnemyMovementAction
    {
        foreach (IEnemyMovementAction action in _movementActions)
        {
            if (action is T)
            {
                action.ResetAction(actionTrigger);
                return action;
            }
        }

        throw new System.Exception("requested action is not in actions list");
    }
}
