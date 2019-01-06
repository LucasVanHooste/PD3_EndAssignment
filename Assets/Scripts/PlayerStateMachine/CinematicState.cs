using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicState : PlayerState
{
    private Transform _playerTransform;
    private PhysicsController _physicsController;
    private PlayerController _playerController;
    private AnimationsController _animationsController;
    private List<Collider> _triggers;
    private GameObject _object;

    private CinematicBehaviour _cinematicBehaviour;

    public CinematicState(Transform playerTransform, PhysicsController physicsController, PlayerController playerController, AnimationsController animationsController, GameObject triggerObject, CinematicBehaviour cinematicBehaviour)
    {
        _playerTransform = playerTransform;
        _physicsController = physicsController;
        _playerController = playerController;
        _animationsController = animationsController;
        _object = triggerObject;
        _triggers = _playerController.Triggers;
        _cinematicBehaviour = cinematicBehaviour;

        if(_object.tag=="FirstGun")
        PickupFirstGun();
    }

    public override void Update()
    {

    }

    private void PickupFirstGun()
    {
        Debug.Log("Pick up gun");
        _physicsController.StopMoving();
        _physicsController.Aim = Vector3.zero;

        _playerController.StartCoroutine(RotateToObject());
    }

    private IEnumerator RotateToObject()
    {
        Vector3 direction = Vector3.Scale((_object.transform.position - _playerTransform.position), new Vector3(1, 0, 1));

        while (Quaternion.Angle(_playerTransform.rotation, Quaternion.LookRotation(direction)) > 1f)
        {
            Debug.Log("rotate");
            Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, direction, .05f, 0.0f);

            float angle = Vector3.SignedAngle(_playerTransform.forward, newDir, Vector3.up);
            _physicsController.Aim = new Vector3(Mathf.Rad2Deg * angle / _physicsController.HorizontalRotationSpeed, _physicsController.Aim.y, _physicsController.Aim.z);

            yield return null;
        }

        Debug.Log("finish rotation");
        _physicsController.Aim = Vector3.zero;
        yield return new WaitForSeconds(.2f);
        _playerController.StartCoroutine(PickUpFirstGun());
    }

    private IEnumerator PickUpFirstGun()
    {
        _animationsController.HoldGunIK.Gun=_object.transform;
        _cinematicBehaviour.PlayCinematicScene("PickUpFirstGun");
        yield return new WaitForSeconds(1);

        _animationsController.PickUpGun(true);

        yield return new WaitUntil(() => _cinematicBehaviour.IsSceneFinished);
        Debug.Log("cinematic finished");
        _animationsController.PickUpGun(false);
        _playerController.ToGunState(_object);
    }

    public override void PickUpGun()
    {
        if (_object.GetComponent<GunScript>())
        {
            GunScript _gunScript = _object.GetComponent<GunScript>();
                _gunScript.TakeFirstGun(_playerController.gameObject.layer, _playerController.RightHand, _playerController.CameraRoot);
        }

        RemoveTriggersFromList(_object.GetComponents<Collider>());
    }

    public void RemoveTriggersFromList(Collider[] colliders)
    {
        for (int i = colliders.Length - 1; i >= 0; i--)
        {
            if (colliders[i].isTrigger)
            {
                if (_triggers.Contains(colliders[i]))
                    _triggers.Remove(colliders[i]);
            }

        }
    }

}
