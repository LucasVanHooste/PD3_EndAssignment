using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : BaseWeapon, IInteractable {

    private Transform _transform;
    private Transform _cameraRootTransform;
    private Transform _rightHandTransform;

    public Transform RightHandIK;
    public Transform LeftHandIK;
    public Transform LeftElbowIK;
    public Transform RightElbowIK;


    [SerializeField] private Transform _gunCollisionCheckerPrefab;
    private Transform _gunCollisionChecker;
    private Collider _trigger;

    //I know this i wrong but didnt wan't to change it because I'd have to enter all that values again
    public Vector3 LocalPositionOnPlayer;
    public Vector3 LocalRotationOnPlayer;

    public Vector3 LocalAimPositionOnPlayer;
    public Vector3 LocalAimRotationOnPlayer;

    [SerializeField] private bool _isTwoHanded;
    public bool IsTwoHanded { get => _isTwoHanded; }


    protected override void Start()
    {
        base.Start();

        _transform = transform;
        _gunCollisionChecker = GameObject.Instantiate(_gunCollisionCheckerPrefab, _transform.position, _transform.rotation);
        _trigger = GetComponent<Collider>();
    }

    private void Update()
    {
        if ( _transform.parent == null)
        {
            _transform.SetPositionAndRotation(_gunCollisionChecker.position, _gunCollisionChecker.rotation);
        }
    }

    public void HoldGun(Transform RightHand, Transform gunAnchor)
    {
        _gunCollisionChecker.gameObject.SetActive(false);
        _trigger.enabled = false;


        if (IsTwoHanded)
        {
            _transform.parent = gunAnchor;
        }
        else
        {
            _transform.parent = RightHand;
        }
        _transform.localPosition = LocalPositionOnPlayer;
        _transform.localEulerAngles = LocalRotationOnPlayer;

        _rightHandTransform = RightHand;
        _cameraRootTransform = gunAnchor;
    }

    public void DropGun()
    {
        _transform.parent = null;
        _trigger.enabled = true;

        _gunCollisionChecker.SetPositionAndRotation(_transform.position, _transform.rotation);
        _gunCollisionChecker.gameObject.SetActive(true);
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
