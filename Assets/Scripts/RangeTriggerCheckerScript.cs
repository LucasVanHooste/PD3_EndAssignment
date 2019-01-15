using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeTriggerCheckerScript : MonoBehaviour {

    private List<Collider> _triggers = new List<Collider>();

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
        Vector3 position = transform.position;
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
