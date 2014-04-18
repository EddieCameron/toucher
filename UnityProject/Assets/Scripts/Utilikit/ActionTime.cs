/* ActionTime.cs
 * Copyright Eddie Cameron & Grasshopper NYC 2013 (MIT licenced)
 * ----------------------------
 * Pauseable time implimentation, use instead of Time
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionTime : StaticBehaviour<ActionTime>
{
    public static bool isPaused { get; private set; }
    public static float deltaTime { get; private set; }
    public static float time { get; private set; }

    static float _timeScale = 1f;
    public static float timeScale
    {
        get
        {
            return _timeScale;
        }
        set
        {
            if ( _timeScale != value )
            {
                if ( value == 0 )
                    Pause();
                else if ( isPaused )
                    UnPause( value );
                else
                    _timeScale = value;
            }
        }
    }

    public static event System.Action Paused;
    public static event System.Action UnPaused;

    protected override void LateUpdate()
    {
        base.LateUpdate();

        deltaTime = Time.deltaTime * timeScale;
        time += deltaTime;
    }

    public static void Pause()
    {
        if ( !isPaused )
        {
            isPaused = true;
            _timeScale = 0;
            if ( Paused != null )
                Paused();
        }
    }

    public static void UnPause( float toTimeScale = 1f )
    {
        if ( isPaused )
        {
            isPaused = false;
            _timeScale = toTimeScale;
            if ( UnPaused != null )
                UnPaused();
        }
    }

    public static Coroutine WaitOneActionFrame()
    {
        return instance.StartCoroutine( _WaitOneActionFrame() );
    }

    static IEnumerator _WaitOneActionFrame()
    {
        do
        {
            yield return 0;
        } while ( isPaused );
    }

    public static Coroutine WaitForActionSeconds( float secondsToWait )
    {
        return instance.StartCoroutine( _WaitForActionSeconds( secondsToWait ) );
    }

    static IEnumerator _WaitForActionSeconds( float secondsToWait )
    {
        while ( secondsToWait > 0 )
        {
            if ( !isPaused )
                secondsToWait -= Time.deltaTime;

            yield return 0;
        }
    }
}