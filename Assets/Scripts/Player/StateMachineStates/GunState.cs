using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunState : BasePlayerState
{
    private Transform _playerTransform;
    private PlayerMotor _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private List<Collider> _triggers;
    private GameObject _closestGameObject;
    private CameraController _cameraController;
    private Transform _holsterGun1Hand;
    private Transform _holsterGun2Hands;
    private GameObject _crossHair;
    private LayerMask _bulletLayerMask;

    private bool _isAiming;
    private bool _isFiring;
    private GunScript _gunScript;

    private float _dropGunTime = 1;
    private float _dropGunTimer=0;
    private float _punchCoolDownTimer = 0;

    public GunState(PlayerMotor physicsController, PlayerController playerController, AnimationsController animationsController, GunScript gun, 
        CameraController cameraController)
    {
        _playerTransform = PlayerController.PlayerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _triggers = _playerController.Triggers;
        _cameraController = cameraController;
        _holsterGun1Hand = _playerController.HolsterGun1Hand;
        _holsterGun2Hands = _playerController.HolsterGun2Hands;
        _crossHair = _playerController.CrossHair;
        _bulletLayerMask = _playerController.BulletLayerMask;
        _playerController.GunAnchor.rotation = _cameraController.CameraRoot.rotation;

        if (gun == null)
        {
            TakeGunFromHolster();
        }
        else
        {
            HoldGun(gun);
        }

        _punchCoolDownTimer = _playerController.PunchCoolDown;
    }

    public override void OnStateEnter()
    {
        if (_gunScript == null)
        {
            Debug.Log("to normal please");
            _playerController.SwitchState(_playerController.GetNormalState());
        }
    }

    public override void OnStateExit()
    {
        
    }

    public override void Update()
    {
        _playerController.GunAnchor.rotation = _cameraController.CameraRoot.rotation;

        if (Input.GetButtonDown("Jump") && _physicsController.IsGrounded)
        {
            _physicsController.Jump = true;
        }

        if (_physicsController.IsGrounded)
            _physicsController.Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _physicsController.Aim = new Vector3(Input.GetAxis("RightJoystickX"), 0, Input.GetAxis("RightJoystickY"));

        _isAiming = Input.GetAxis("TriggerLeft") > 0.2f && _physicsController.IsGrounded;
        AimGun();

        _isFiring = Input.GetAxis("TriggerRight") > 0.2f && _isAiming;
        FireGun();

        if (Input.GetButtonDown("Punch") && !_gunScript.IsTwoHanded && !_isAiming)
        {
            if (_punchCoolDownTimer >= _playerController.PunchCoolDown)
            {
                Punch();
                _punchCoolDownTimer = 0;
            }

        }
        _punchCoolDownTimer += Time.deltaTime;
        if (Input.GetButtonDown("Interact") && _physicsController.IsGrounded && !_isAiming)
        {
            InteractWithObject();
        }

        if (Input.GetButton("Interact"))
        {
            _dropGunTimer += Time.deltaTime;
        }
        else
        {
            _dropGunTimer = 0;
        }

        if (_dropGunTimer >= _dropGunTime)
        {
            DropGun();
        }
        //bad
        if (_physicsController.Velocity.y < -10f)
            DropGun();

        if(Input.GetButtonDown("HolsterGun") && !_isAiming)
        {
            HolsterGun();
        }
    }

    private void InteractWithObject()
    {
        Debug.Log(_triggers.Count);

        if (_triggers.Count <= 0) return;

        _closestGameObject = _playerController.GetClosestTriggerObject();
        IInteractable interactable = _closestGameObject.GetComponent<IInteractable>();

        switch (interactable)
        {
            case ObstacleScript obstacle:
                {
                    if (GetGunInHolster() == null)
                    {
                        HolsterGun();
                        _playerController.SwitchState(_playerController.GetPushingState(obstacle));
                    }
                }break;
            case GunScript gun:
                {
                    _gunScript.DropGun();

                    HoldGun(gun);
                    _playerController.RemoveTriggersFromList(_closestGameObject.GetComponents<Collider>());
                    _gunScript = gun;
                }
                break;
            case LadderScript ladder:
                {
                    if (GetGunInHolster() == null)
                    {
                        if (!ladder.IsPersonClimbing)
                        {
                            HolsterGun();
                            _playerController.SwitchState(_playerController.GetClimbingState(ladder));
                        }                          
                    }
                }
                break;
            case TurretScript turret:
                {
                    if (GetGunInHolster() == null)
                    {
                        HolsterGun();
                        _playerController.SwitchState(_playerController.GetTurretState(turret));
                    }
                }
                break;
        }
        
    }

    private void Punch()
    {
        _animationsController.Punch();
        RaycastHit hit;
        if (Physics.Raycast(_playerTransform.position + new Vector3(0, 1.5f, 0), _playerTransform.forward, out hit, _playerController.PunchRange))
        {
            if (hit.transform.gameObject.layer == 16)
            {
                if (hit.transform.GetComponent<EnemyBehaviour>())
                    hit.transform.GetComponent<EnemyBehaviour>().TakeDamage(_playerController.PunchDamage, _playerTransform.position);
            }
        }
    }

    void AimGun()
    {
        _physicsController.IsWalking=_isAiming;

        _animationsController.AimGun(_isAiming);
        _animationsController.IsTwoHandedGun(_gunScript.IsTwoHanded);

        _crossHair.SetActive(_isAiming);
        _cameraController.IsAiming=_isAiming;
        _gunScript.AimGun(_isAiming);
    }

    private void FireGun()
    {
        Ray _centerOfScreenRay = _cameraController.PlayerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));

        _gunScript.FireWeapon(_isFiring, _centerOfScreenRay, _bulletLayerMask);
    }

    

    private void HoldGun(GunScript gunScript)
    {
        if (gunScript)
        {
            _gunScript = gunScript;
            _gunScript.HoldGun(_playerController.RightHand, _playerController.GunAnchor);

            _animationsController.HoldGunIK.Gun = _gunScript.transform;
        }
    }

    public void DropGun()
    {
        _isAiming = false;
        AimGun();

        _gunScript.DropGun();

        _animationsController.HoldGunIK.Gun=null;

        _playerController.SwitchState(_playerController.GetNormalState());
    }

    private void HolsterGun()
    {

        GameObject tempGun = GetGunInHolster();

        if (_gunScript.IsTwoHanded)
        {
            _gunScript.transform.parent = _holsterGun2Hands;
            _gunScript.transform.position = _holsterGun2Hands.position;
            _gunScript.transform.rotation = _holsterGun2Hands.rotation;
        }
        else
        {
            _gunScript.transform.parent = _holsterGun1Hand;
            _gunScript.transform.position = _holsterGun1Hand.position;
            _gunScript.transform.rotation = _holsterGun1Hand.rotation;
        }

        _animationsController.HoldGunIK.Gun=null;


        if (tempGun!=null)
        {
            HoldGun(tempGun.GetComponent<GunScript>());
        }
        else
        {
            _playerController.SwitchState(_playerController.GetNormalState());
        }

    }

    private GameObject GetGunInHolster()
    {
        GameObject tempGun = null;

        if (_holsterGun1Hand.childCount > 0)
            tempGun = _holsterGun1Hand.GetChild(0).gameObject;

        if (_holsterGun2Hands.childCount > 0)
            tempGun = _holsterGun2Hands.GetChild(0).gameObject;

        return tempGun;
    }

    private void TakeGunFromHolster()
    {
        GameObject gunInHolster = GetGunInHolster();

        if (gunInHolster)
        HoldGun(gunInHolster.GetComponent<GunScript>());
    }

    public override void Die()
    {
        DropGun();
    }


}
