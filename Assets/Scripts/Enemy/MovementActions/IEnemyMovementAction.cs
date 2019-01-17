using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMovementAction {

    void Stop();
    void OnTriggerExit(Collider other);
    void OnCollisionEnter(Collision collision);
}
