using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayerState : IState
{
    //protected PlayerMotor _playerMotor;
    //protected PlayerController _playerController;
    //protected AnimationsController _animationsController;
    //protected IInteractable _interactableObject;

    //protected BasePlayerState(PlayerMotor playerMotor, PlayerController playerController, AnimationsController animationsController, IInteractable interactableObject)
    //{
    //    _playerMotor = playerMotor;
    //    _playerController = playerController;
    //    _animationsController = animationsController;
    //    _interactableObject = interactableObject;
    //}

    public virtual void Die()
    {
        
    }

    public virtual void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        
    }

    public virtual void OnTriggerExit(Collider other)
    {
        
    }

    public abstract void Update();
    public abstract void OnStateEnter();
    public abstract void OnStateExit();

    public abstract void ResetState(IInteractable interactable);
}
