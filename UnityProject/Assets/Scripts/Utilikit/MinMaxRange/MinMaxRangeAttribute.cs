/* MinMaxRangeAttribute.cs
 * by Eddie Cameron - For the public domain
 * ----------------------------
 * Use a MinMaxRange class to replace twin float range values (eg: float minSpeed, maxSpeed; becomes MinMaxRange speed)
 * Apply a [MinMaxRange( minLimit, maxLimit )] attribute to a MinMaxRange instance to control the limits and to show a
 * slider in the inspector
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinMaxRangeAttribute : PropertyAttribute
{
    public float minLimit, maxLimit;

    public MinMaxRangeAttribute( float minLimit, float maxLimit )
    {
        this.minLimit = minLimit;
        this.maxLimit = maxLimit;
    }
}

[System.Serializable]
public class MinMaxRangeFloat
{
    public float rangeStart, rangeEnd;

    public float GetRandomValue()
    {
        return Random.Range( rangeStart, rangeEnd );
    }
}

[System.Serializable]
public class MinMaxRangeInt
{
    public int rangeStart, rangeEnd;

    /// <summary>
    /// Random between rangeStart and rangeEnd (INCLUSIVE)
    /// </summary>
    /// <returns>The random value.</returns>
    public int GetRandomValue()
    {
        return Random.Range( rangeStart, rangeEnd );
    }
}