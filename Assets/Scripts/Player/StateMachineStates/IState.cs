using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState {
    void OnStateEnter();
    void OnStateExit();
    void OnTriggerEnter(Collider other);
    void OnTriggerExit(Collider other);
    void OnControllerColliderHit(ControllerColliderHit hit);
    void Update();
    void Die();
}
