using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObstacleScript : MonoBehaviour {

    public readonly float ObstacleWidth = 2;

    public GameObject ObstacleCollisionCheckerPrefab;
    public Transform LeftHandIK;
    public Transform RightHandIK;

    public float ObstacleHeightPadding;
    public float ObstaclePadding;

    private Rigidbody _rigidbody;

	// Use this for initialization
	void Start () {
        _rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //public void SetVelocity(Vector3 velocity)
    //{
    //    _rigidbody.velocity = velocity;
    //}

    public void SetConstraints(RigidbodyConstraints constraints)
    {
        _rigidbody.constraints = constraints;
    }

    public void UseGravity(bool gravity)
    {
        _rigidbody.useGravity = gravity;
    }


}
