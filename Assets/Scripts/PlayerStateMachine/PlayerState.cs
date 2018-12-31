using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : IState
{
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

    public virtual void PickUpGun()
    {
        
    }

    public abstract void Update();
}
