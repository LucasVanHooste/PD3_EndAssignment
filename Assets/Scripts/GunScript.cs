using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {

    public Transform RightHand;
    public Transform LeftHand;
    public Vector3 LocalPositionOnPlayer;
    public Vector3 LocalRotationOnPlayer;

    public bool IsTwoHanded;

    public void TakeGun(int layerIndex, Transform parent)
    {
        gameObject.layer = 9;
        transform.parent = parent;
        transform.localPosition = LocalPositionOnPlayer;
        transform.localEulerAngles = LocalRotationOnPlayer;
    }

    public void DropGun()
    {

    }
}
