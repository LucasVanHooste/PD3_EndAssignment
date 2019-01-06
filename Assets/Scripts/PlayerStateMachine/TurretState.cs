using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretState : PlayerState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;

    private GameObject _object;
    private CameraController _cameraController;
    private GameObject _crossHair;

    private GameObject _turret;
    private TurretScript _turretScript;

    private bool _isAiming = false;
    private bool _isShooting = false;
    private bool _isReady = false;

    float _turretPaddingDistance = 0.15f;

    public TurretState(Transform playerTransform, PhysicsController physicsController, PlayerController playerController, AnimationsController animationsController, GameObject turret,
        CameraController cameraController, GameObject crossHair)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;

        _cameraController = cameraController;
        _crossHair = crossHair;

        _turret = turret;
        _turretScript = _turret.GetComponent<TurretScript>();

        GoToTurret();
    }

    public override void Update()
    {
        if (!_isReady) return;

        if (Input.GetAxis("TriggerLeft") > 0.2f && _physicsController.IsGrounded())
        {
            //Debug.Log("Aim");
            _isAiming = true;
        }
        else _isAiming = false;

        AimTurret();

        if (Input.GetAxis("TriggerRight") > 0.2f && _isAiming)
        {
            //Debug.Log("Shoot");
            _isShooting = true;
        }
        else _isShooting = false;

        FireTurret();


        if (Input.GetButton("Interact"))
        {
            OnStateExit();
            _playerController.ToNormalState();
            return;
        }

        RotateTurret(Input.GetAxis("RightJoystickX"), Input.GetAxis("RightJoystickY"));
        FollowTurret();
    }

    private void GoToTurret()
    {
        _physicsController.Aim = Vector3.zero;
        _physicsController.StopMoving();

        _playerController.StartCoroutine(RotateToTurret());

    }

    private IEnumerator RotateToTurret()
    {
        Vector3 direction = Vector3.Scale((_turret.transform.position - _playerTransform.position), new Vector3(1, 0, 1));

        while (Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(direction)) >1f)
        {
            //direction = Vector3.Scale((_ladder.transform.position - _playerTransform.position), new Vector3(1, 0, 1));
            Debug.Log("rotate1");
            Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, direction, .05f, 0.0f);

            float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
            _physicsController.Aim = new Vector3(Mathf.Rad2Deg * angle / _physicsController.HorizontalRotationSpeed, _physicsController.Aim.y, _physicsController.Aim.z);
            yield return null;
        }

        Debug.Log("finish rotation1");
        _physicsController.Aim = Vector3.zero;
        _playerController.StartCoroutine(MoveToTurret());
    }

    private IEnumerator MoveToTurret()
    {
        SetTurretIK(_turretScript);

        Vector3 turretPosition = new Vector3(_turretScript.PlayerPosition.position.x, _playerTransform.position.y, _turretScript.PlayerPosition.position.z);
        while (Vector3.Scale(turretPosition - _playerTransform.position, new Vector3(1, 0, 1)).magnitude > _turretPaddingDistance)
        {
            Vector3 lerp = Vector3.Lerp(_playerTransform.position, turretPosition, .8f) - _playerTransform.position;
            _physicsController.Movement = _playerTransform.InverseTransformVector(new Vector3(lerp.x, 0, lerp.z));
            yield return null;
        }

        _physicsController.StopMoving();
        //ClimbLadder();
        _playerController.StartCoroutine(RotateToTurretDirection());
    }

    private IEnumerator RotateToTurretDirection()
    {
        Vector3 direction = _turretScript.GetHorizontalDirection();

        while (Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(direction)) > 1f)
        {
            Debug.Log("rotate2");
            Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, direction, .05f, 0.0f);

            float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
            _physicsController.Aim = new Vector3(Mathf.Rad2Deg*angle/_physicsController.HorizontalRotationSpeed, _physicsController.Aim.y, _physicsController.Aim.z);

            yield return null;
        }

        Debug.Log("finish rotation2");
        _physicsController.Aim = Vector3.zero;
        yield return new WaitForSeconds(.2f);
        UseTurret();
        //ClimbLadder();
    }

    private void SetTurretIK(TurretScript turretScript)
    {
        _animationsController.TurretIK.Turretscript=turretScript;
    }

    private void UseTurret()
    {
        _isReady = true;
        _cameraController.HoldTurret(_turretScript.CamDefaultTransform, _turretScript.CamAimingTransform, _turretScript.VerticalAnchorTransform);
    }

    private void ClimbLadder()
    {
        _animationsController.ApplyRootMotion(true);
        _physicsController.HasGravity(false);
        _animationsController.Climb(true);
    }

    private void FireTurret()
    {
        _turretScript.FireTurret(_isShooting);
    }

    private void AimTurret()
    {
            _physicsController.IsWalking = _isAiming;

            _crossHair.SetActive(_isAiming);
            _cameraController.AimTurret(_isAiming);
    }

    private void RotateTurret(float horizontalInput, float verticalInput)
    {
        if (!_isAiming)
            _turretScript.Rotate(horizontalInput * _physicsController.HorizontalRotationSpeed * Time.deltaTime, verticalInput * _physicsController.VerticalRotationSpeed * Time.deltaTime);
        else
            _turretScript.Rotate(horizontalInput * _physicsController.HorizontalRotationSpeed/2 * Time.deltaTime, verticalInput * _physicsController.VerticalRotationSpeed/2 * Time.deltaTime);

    }

    private void FollowTurret()
    {
        _playerTransform.rotation = Quaternion.LookRotation(_turretScript.GetHorizontalDirection(), Vector3.up);

        Vector3 turretPosition = new Vector3(_turretScript.PlayerPosition.position.x, _playerTransform.position.y, _turretScript.PlayerPosition.position.z);
        _physicsController.SetPosition(turretPosition);

        //Vector3 lerp = Vector3.Lerp(_playerTransform.position, turretPosition, 1.6f) - _playerTransform.position;
        //Vector3 direction = turretPosition - _playerTransform.position;
        //_physicsController.Movement = _playerTransform.InverseTransformVector(lerp * 2f);
    }

    private void OnStateExit()
    {
        SetTurretIK(null);
        _cameraController.HoldTurret(null, null, null);
        _isAiming = false;
        AimTurret();
    }

    public override void Die()
    {
        OnStateExit();
    }
}
