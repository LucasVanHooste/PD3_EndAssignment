using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {

    public Transform RightHandIK;
    public Transform LeftHandIK;
    public Transform LeftElbowIK;
    public Transform RightElbowIK;
    [SerializeField] private Transform _gunCollisionCheckerPrefab;
    private Transform _gunCollisionChecker;

    public Vector3 LocalPositionOnPlayer;
    public Vector3 LocalRotationOnPlayer;

    public Vector3 LocalAimPositionOnPlayer;
    public Vector3 LocalAimRotationOnPlayer;

    public bool IsTwoHanded;

    private Transform _transform;
    private Transform _cameraRootTransform;
    private Transform _rightHandTransform;

    [SerializeField] private float _fireRate;
    private float _fireTimer;

    [SerializeField] private int _bulletDamage;
    [SerializeField] private LayerMask _bulletLayermask;
    private Camera _cam;

    private void Start()
    {
        _transform = transform;
        _fireTimer = _fireRate;
        _cam = Camera.main;
        _gunCollisionChecker = GameObject.Instantiate(_gunCollisionCheckerPrefab, _transform.position, _transform.rotation);
    }

    private void Update()
    {
        if (_gunCollisionChecker!=null && _transform.parent == null)
        {
            _transform.position = _gunCollisionChecker.position;
            _transform.rotation = _gunCollisionChecker.rotation;
        }
    }

    public void TakeFirstGun(int layerIndex, Transform RightHand, Transform CameraRootTransform)
    {
        GameObject.Destroy(_gunCollisionChecker.gameObject);

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
        GameObject.Destroy(_gunCollisionChecker.gameObject);

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
        gameObject.layer = 14;
        _gunCollisionChecker = GameObject.Instantiate(_gunCollisionCheckerPrefab, _transform.position, _transform.rotation);
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

    public void ShootGun(bool isShooting)
    {
        if (isShooting)
        {
            if (_fireTimer >= _fireRate)
            {
                _fireTimer = 0;
                FireBullet();
            }
            else
            {
                _fireTimer += Time.deltaTime;
            }
        }
        else
        {
            _fireTimer = _fireRate;
        }
    }

    private void FireBullet()
    {
        Ray _centreScreenRay = _cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(_centreScreenRay, out hit,1000,_bulletLayermask))
            print("I'm looking at " + hit.transform.name);
        else
            print("I'm looking at nothing!");
    }
}
