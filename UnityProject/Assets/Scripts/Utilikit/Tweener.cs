/* Tweener.cs
 * ----------------------------
 * Copyright Eddie Cameron & Grasshopper NYC, 2013 (MIT licenced)
 * ----------------------------
 * Does simple tweening.
 * Move/Rotate to target in given time. Sends events on completion.
 * 
 * Can add to object and control directly, or just use the static methods
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tweener : BehaviourBase
{
    struct TweenInfo
    {
        public float tweenTime;
        public Space space;
        public Quaternion targetRot;
        public Vector3 targetPos;

        public TweenInfo( float tweenTime, Space space )
        {
            this.tweenTime = tweenTime;
            this.space = space;

            this.targetRot = Quaternion.identity;
            this.targetPos = Vector3.zero;
        }
    }

    [HideInInspector]
    public bool destroyOnComplete;

    bool rotating, moving;

    public event System.Action FinishedMove;
    public event System.Action FinishedRotation;

    #region Manager
    static Dictionary<GameObject, Tweener> sceneTweeners = new Dictionary<GameObject, Tweener>(); // tweeners made at runtime

    public static Tweener MoveObjectTo( GameObject objToMove, Vector3 location, float inTime, Space space = Space.Self )
    {
        Tweener tweener;
        if ( !sceneTweeners.TryGetValue( objToMove, out tweener ) )
            tweener = objToMove.AddComponent<Tweener>();
        tweener.MoveTo( location, inTime, space );

        return tweener;
    }

    public static Tweener MoveObjectTo( Component objToMove, Vector3 location, float inTime, Space space = Space.Self )
    {
        return MoveObjectTo( objToMove.gameObject, location, inTime, space );
    }
    
    public static void StopMove( GameObject onObj )
    {
        Tweener tweener;
        if ( sceneTweeners.TryGetValue( onObj, out tweener ) )
            tweener.StopMove();
        else
            Debug.Log( "No tween on object: " + onObj.name ); 
    }

    public static Tweener RotateObjectTo( GameObject objToRotate, Quaternion rotation, float inTime, Space space = Space.Self )
    {
        Tweener tweener;
        if ( !sceneTweeners.TryGetValue( objToRotate, out tweener ) )
            tweener = objToRotate.AddComponent<Tweener>();

        tweener.RotateTo( rotation, inTime, space );

        return tweener;
    }

    public static Tweener RotateObjectTo( Component objToRotate, Quaternion rotation, float inTime, Space space = Space.Self )
    {
        return RotateObjectTo( objToRotate.gameObject, rotation, inTime, space );
    }

    public static void StopRotation( GameObject onObj )
    {
        Tweener tweener;
        if ( sceneTweeners.TryGetValue( onObj, out tweener ) )
            tweener.StopRotation();
        else
            Debug.Log( "No tween on object: " + onObj.name ); 
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        if ( !sceneTweeners.TryAdd( gameObject, this ) )
        {
            DebugExtras.LogWarning( "Gameobject already has a tweener on it. Destroying. " + name );
            Destroy( this );
        }
    }

    void OnDestroy()
    {
        sceneTweeners.Remove( gameObject );
    }

    /// <summary>
    /// Move object to given location in given time
    /// Replaces any current movements
    /// </summary>
    /// <param name="location">Location.</param>
    /// <param name="inTime">In time.</param>
    /// <param name="space">World or local space (default = local)</param>
    public void MoveTo( Vector3 location, float inTime, Space space = Space.Self )
    {
        StopCoroutine( "Move" );
        var tweenInfo = new TweenInfo( inTime, space );
        tweenInfo.targetPos = location;
        StartCoroutine( "Move", tweenInfo );
    }

    /// <summary>
    /// Stop any current movement. Finish event not raised
    /// </summary>
    public void StopMove()
    {
        StopCoroutine( "Move" );
        if ( moving )
        {
            moving = false;
            FinishedAction();
        }
    }

    IEnumerator Move( TweenInfo tween )
    {
        moving = true;
        var startPos = tween.space == Space.Self ? transform.localPosition : transform.position;

        float amount = 0;
        while ( amount < 1f )
        {
            amount += Time.deltaTime / tween.tweenTime;
            var moveTo = Vector3.Lerp( startPos, tween.targetPos, amount );
            if ( tween.space == Space.Self )
                transform.localPosition = moveTo;
            else
                transform.position = moveTo;
            if ( amount >= 1f )
                break;

            yield return 0;
        }

        if ( FinishedMove != null )
            FinishedMove();

        moving = false;
        FinishedAction();
    }
    
    /// <summary>
    /// Rotate object to given rotation in given time
    /// Replaces any current rotations
    /// </summary>
    /// <param name="rotation">Rotation.</param>
    /// <param name="inTime">In time.</param>
    /// <param name="space">World or local space (default = local)</param>
    public void RotateTo( Quaternion rotation, float inTime, Space space = Space.Self )
    {
        StopCoroutine( "Rotate" );
        var tweenInfo = new TweenInfo( inTime, space );
        tweenInfo.targetRot = rotation;
        StartCoroutine( "Rotate", tweenInfo );
    }

    /// <summary>
    /// Stop any current rotation. Finish event not raised
    /// </summary>
    public void StopRotation()
    {
        StopCoroutine( "Rotate" );
        if ( rotating )
        {
            rotating = false;
            FinishedAction();
        }
    }

    IEnumerator Rotate( TweenInfo tween )
    {
        rotating = true;
        var startRot = tween.space == Space.Self ? transform.localRotation : transform.rotation;

        float amount = 0;
        while ( amount < 1f )
        {
            amount += Time.deltaTime / tween.tweenTime;
            var rotateTo = Quaternion.Lerp( startRot, tween.targetRot, amount );
            if ( tween.space == Space.Self )
                transform.localRotation = rotateTo;
            else
                transform.rotation = rotateTo;
            if ( amount >= 1f )
                break;

            yield return 0;
        }

        if ( FinishedRotation != null )
            FinishedRotation();

        rotating = false;
        FinishedAction();
    }

    void FinishedAction()
    {
        if ( !rotating && !moving && destroyOnComplete )
            Destroy( this );
    }
}
                     
