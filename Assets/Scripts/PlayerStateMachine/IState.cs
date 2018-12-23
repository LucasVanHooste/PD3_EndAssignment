using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState {
    void PickUpGun();
    void OnTriggerEnter(Collider other);
    //void OnTriggerStay(Collider other);
    void OnTriggerExit(Collider other);
    void OnControllerColliderHit(ControllerColliderHit hit);
    void Update();
}
