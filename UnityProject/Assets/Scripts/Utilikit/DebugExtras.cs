/* DebugExtras.cs
 * ----------------------------
 * Copyright Eddie Cameron & Grasshopper NYC 2013 (MIT licenced)
 * ----------------------------
 * Extends the Unity Debug class to compile and execute conditionally. Stack traces can easily eat up performance otherwise
    */

using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public static class DebugExtras 
{
    /// <summary>
    /// For other classes to check at runtime whether it is in debug mode
    /// </summary>
    /// <value><c>true</c> if debug build; otherwise, <c>false</c>.</value>
	public static bool debugBuild
	{
		get
		{
			bool debug = false;
#if DEBUG
    		debug = true;
#endif
			return debug;
		}
	}

    [Conditional( "DEBUG" )]
    public static void Assert( bool isTrue, string assertMsg = null )
    {
        if ( !isTrue )
            UnityEngine.Debug.LogError( "Assert failed " + assertMsg ?? "" );
    }
    
    [Conditional( "DEBUG" )]
    public static void Log( object toLog, Object context = null)
    {
        UnityEngine.Debug.Log( toLog + "\n-----------------------", context );
    }
    
    [Conditional( "DEBUG" )]
    public static void LogWarning( object toLog, Object context = null)
    {
        UnityEngine.Debug.LogWarning( toLog + "\n-----------------------", context );
    }
    
    [Conditional( "DEBUG" )]
    public static void LogError( object toLog, Object context = null)
    {
        UnityEngine.Debug.LogError( toLog + "\n-----------------------", context );
    }
}
