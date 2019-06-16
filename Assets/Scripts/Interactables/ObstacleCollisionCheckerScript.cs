using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCollisionCheckerScript : MonoBehaviour, IInteractable {

    private bool _hasCollided=false;
    private bool _hasGravity = false;
    private int _gravityBoxLayer;

    private void Start()
    {
        _gravityBoxLayer = LayerMask.NameToLayer("GravityBox");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if (other.gameObject.layer != _gravityBoxLayer)
                _hasCollided = true;
            else
            {
                _hasGravity = true;
                GameObject.Destroy(other.gameObject);
            }

        }

    }

    public bool GetHasCollided()
    {
        return _hasCollided;
    }

    public bool GetHasGravity()
    {
        return _hasGravity;
    }
}
