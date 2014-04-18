/* Network.cs
 * Copyright Grasshopper 2013
 * ----------------------------
 *
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkStuff : StaticBehaviour<NetworkStuff> 
{
    public const string GameID = "touch-5ndpucui1k2jut2hedatya";

    public static string userID;

    protected override void Awake()
    {
        base.Awake();

        userID = "user" + SystemInfo.deviceUniqueIdentifier;
    }

    protected override void Start()
    {
        base.Start();

        NetworkConnector.Connect( "http://eddiecameron-touchnode.jit.su" );
        NetworkConnector.AddMessageHandler( "message", OnMessage );
    }

    public static void UpdateSpot( Vector3 pos, int id )
    {
        if ( NetworkConnector.isConnected )
            NetworkConnector.SendSocketMessage( "message", new object[] { "updateSpot", userID + id, pos.x, pos.y } );
    }

    public static void RemoveSpot( int id )
    {
        if ( NetworkConnector.isConnected )
            NetworkConnector.SendSocketMessage( "message", new object[] { "removeSpot", userID + id } );
    }

    static void OnMessage( string eventType, SocketIOClient.Messages.JsonEncodedEventMessage msg )
    {
        object[] args = msg.args;
        DebugExtras.Log( "Received " + eventType + " message of length " + args.Length );
        if ( args.Length > 0 )
        {
            string messageType = args[0] as string;
            DebugExtras.Log( messageType );
            string id;
            switch( messageType )
            {
            case "updateSpot":
                DebugExtras.Assert( args.Length == 4 );
                id = (string)args[1];
                Vector2 screenPos;
                screenPos.x = (float)args[2];
                screenPos.y = (float)args[3];

                SpotMaster.UpdateSpot( id, screenPos );

                break;
            case "removeSpot":
                DebugExtras.Assert( args.Length == 2 );
                id = (string)args[1];

                SpotMaster.RemoveSpot( id );

                break;
            }
        }
    }

    [RPC]
    void _UpdateSpot( string id, Vector3 pos )
    {
        pos.x *= Screen.width;
        pos.y *= Screen.height;
        SpotMaster.UpdateSpot( id, pos );
    }

    [RPC]
    void _RemoveSpot( string id )
    {
        SpotMaster.RemoveSpot( id );
    }
}