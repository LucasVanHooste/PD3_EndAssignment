using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCollisionCheckerScript : MonoBehaviour{

    public bool HasCollided { get; private set; }
    public bool HasGravity { get; private set; }

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
                HasCollided = true;
            else
            {
                HasGravity = true;
                GameObject.Destroy(other.gameObject);
            }

        }

    }
}
