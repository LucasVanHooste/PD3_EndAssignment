using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {

    public Transform RightHandIK;
    public Transform LeftHandIK;
    public Transform LeftElbowIK;
    public Transform RightElbowIK;
    [SerializeField] private GameObject _fireGunEffect;
    private Coroutine _showFireEffect;
    [SerializeField] private Transform _gunCollisionCheckerPrefab;
    private Transform _gunCollisionChecker;
    private Collider _trigger;

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
    [SerializeField] private LayerMask _playerBulletLayermask;
    [SerializeField] private LayerMask _enemyBulletLayermask;

    private void Start()
    {
        _transform = transform;
        _fireTimer = _fireRate;
        _gunCollisionChecker = GameObject.Instantiate(_gunCollisionCheckerPrefab, _transform.position, _transform.rotation);
        _trigger = GetComponent<Collider>();
    }

    private void Update()
    {
        if (_gunCollisionChecker!=null && _transform.parent == null)
        {
            _transform.position = _gunCollisionChecker.position;
            _transform.rotation = _gunCollisionChecker.rotation;
        }
    }

    public void TakeFirstGun(Transform RightHand, Transform CameraRootTransform)
    {
        GameObject.Destroy(_gunCollisionChecker.gameObject);

        _trigger.enabled = false;
        //gameObject.layer = layerIndex;
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

    public void TakeGun(Transform RightHand, Transform CameraRootTransform/*, HoldGunStateBehaviour holdGunIK*/)
    {
        if(_gunCollisionChecker)
        GameObject.Destroy(_gunCollisionChecker.gameObject);

        _trigger.enabled = false;
        //gameObject.layer = layerIndex;

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
        _trigger.enabled = true;
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

    public void PlayerFireGun(bool isShooting, Camera cam)
    {
        if (isShooting)
        {
            if (_fireTimer >= _fireRate)
            {
                _fireTimer = 0;
                PlayerFireBullet(cam);
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

    public void EnemyFireGun(bool isShooting, Vector3 origin, Vector3 direction)
    {
        if (isShooting)
        {
            if (_fireTimer >= _fireRate)
            {
                _fireTimer = 0;
                EnemyFireBullet(origin, direction);
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

    private void PlayerFireBullet(Camera cam)
    {
        Ray _centreScreenRay = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(_centreScreenRay, out hit, 1000, _playerBulletLayermask))
        {
            print("I'm looking at " + hit.transform.name);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (hit.transform.GetComponent<EnemyBehaviour>())
                    hit.transform.GetComponent<EnemyBehaviour>().GetShot(_bulletDamage,hit.point);
            }

        }
        else
            print("I'm looking at nothing!");

        //fire effect
        PlayEffect();
    }

    private void EnemyFireBullet(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, 1000, _enemyBulletLayermask))
        {
            print("I'm looking at " + hit.transform.name);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (hit.transform.GetComponent<PlayerController>())
                    hit.transform.GetComponent<PlayerController>().GetShot(_bulletDamage, hit.point);
            }

        }
        else
            print("I'm looking at nothing!");

        //fire effect
        PlayEffect();
    }

    private void PlayEffect()
    {
        if (_showFireEffect == null)
            _showFireEffect = StartCoroutine(ShowFireEffect());
        else
        {
            StopCoroutine(_showFireEffect);
            _showFireEffect = StartCoroutine(ShowFireEffect());
        }
    }

    private IEnumerator ShowFireEffect()
    {
        _fireGunEffect.SetActive(true);
        yield return new WaitForSeconds(.1f);
        _fireGunEffect.SetActive(false);
    }
}
