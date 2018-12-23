using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _cameraRootTransform;
    [SerializeField] private Transform _cameraDefaultTransform;
    [SerializeField] private Transform _camaraAimTransform;

    [SerializeField] private float _camRotation;
    [SerializeField] private float _minCamAngle;
    [SerializeField] private float _maxCamAngle;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
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
        if (isAiming)
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
}
