using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretState : BasePlayerState
{
    private Transform _playerTransform;
    private PlayerMotor _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;

    private GameObject _object;
    private CameraController _cameraController;
    private GameObject _crossHair;
    private LayerMask _bulletLayerMask;

    private TurretScript _turretScript;

    private bool _isAiming = false;
    private bool _isFiring = false;
    private bool _isReady = false;

    float _turretPaddingDistance = 0.15f;

    public TurretState(PlayerMotor physicsController, PlayerController playerController, AnimationsController animationsController)
    {
        _playerTransform = PlayerController.PlayerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;

        _cameraController = _playerController.cameraController;
        _crossHair = _playerController.CrossHair;
        _bulletLayerMask = _playerController.BulletLayerMask;
    }

    public override void ResetState(IInteractable turret)
    {
        _turretScript = (TurretScript)turret;

        _isAiming = false;
        _isFiring = false;
        _isReady = false;
}

    public override void OnStateEnter()
    {
        GoToTurret();
    }

    public override void OnStateExit()
    {
        SetTurretIK(null);
        _cameraController.ResetCameraAnchorAndPositions();
        _isAiming = false;
        AimTurret();
    }

    public override void Update()
    {
        if (!_isReady) return;

        _isAiming = InputController.LeftTrigger > 0.2f && _physicsController.IsGrounded;
        AimTurret();

        _isFiring = InputController.RightTrigger > 0.2f && _isAiming;
        FireTurret();

        if (InputController.InteractButtonDown)
        {
            _playerController.SwitchState<NormalState>();
            return;
        }

        RotateTurret(InputController.RightJoystickX, InputController.RightJoystickY);
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
        Vector3 direction = Vector3.Scale((_turretScript.transform.position - _playerTransform.position), new Vector3(1, 0, 1));

        while (Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(direction)) >1f)
        {
            Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, direction, .05f, 0.0f);

            float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
            _physicsController.Aim = new Vector3(Mathf.Rad2Deg * angle / _physicsController.HorizontalRotationSpeed, _physicsController.Aim.y, _physicsController.Aim.z);
            yield return null;
        }

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
        _playerController.StartCoroutine(RotateToTurretDirection());
    }

    private IEnumerator RotateToTurretDirection()
    {
        Vector3 direction = _turretScript.GetHorizontalDirection();

        while (Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(direction)) > 1f)
        {
            Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, direction, .05f, 0.0f);

            float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
            _physicsController.Aim = new Vector3(Mathf.Rad2Deg*angle/_physicsController.HorizontalRotationSpeed, _physicsController.Aim.y, _physicsController.Aim.z);

            yield return null;
        }

        _physicsController.Aim = Vector3.zero;
        yield return new WaitForSeconds(.2f);
        UseTurret();
    }

    private void SetTurretIK(TurretScript turretScript)
    {
        _animationsController.TurretIK.Turretscript=turretScript;
    }

    private void UseTurret()
    {
        _isReady = true;
        _cameraController.SetCameraAnchorAndPositions(_turretScript.VerticalAnchorTransform, _turretScript.CamDefaultTransform, _turretScript.CamAimingTransform);
    }

    private void FireTurret()
    {
        Ray centerOfScreenRay = _cameraController.PlayerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        _turretScript.FireWeapon(_isFiring, centerOfScreenRay, _bulletLayerMask);
    }

    private void AimTurret()
    {
            _physicsController.IsWalking = _isAiming;

            _crossHair.SetActive(_isAiming);
            _cameraController.IsAiming = _isAiming;
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
    }


    public override void Die()
    {
        OnStateExit();
    }


}
