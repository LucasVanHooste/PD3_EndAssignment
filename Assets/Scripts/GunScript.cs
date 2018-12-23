using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {

    public Transform RightHand;
    public Transform LeftHand;
    public Transform LeftElbow;
    public Transform RightElbow;
    public Vector3 LocalPositionOnPlayer;
    public Vector3 LocalRotationOnPlayer;

    public bool IsTwoHanded;

    private Transform _transform;

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (_transform.parent)
        {
            _transform.localPosition = LocalPositionOnPlayer;
            _transform.localEulerAngles = LocalRotationOnPlayer;
        }
    }

    public void TakeFirstGun(int layerIndex, Transform parent)
    {
        gameObject.layer = 9;
        _transform.parent = parent;
        _transform.localPosition = LocalPositionOnPlayer;
        _transform.localEulerAngles = LocalRotationOnPlayer;
    }

    public void TakeGun(int layerIndex, Transform parent, HoldGunStateBehaviour holdGunIK)
    {
        gameObject.layer = 9;
        _transform.parent = parent;
        _transform.localPosition = LocalPositionOnPlayer;
        _transform.localEulerAngles = LocalRotationOnPlayer;
        holdGunIK.SetGun(_transform);
    }

    public void DropGun()
    {
        _transform.parent = null;
        gameObject.layer = 0;
        gameObject.tag = "Gun";
    }
}
