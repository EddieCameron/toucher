/* Trigger.cs
 * ----------------------------
 * Copyright Eddie Cameron & Grasshopper NYC, 2013 (MIT licenced)
 * ----------------------------
 * Drop on a trigger collider to have it send events when specified layers enter/stay/exit.
 * Useful for triggers on child objects and for making dealing with layer mask problems far easier
 */

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Trigger : BehaviourBase
{
    public LayerMask layers; // Much faster, overrides tag string if set
    public bool saveCollidersWithin;
    public string tagToCheck = "None";

    bool checkNone;
    bool checkString;
    HashSet<Collider> within = new HashSet<Collider>();

    public event Action<Transform> triggerEntered;
    public event Action<Transform> triggerStay;
    public event Action<Transform> triggerLeft; 
    
    protected override void Start()
    {
        base.Start();

        if ( ~layers.value == 0 )
        {
            checkString = true;
            if ( tagToCheck == "None" )
                checkNone = true;
        }
    }

    public Collider[] GetCollidersWithin()
    {
        if ( !saveCollidersWithin )
        {
            DebugExtras.LogWarning( "Trigger not saving colliders" );
            return new Collider[0];
        }

        var colliders = new Collider[within.Count];
        within.CopyTo( colliders );
        return colliders;
    }

    public bool IsColliderWithin( Collider toTest )
    {
        return within.Contains( toTest );
    }

    public IEnumerable<Collider> GetCollidersEnumerable()
    {
        if ( !saveCollidersWithin )
        {
            DebugExtras.LogWarning( "Trigger not saving colliders" );
            yield break;
        }
        foreach ( var coll in within.EnumerateRemoveNull() )
            yield return coll;
    }
    
    void OnTriggerEnter( Collider other )
    {   
        if ( checkNone || ( checkString && other.CompareTag( tagToCheck ) ) || ( ( 1 << other.gameObject.layer ) & layers.value ) != 0 )
        {
            if ( saveCollidersWithin )
                within.AddUnique( other );

            if ( triggerEntered != null )
            {
                triggerEntered( other.transform );
            }
        }
    }
    
    void OnTriggerStay( Collider other )
    {
        // only for layers
        if ( checkNone || ( ( 1 << other.gameObject.layer ) & layers.value ) != 0 )
            if ( triggerStay != null ) triggerStay( other.transform );
    }
    
    void OnTriggerExit( Collider other )
    {
        if ( checkNone || ( checkString && other.CompareTag( tagToCheck ) ) || ( ( 1 << other.gameObject.layer ) & layers.value ) != 0 )
        {
            if ( saveCollidersWithin )
                within.Remove( other );

            if ( triggerLeft != null )
                triggerLeft( other.transform );
        }
    }
}
