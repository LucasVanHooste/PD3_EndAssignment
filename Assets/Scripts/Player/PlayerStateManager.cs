using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager
{
    private List<IState> _states = new List<IState>();

    private PlayerMotor _playerMotor;
    private PlayerController _playerController;
    private AnimationsController _animationsController;

    public PlayerStateManager(PlayerMotor playerMotor, PlayerController playerController, AnimationsController animationsController)
    {
        _playerMotor = playerMotor;
        _playerController = playerController;
        _animationsController = animationsController;

        CreateStates();
    }

    private void CreateStates()
    {
        _states.Add(new NormalState(_playerMotor, _playerController, _animationsController));
        _states.Add(new PushingState(_playerMotor, _playerController, _animationsController));
        _states.Add(new GunState(_playerMotor, _playerController, _animationsController));
        _states.Add(new DeadState(_playerMotor, _playerController, _animationsController));
        _states.Add(new ClimbingState(_playerMotor, _playerController, _animationsController));
        _states.Add(new TurretState(_playerMotor, _playerController, _animationsController));
    }

    public IState GetState<T>(IInteractable interactableObject) where T : IState
    {
        foreach(IState state in _states)
        {
            if(state is T)
            {
                state.ResetState(interactableObject);
                return state;
            }
        }

        throw new System.Exception("requested state is not in states list");
    }
}
