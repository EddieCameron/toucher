/* NetworkConnector.cs
 * Copyright Grasshopper 2013
 * ----------------------------
 *
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient;

public class NetworkConnector : StaticBehaviour<NetworkConnector> 
{
    static Client socket;

    public static bool isConnected { get { return socket.IsConnected; } }

    public delegate void SocketMessageHandler( string eventType, SocketIOClient.Messages.JsonEncodedEventMessage json );

    public static void Connect( string url )
    {
        socket = new SocketIOClient.Client( url );
        socket.On( "connect", ( fn ) =>
        {
            Debug.Log( "connect - socket" );

            socket.Emit( "login", NetworkStuff.userID );
        } );
        socket.Error += ( sender, e ) =>
        {
            DebugExtras.LogWarning( "socket Error: " + e.Message.ToString() );
        };
        socket.Connect();
    }

    void OnApplicationQuit()
    {
        socket.Close();
    }

    public static void AddMessageHandler( string eventName, SocketMessageHandler handler )
    {
        socket.On( eventName, data => handler( eventName, data.Json ) );
    }

    public static void SendSocketMessage( string eventName, object payload )
    {
        if ( isConnected )
            socket.Emit( eventName, payload );
    }
}