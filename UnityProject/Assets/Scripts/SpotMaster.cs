/* SpotMaster.cs
 * Copyright Grasshopper 2013
 * ----------------------------
 *
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpotMaster : StaticBehaviour<SpotMaster> 
{
    public ColourSpot spotPrefab;
    public bool randomSpot;

    float nextSpotMove;
    Vector2 fakeSpotPos;

    const int MAX_SPOTS = 10;
    const float sendFrameTime = .04f;    // how often move update messages are sent
    const float maxSpotSpeed = 10f;

    static Camera cam;

    static Dictionary<string, SpotInfo> spots = new Dictionary<string, SpotInfo>();
    static List<int> curTouches = new List<int>();

    protected override void Awake()
    {
        base.Awake();

        cam = Camera.main;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

#if UNITY_EDITOR || UNITY_DESKTOP
        if ( Input.GetMouseButton( 0 ) )
        {
            Vector2 screenPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
            OnTouch( 0, screenPos );
        }
        if ( Input.GetMouseButtonUp( 0 ) )
        {
            OnTouchEnd( 0 );
        }
#else
        // check for touches
        var newTouches = new List<int>();
        foreach ( var touch in Input.touches )
        {
            curTouches.Remove( touch.fingerId );
            newTouches.Add( touch.fingerId );

            OnTouch( touch.fingerId, touch.position );
        }
        
        // remove missing touches
        foreach ( var oldTouch in curTouches )
            OnTouchEnd( oldTouch );

        curTouches = newTouches;
#endif

        // bot touch
        if ( randomSpot )
        {
            if ( Time.time > nextSpotMove )
            {
                fakeSpotPos = new Vector2( Random.value * Screen.width, Random.value * Screen.height );
                nextSpotMove = Time.time + Random.Range( 1f, 5f );
            }

            OnTouch( -1, fakeSpotPos );
        }

        var timedOut = new List<string>();
        foreach ( var spot in spots.Values )
        {
            if ( Time.time - spot.lastUpdate > 2f )
                timedOut.Add( spot.id );
            else
                spot.spot.transform.position = Vector3.MoveTowards( spot.spot.transform.position, spot.targetPos, maxSpotSpeed * Time.deltaTime ); // smoothly move all touches to new positions
        }
       
        //actually removed timed out spots (can't update spots while enumerating through it)
        foreach ( var spot in timedOut )
            RemoveSpot( spot );
    }

    public static void UpdateSpot( string id, Vector2 screenPos )
    {
        SpotInfo spot;
        if ( spots.TryGetValue( id, out spot ) )
        {
            // update existing spot
            Vector3 newPos = cam.ScreenToWorldPoint( screenPos );
            newPos.z = spot.spot.transform.position.z;
            spot.targetPos = newPos;
            spots[id] = spot;
        }
        else
        {
            // add new spot!
            var spotDist = Random.Range( 1f, 10f );
            Vector3 screenPosDepth = new Vector3( screenPos.x, screenPos.y, spotDist );  // spread out depth
            var spotPos = cam.ScreenToWorldPoint( screenPosDepth );

            var newSpot = (ColourSpot)Instantiate( instance.spotPrefab, spotPos, Quaternion.identity );
            SpotInfo spotInfo;
            spotInfo.id = id;
            spotInfo.spot = newSpot;
            spotInfo.lastUpdate = Time.time;
            spotInfo.targetPos = spotPos;
            spots.Add( id, spotInfo );

            // TODo remove oldest if too many
        }
    }

    public static void RemoveSpot( string id )
    {
        SpotInfo spot;
        if ( spots.TryGetValue( id, out spot ) )
        {
            spot.spot.Remove();
            spots.Remove( id );
        }
        else
            DebugExtras.LogWarning( "Tried to remove spot that doesn't exist " + id );
    }

    static void OnTouch( int fingerID, Vector2 touchScreenPos )
    {
        string spotID = NetworkStuff.userID + fingerID;
        
        UpdateSpot( spotID, touchScreenPos );

        SpotInfo spot;
        if ( spots.TryGetValue( spotID, out spot ) )
        {
            //local spot
            if ( Time.time < spot.lastUpdate + sendFrameTime )
                return; // dont' send too often
         
            spot.lastUpdate = Time.time;
            spots[spotID] = spot;
        }

        NetworkStuff.UpdateSpot( cam.ScreenToViewportPoint( touchScreenPos ), fingerID );
    }

    static void OnTouchEnd( int fingerID )
    {
        string spotID = NetworkStuff.userID + fingerID;
        RemoveSpot( spotID );

        NetworkStuff.RemoveSpot( fingerID );
    }

    static void OnLeftServer()
    {
        var spotIDs = spots.Keys;
        foreach ( var id in spotIDs )
            RemoveSpot( id );
    }

    struct SpotInfo
    {
        public ColourSpot spot;
        public string id;
        public float lastUpdate;
        public Vector3 targetPos;
    }
}