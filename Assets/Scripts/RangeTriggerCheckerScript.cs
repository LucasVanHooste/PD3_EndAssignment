using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeTriggerCheckerScript : MonoBehaviour {

    private List<Collider> _triggers = new List<Collider>();
    private Transform _transform;

    private void Start()
    {
        _transform = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && !_triggers.Contains(other))
            _triggers.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_triggers.Contains(other))
            _triggers.Remove(other);
    }

    public GameObject GetClosestTriggerObjectWithTag(string tag)
    {
        Vector3 position = _transform.position;
        float distance = 100;
        GameObject closest = null;
        foreach (Collider col in _triggers)
        {
            if (!col.gameObject.CompareTag(tag)) continue;

            float tempDistance = Vector3.Magnitude(position - col.transform.position);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                closest = col.gameObject;
            }

        }
        return closest;
    }
}
