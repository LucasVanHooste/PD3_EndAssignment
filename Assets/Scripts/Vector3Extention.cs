using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extention
{
    public static Vector3 TransformToHorizontalAxisVector(this Vector3 vector)
    {
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.z))
        {
            vector.x = vector.x / Mathf.Abs(vector.x);
            vector.z = 0;
        }
        else
        {
            vector.z = vector.z / Mathf.Abs(vector.z);
            vector.x = 0;
        }

        vector.y = 0;
        return vector;
    }
}
