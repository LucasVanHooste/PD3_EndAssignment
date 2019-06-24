using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : BaseWeapon, IInteractable {

    [SerializeField]private Transform _horizontalAnchorTransform;
    [SerializeField] private Transform _verticalAnchorTransform;
    public Transform VerticalAnchorTransform { get => _verticalAnchorTransform; }

    [SerializeField] private Transform _leftHandIK;
    public Transform LeftHandIK {get => _leftHandIK; }

    [SerializeField] private Transform _rightHandIK;
    public Transform RightHandIK { get => _rightHandIK; }

    [SerializeField] private Transform _playerPosition;
    public Transform PlayerPosition { get => _playerPosition; }

    [SerializeField] private Transform _camDefaultTransform;
    public Transform CamDefaultTransform { get => _camDefaultTransform; }

    [SerializeField] private Transform _camAimingTransform;
    public Transform CamAimingTransform { get => _camAimingTransform; }

    [Space]
    [Header("Clamp Parameters")]
    [SerializeField] private float _minVerticalAngle;
    [SerializeField] private float _maxVerticalAngle;
    [SerializeField] private float _minHorizontalAngle;
    [SerializeField] private float _maxHorizontalAngle;

    private float _verticalRotation = 0;
    private float _horizontalRotation = 0;
	

    public Vector3 GetHorizontalDirection()
    {
        return _horizontalAnchorTransform.forward;
    }

    public void Rotate(float horizontalRotation, float verticalRotation)
    {
        //horizontal rotation
        _horizontalRotation += horizontalRotation; 

        _horizontalRotation = Mathf.Clamp(_horizontalRotation, _minHorizontalAngle, _maxHorizontalAngle); //clamp vertical rotation

        transform.localEulerAngles = new Vector3(transform.eulerAngles.x, _horizontalRotation, transform.eulerAngles.z);

        //vertical rotation
        _verticalRotation += verticalRotation; 

        _verticalRotation = Mathf.Clamp(_verticalRotation, _minVerticalAngle, _maxVerticalAngle); 

        _verticalAnchorTransform.eulerAngles = new Vector3(_verticalRotation, _verticalAnchorTransform.eulerAngles.y, _verticalAnchorTransform.eulerAngles.z);
    }

    public void Interact(IInteractor interactor)
    {
        interactor.TurretInteraction(this);
    }
}
