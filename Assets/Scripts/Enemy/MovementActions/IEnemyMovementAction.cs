using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMovementAction {

    void OnActionEnter();
    void Stop();
    void OnTriggerExit(Collider other);
    void OnCollisionEnter(Collision collision);
    void ResetAction(Transform actionTrigger);
}
