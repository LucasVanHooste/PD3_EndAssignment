using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCollisionCheckerScript : MonoBehaviour {

    private bool _hasCollided=false;
    private bool _hasGravity = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("GravityBox"))
                _hasCollided = true;
            else
            {
                _hasGravity = true;
                GameObject.Destroy(other.gameObject);
            }

        }

    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (!other.isTrigger)
    //        _hasCollided = false;
    //}

    public bool GetHasCollided()
    {
        return _hasCollided;
    }

    public bool GetHasGravity()
    {
        return _hasGravity;
    }
}
