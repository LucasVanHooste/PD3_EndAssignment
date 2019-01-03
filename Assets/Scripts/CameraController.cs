using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _cameraRootTransform;
    [SerializeField] private Transform _cameraDefaultTransform;
    [SerializeField] private Transform _camaraAimTransform;

    private Transform _turretDefaultTransform = null;
    private Transform _turretAimTransform = null;

    [SerializeField] private float _camRotation;
    [SerializeField] private float _minCamAngle;
    [SerializeField] private float _maxCamAngle;


    private bool _isAimingGun = false;
    private bool _isAimingTurret = false;
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if(_turretAimTransform!=null && _turretDefaultTransform != null)
        {
            if (_isAimingTurret)
            {
                _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _turretAimTransform.position, .2f);
                _cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, _turretAimTransform.rotation, .2f);
            }
            else
            {
                _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _turretDefaultTransform.position, .2f);
                _cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, _turretDefaultTransform.rotation, .2f);
            }
            return;
        } 

        if (_isAimingGun)
        {
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _camaraAimTransform.position, .2f);
            _cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, _camaraAimTransform.rotation, .2f);
        }
        else
        {
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _cameraDefaultTransform.position, .2f);
            _cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, _cameraDefaultTransform.rotation, .2f);
        }
    }

    public void RotateVertically(float angle)
    {
        //vertical rotation
        _camRotation += angle * Time.deltaTime; //get vertical rotation

        _camRotation = Mathf.Clamp(_camRotation, _minCamAngle, _maxCamAngle); //clamp vertical rotation

        _cameraRootTransform.eulerAngles = new Vector3(_camRotation, _cameraRootTransform.eulerAngles.y, _cameraRootTransform.eulerAngles.z);
    }

    public void AimGun(bool isAiming)
    {
        _isAimingGun = isAiming;
    }

    public void HoldTurret(Transform turretDefaultTransform, Transform turretAimTransform, Transform turretRootTransform)
    {
        _turretDefaultTransform = turretDefaultTransform;
        _turretAimTransform = turretAimTransform;

        if (_turretAimTransform == null || _turretDefaultTransform == null || turretRootTransform==null)
        {
            _cameraTransform.parent = _cameraRootTransform;
        }
        else
        {
            _cameraTransform.parent = turretRootTransform;
        }
    }

    public void AimTurret(bool isAiming)
    {
        _isAimingTurret = isAiming;
    }
}
