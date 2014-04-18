/* BehaviourBase.cs
 * ----------------------------
 * Copyright Eddie Cameron & Grasshopper NYC 2013 (MIT licenced)
 * ----------------------------
 * Replacement for stock MonoBehaviour. To be used for all scripts
 * - Make sure to override (rather than hide with 'new') any unity methods like Start() or Update(), so 
 * that any future code here will be executed
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BehaviourBase : MonoBehaviour
{
    Transform _transform;

    public new Transform transform
    {
        get
        {
            if ( !_transform )
                _transform = GetComponent<Transform>();

            return _transform;
        }
    }

    Renderer _renderer;

    public new Renderer renderer
    {
        get
        {
            if ( !_renderer )
                _renderer = GetComponent<Renderer>();

            return _renderer;
        }
    }

    AudioSource _audioSource;

    public new AudioSource audio
    {
        get
        {
            if ( !_audioSource )
                _audioSource = GetComponent<AudioSource>();

            return _audioSource;
        }
    }

    Camera _camera;

    public new Camera camera
    {
        get
        {
            if ( !_camera )
                _camera = GetComponent<Camera>();

            return _camera;
        }
    }

    NetworkView _networkView;
    public new NetworkView networkView
    {
        get {
            if ( !_networkView )
                _networkView = GetComponent<NetworkView>();

            return _networkView;
        }
    }

    protected virtual void Awake()
    {
		
    }

    protected virtual void Start()
    {
		
    }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
  
    }

    protected virtual void Update()
    {
		
    }

    protected virtual void LateUpdate()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    /// <summary>
    /// Use to do stuff with a gradually changing value
    /// </summary>
    /// <param name="from">Value to start from</param>
    /// <param name="to">Value to end at</param>
    /// <param name="time">Time for value to change</param>
    /// <param name="lerpedFunction">Function that does something with T. Called each frame and given the updated value</param>
    /// <param name="finishFunction">Function called when change is finished</param>
    protected IEnumerator LerpValue( float from, float to, float time, System.Action<float> lerpedFunction, System.Action finishFunction = null )
    {
        float amount = 0;
        while ( amount < 1f )
        {
            yield return 0;
            amount += Time.deltaTime / time;
            lerpedFunction( Mathf.Lerp( from, to, amount ) );
        }

        if ( finishFunction != null )
            finishFunction();
    }
    
    /// <summary>
    /// Use to do stuff with a gradually changing vector
    /// </summary>
    /// <param name="from">Vector to start from</param>
    /// <param name="to">Vector to end at</param>
    /// <param name="time">Time for vector to change</param>
    /// <param name="lerpedFunction">Function that does something with vector. Called each frame and given the updated value of the vector</param>
    /// <param name="finishFunction">Function called when change is finished</param>
    protected IEnumerator LerpVector( Vector3 from, Vector3 to, float time, System.Action<Vector3> lerpedFunction, System.Action finishFunction = null )
    {
        float amount = 0;
        while ( amount < 1f )
        {
            yield return 0;
            amount += Time.deltaTime / time;
            lerpedFunction( Vector3.Lerp( from, to, amount ) );
        }

        if ( finishFunction != null )
            finishFunction();
    }

	protected IEnumerator LerpVectorActionTime( Vector3 from, Vector3 to, float time, System.Action<Vector3> lerpedFunction, System.Action finishFunction = null )
	{
        yield return StartCoroutine( LerpFunctionActionTime( time, f => lerpedFunction( Vector3.Lerp( from, to, f ) ) ) );

        if ( finishFunction != null )
            finishFunction();
	}

    protected IEnumerator LerpQuaternion( Quaternion from, Quaternion to, float time, System.Action<Quaternion> lerpedFunction, System.Action finishFunction = null )
    {
        float amount = 0;
        while ( amount < 1f )
        {
            yield return 0;
            amount += Time.deltaTime / time;
            lerpedFunction( Quaternion.Lerp( from, to, amount ) );
        }

        if ( finishFunction != null )
            finishFunction();
    }

    protected IEnumerator LerpFunctionRealTime( float time, System.Action<float> lerpResultFunction )
    {
        float amount = 0;
        while ( amount < 1f )
        {
            lerpResultFunction( amount );
            yield return 0;
            amount += Time.deltaTime / time;
        }

        lerpResultFunction( 1f );
    }

    protected IEnumerator LerpFunctionActionTime( float time, System.Action<float> lerpResultFunction )
    {
        float amount = 0;
        while ( amount < 1f )
        {
            lerpResultFunction( amount );
            yield return ActionTime.WaitOneActionFrame();
            amount += ActionTime.deltaTime / time;
        }

        lerpResultFunction( 1f );
    }
}

/// <summary>
/// Base class for all Singletons, T is the actual type
/// eg: public class MyClass : StaticBehaviour<MyClass> {...}
/// -------
/// Use when there will only be one instance of a script in the scene. Makes access to non-static variables from static methods easy (with instance.fieldName)
/// </summary>
public class StaticBehaviour<T> : BehaviourBase where T : BehaviourBase
{
    static T _instance;

    protected static T instance
    {
        get
        {
            if ( !_instance )
                UpdateInstance();
            return _instance;
        }
    }

    protected override void Awake()
    {
        if ( _instance )
        {
            DebugExtras.LogWarning( "Duplicate instance of " + GetType() + " found. Removing " + name );
            Destroy( this );
            return;
        }

        UpdateInstance();
		
        base.Awake();
    }

    static void UpdateInstance()
    {
        _instance = GameObject.FindObjectOfType( typeof( T ) ) as T;
        if ( !_instance )
        {
            DebugExtras.LogWarning( "No object of type : " + typeof( T ) + " found in scene. Creating" );
            _instance = new GameObject( typeof( T ).ToString() ).AddComponent<T>();
        }
    }
}

/// <summary>
/// Base class for a nullable type, so you can use  " if ( myClass ) {...} " to check for null
/// </summary>
public class Nullable
{
    public static implicit operator bool( Nullable me )
    {
        return me != null;
    }
}