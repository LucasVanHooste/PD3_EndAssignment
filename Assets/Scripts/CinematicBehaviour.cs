using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CinematicScene
{
    public string Name;
    public Transform StartPosition;
    public Transform TargetPosition;
    public float TimeInSeconds;
}

public class CinematicBehaviour : MonoBehaviour {

    private bool _isSceneFinished=true;
    public bool IsSceneFinished
    {
        get
        {
            return _isSceneFinished;

        }
    }

    [SerializeField] private CameraController _cameraController;
    [SerializeField] private List<CinematicScene> _cinematicScenes;

    private CinematicScene _currentScene;

    public void PlayCinematicScene(string sceneName)
    {
        _isSceneFinished = false;

        //get scene from list
        foreach (CinematicScene scene in _cinematicScenes)
        {
            if (scene.Name == sceneName)
            {
                _currentScene = scene;
                break;
            }
        }

        _cameraController.PauseUpdate=true;
        _cameraController.SetPositionAndRotation(_currentScene.StartPosition.position, _currentScene.StartPosition.rotation);
        StartCoroutine(MoveCameraToTargetPosition());
    }

    private IEnumerator MoveCameraToTargetPosition()
    {
        //while cam isnt in position, lerp

        while ((_cameraController.CameraPosition - _currentScene.TargetPosition.position).magnitude > .1f || 
            Mathf.Abs(Quaternion.Angle(_cameraController.CameraRotation, _currentScene.TargetPosition.rotation)) > 2)
        {
            LerpCameraToPosition(_currentScene.TargetPosition.position, _currentScene.TargetPosition.rotation);
            yield return null;
        }

        yield return new WaitForSeconds(_currentScene.TimeInSeconds);
        StartCoroutine(MoveCameraToStartPosition());
    }

    private IEnumerator MoveCameraToStartPosition()
    {
        //while cam isnt in position, lerp
        while ((_currentScene.StartPosition.position - _cameraController.CameraPosition).magnitude > .1f || 
            Mathf.Abs(Quaternion.Angle(_cameraController.CameraRotation, _currentScene.StartPosition.rotation)) >2)
        {
            LerpCameraToPosition(_currentScene.StartPosition.position, _currentScene.StartPosition.rotation);
            yield return null;
        }

        _cameraController.PauseUpdate = false;
        _isSceneFinished = true;
    }

    private void LerpCameraToPosition(Vector3 position, Quaternion rotation)
    {
        _cameraController.MoveTowards(position, .1f);
        _cameraController.RotateTowards(rotation, 3f);
    }
}
