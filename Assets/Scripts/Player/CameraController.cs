using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _cameraStartRootTransform;
    [SerializeField] private Transform _cameraStartDefaultPosition;
    [SerializeField] private Transform _cameraStartAimPosition;

    [Space]
    [Header("Camera Clamping Parameters")]
    [SerializeField] private float _camRotation;
    [SerializeField] private float _minCamAngle;
    [SerializeField] private float _maxCamAngle;

    public Camera PlayerCamera { get => _playerCamera; }
    public Transform CameraRoot { get => _cameraStartRootTransform; }
    public Vector3 CameraPosition { get => _cameraTransform.position; }
    public Quaternion CameraRotation { get => _cameraTransform.rotation; }


    private Transform _cameraDefaultPosition;
    private Transform _cameraAimPosition;

    public bool IsAiming {private get; set; }


    // Use this for initialization
    void Start () {
        _cameraDefaultPosition = _cameraStartDefaultPosition;
        _cameraAimPosition = _cameraStartAimPosition;
	}
	
	// Update is called once per frame
	void Update () {

        if (IsAiming)
        {
            Vector3 position = Vector3.Lerp(_cameraTransform.position, _cameraAimPosition.position, .2f);
            Quaternion rotation = Quaternion.Lerp(_cameraTransform.rotation, _cameraAimPosition.rotation, .2f);

            _cameraTransform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            Vector3 position = Vector3.Lerp(_cameraTransform.position, _cameraDefaultPosition.position, .2f);
            Quaternion rotation = Quaternion.Lerp(_cameraTransform.rotation, _cameraDefaultPosition.rotation, .2f);

            _cameraTransform.SetPositionAndRotation(position, rotation);
        }            

    }

    public void RotateVertically(float angle)
    {
        _camRotation += angle;

        _camRotation = Mathf.Clamp(_camRotation, _minCamAngle, _maxCamAngle);

        _cameraStartRootTransform.eulerAngles = new Vector3(_camRotation, _cameraStartRootTransform.eulerAngles.y, _cameraStartRootTransform.eulerAngles.z);
    }

    public void SetCameraAnchorAndPositions(Transform cameraAnchor, Transform defaultPosition, Transform aimPosition)
    {
        _cameraTransform.parent = cameraAnchor;
        _cameraDefaultPosition = defaultPosition;
        _cameraAimPosition = aimPosition;
    }

    public void ResetCameraAnchorAndPositions()
    {
        _cameraTransform.parent = _cameraStartRootTransform;
        _cameraDefaultPosition = _cameraStartDefaultPosition;
        _cameraAimPosition = _cameraStartAimPosition;
    }

}
