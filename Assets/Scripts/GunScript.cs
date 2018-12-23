using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {

    public Transform RightHandIK;
    public Transform LeftHandIK;
    public Transform LeftElbowIK;
    public Transform RightElbowIK;

    public Vector3 LocalPositionOnPlayer;
    public Vector3 LocalRotationOnPlayer;

    public Vector3 LocalAimPositionOnPlayer;
    public Vector3 LocalAimRotationOnPlayer;

    public bool IsTwoHanded;

    private Transform _transform;
    private Transform _cameraRootTransform;
    private Transform _rightHandTransform;

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (_transform.parent)
        {
            //_transform.localPosition = LocalPositionOnPlayer;
            //_transform.localEulerAngles = LocalRotationOnPlayer;
        }
    }

    public void TakeFirstGun(int layerIndex, Transform RightHand, Transform CameraRootTransform)
    {
        gameObject.layer = 9;
        gameObject.tag = "Gun";
        //_transform.parent = parent;
        if (IsTwoHanded)
        {
            _transform.parent = CameraRootTransform;
        }
        else
        {
            _transform.parent = RightHand;
        }
        _transform.localPosition = LocalPositionOnPlayer;
        _transform.localEulerAngles = LocalRotationOnPlayer;

        _rightHandTransform = RightHand;
        _cameraRootTransform = CameraRootTransform;
    }

    public void TakeGun(int layerIndex, Transform RightHand, Transform CameraRootTransform/*, HoldGunStateBehaviour holdGunIK*/)
    {
        gameObject.layer = 9;
        if (IsTwoHanded)
        {
            _transform.parent = CameraRootTransform;
        }
        else
        {
            _transform.parent = RightHand;
        }

        _transform.localPosition = LocalPositionOnPlayer;
        _transform.localEulerAngles = LocalRotationOnPlayer;
        //holdGunIK.SetGun(_transform);

        _rightHandTransform = RightHand;
        _cameraRootTransform = CameraRootTransform;
    }

    public void DropGun()
    {
        _transform.parent = null;
        gameObject.layer = 0;
    }

    public void AimGun(bool isAiming)
    {
        if (isAiming)
        {
            if(!IsTwoHanded)
                _transform.parent = _cameraRootTransform;
            _transform.localPosition = Vector3.Lerp(_transform.localPosition, LocalAimPositionOnPlayer, .2f);
            _transform.localRotation = Quaternion.Lerp(_transform.localRotation, Quaternion.Euler(LocalAimRotationOnPlayer), .2f);
        }
        else
        {
            if (!IsTwoHanded)
                _transform.parent = _rightHandTransform;
            _transform.localPosition = Vector3.Lerp(_transform.localPosition, LocalPositionOnPlayer, .2f);
            _transform.localRotation = Quaternion.Lerp(_transform.localRotation, Quaternion.Euler(LocalRotationOnPlayer), .2f);
        }
    }
}
