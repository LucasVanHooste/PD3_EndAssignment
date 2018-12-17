using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CinematicScene
{
    public string _name;
    public Transform _cameraTransform;
    public Transform _targetPosition;
}

public class CinematicBehaviour : MonoBehaviour {

    private bool _isSceneFinished=true;

    [SerializeField] private Camera _camera;
    [SerializeField] private List<CinematicScene> _cinematicScenes;

    private bool _hasFinished = false;
    private CinematicScene _currentScene;

    private Vector3 _camStartPos;
    private Quaternion _camStartRotation;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator PlayCinematicScene(string sceneName)
    {
        _isSceneFinished = false;
        Debug.Log("Play cinematic");
        //_currentScene= _cinematicScenes[0];
        foreach (CinematicScene scene in _cinematicScenes)
        {
            if (scene._name == sceneName)
            {
                _currentScene = scene;
                break;
            }
        }

        _camStartPos = _currentScene._cameraTransform.position;
        _camStartRotation  = _currentScene._cameraTransform.rotation;

        yield return new WaitUntil(MoveCameraToTargetPosition);
        yield return new WaitForSeconds(4);
        yield return new WaitUntil(MoveCameraToStartPosition);
        _isSceneFinished = true;
    }

    private bool MoveCameraToTargetPosition()
    {
        Vector3 distance = _currentScene._cameraTransform.position - _currentScene._targetPosition.position;
        float angle = Quaternion.Angle(_currentScene._cameraTransform.rotation, _currentScene._targetPosition.rotation);

        while (distance.magnitude > .1f || Mathf.Abs(angle) > 2)
        {
            _currentScene._cameraTransform.position = Vector3.MoveTowards(_currentScene._cameraTransform.position, _currentScene._targetPosition.position, .1f);
            _currentScene._cameraTransform.rotation= Quaternion.RotateTowards(_currentScene._cameraTransform.rotation, _currentScene._targetPosition.rotation, 2.2f);
            Debug.Log("angle: " + Quaternion.Angle(_currentScene._cameraTransform.rotation, _currentScene._targetPosition.rotation));
            Debug.Log("cam: " + _currentScene._cameraTransform.eulerAngles);
            Debug.Log("tar: " + _currentScene._targetPosition.eulerAngles);
            return false;
        }

        return true;
    }

    private bool MoveCameraToStartPosition()
    {
        Vector3 distance = _currentScene._cameraTransform.position - _camStartPos;
        float angle = Quaternion.Angle(_currentScene._cameraTransform.rotation, _camStartRotation);

        while (distance.magnitude > .1f || Mathf.Abs(angle)>2)
        {
            _currentScene._cameraTransform.position = Vector3.MoveTowards(_currentScene._cameraTransform.position, _camStartPos, .1f);
            _currentScene._cameraTransform.rotation = Quaternion.RotateTowards(_currentScene._cameraTransform.rotation, _camStartRotation, 2f);
            Debug.Log("cam: " + _currentScene._cameraTransform.eulerAngles);
            Debug.Log("tar: " + _currentScene._targetPosition.eulerAngles);
            return false;
        }

        return true;
    }

    public bool GetIsSceneFinished()
    {
        return _isSceneFinished;
    }
}
