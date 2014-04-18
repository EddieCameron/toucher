/* SafeEvents.cs
 * ----------------------------
 * Copyright Eddie Cameron & Grasshopper NYC, 2013 (MIT licenced)
 * ----------------------------
 * A collection of methods used to raise events safely.
 * Events raised the conventional way will stop if any listeners throw an exception, so
 * listener functions aren't guaranteed to execute.
 * Using these methods will not only check for null events for you, but will catch exceptions 
 * to make sure all listener functios are run
 * 
 * Useage - Instead of raising an event like:
 * if ( someEvent != null )
 *      someEvent();
 * Raise with:
 * SafeEvents.SafeRaise( someEvent );
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public static class SafeEvents
{
    public static void SafeRaise( Action safeEvent )
    {
        if ( safeEvent != null )
        {
            Action eventCopy;
            lock ( safeEvent )
            {
                eventCopy = safeEvent;
            }

            foreach ( Action subscriber in eventCopy.GetInvocationList() )
            {
                try
                {
                    subscriber();
                }
                catch ( System.Exception e )
                {
                    DebugExtras.LogError( e );
                }
            }
        }
    }

    public static void SafeRaise<T>( Action<T> safeEvent, T arg )
    {
        if ( safeEvent != null )
        {
            Action<T> eventCopy;
            lock ( safeEvent )
            {
                eventCopy = safeEvent;
            }

            foreach ( Action<T> subscriber in eventCopy.GetInvocationList() )
            {
                try
                {
                    subscriber( arg );
                }
                catch ( System.Exception e )
                {
                    DebugExtras.LogError( e );
                }
            }
        }
    }

    public static void SafeRaise<T1, T2>( Action<T1, T2> safeEvent, T1 arg1, T2 arg2 )
    {

        if ( safeEvent != null )
        {
            Action<T1, T2> eventCopy;
            lock ( safeEvent )
            {
                eventCopy = safeEvent;
            }

            foreach ( Action<T1, T2> subscriber in eventCopy.GetInvocationList() )
            {
                try
                {
                    subscriber( arg1, arg2 );
                }
                catch ( System.Exception e )
                {
                    DebugExtras.LogError( e );
                }
            }
        }
    }

    public static void SafeRaise<T1, T2, T3>( Action<T1, T2, T3> safeEvent, T1 arg1, T2 arg2, T3 arg3 )
    {
        if ( safeEvent != null )
        {
            Action<T1, T2, T3> eventCopy;
            lock ( safeEvent )
            {
                eventCopy = safeEvent;
            }

            foreach ( Action<T1, T2, T3> subscriber in eventCopy.GetInvocationList() )
            {
                try
                {
                    subscriber( arg1, arg2, arg3 );
                }
                catch ( System.Exception e )
                {
                    DebugExtras.LogError( e );
                }
            }
        }
    }

    public static void SafeRaise<T1, T2, T3, T4>( Action<T1, T2, T3, T4> safeEvent, T1 arg1, T2 arg2, T3 arg3, T4 arg4 )
    {
        if ( safeEvent != null )
        {
            Action<T1, T2, T3, T4> eventCopy;
            lock ( safeEvent )
            {
                eventCopy = safeEvent;
            }

            foreach ( Action<T1, T2, T3, T4> subscriber in eventCopy.GetInvocationList() )
            {
                try
                {
                    subscriber( arg1, arg2, arg3, arg4 );
                }
                catch ( System.Exception e )
                {
                    DebugExtras.LogError( e );
                }
            }
        }
    }
}