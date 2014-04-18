/* ExtensionMethods.cs
 * ----------------------------
 * Copyright Eddie Cameron & Grasshopper NYC, 2013 (MIT licenced)
 * ----------------------------
 * Holds general useful extension methods in one handy place
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
	#region Transform
    /// <summary>
    /// Sets the X position.
    /// </summary>
    /// <param name="transform">Transform.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="space">Space, (world or self) Default is self</param>
    public static void SetXPos( this Transform transform, float x, Space space = Space.Self )
	{
		if ( space == Space.Self )
		{
			Vector3 newPos = transform.localPosition;
			newPos.x = x;
			transform.localPosition = newPos;
		}
		else
		{
			Vector3 newPos = transform.position;
			newPos.x = x;
			transform.position = newPos;
		}
	}
	
    /// <summary>
    /// Sets the Y position.
    /// </summary>
    /// <param name="transform">Transform.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="space">Space, (world or self) Default is self</param>
    public static void SetYPos( this Transform transform, float y, Space space = Space.Self )
	{
		if ( space == Space.Self )
		{
			Vector3 newPos = transform.localPosition;
			newPos.y = y;
			transform.localPosition = newPos;
		}
		else
		{
			Vector3 newPos = transform.position;
			newPos.y = y;
			transform.position = newPos;
		}
	}
	
    /// <summary>
    /// Sets the Z position.
    /// </summary>
    /// <param name="transform">Transform.</param>
    /// <param name="z">The z coordinate.</param>
    /// <param name="space">Space, (world or self) Default is self</param>
    public static void SetZPos( this Transform transform, float z, Space space = Space.Self )
	{
		if ( space == Space.Self )
		{
			Vector3 newPos = transform.localPosition;
			newPos.z = z;
			transform.localPosition = newPos;
		}
		else
		{
			Vector3 newPos = transform.position;
			newPos.z = z;
			transform.position = newPos;
		}
	}

    /// <summary>
    /// Renames the unity InverseTransformPoint method
    /// </summary>
    /// <returns>The local position.</returns>
    /// <param name="worldPos">world position.</param>
    /// <param name="position">If set to <c>true</c>, transforms as position, not direction.</param>
    public static Vector3 WorldToLocalPosition( this Transform transform, Vector3 worldPos, bool position = true )
    {
        if ( position )
            return transform.InverseTransformPoint( worldPos );
        else
            return transform.InverseTransformDirection( worldPos );
    }

    /// <summary>
    /// Renames the Unity InverseTransformDirection method
    /// </summary>
    /// <returns>The local direction.</returns>
    /// <param name="worldDirection">World direction.</param>
    public static Vector3 WorldToLocalDirection( this Transform transform, Vector3 worldDirection )
    {
        return transform.InverseTransformDirection( worldDirection );
    }

    /// <summary>
    /// Renames the unity TransformPoint method
    /// </summary>
    /// <returns>The world position.</returns>
    /// <param name="worldPos">Local position.</param>
    /// <param name="position">If set to <c>true</c>, transforms as position, not direction.</param>
    public static Vector3 LocalToWorldPosition( this Transform transform, Vector3 localPos, bool position = true )
    {
        if ( position )
            return transform.TransformPoint( localPos );
        else
            return transform.TransformDirection( localPos );
    }

    /// <summary>
    /// Renames the Unity TransformDirection method
    /// </summary>
    /// <returns>The world direction.</returns>
    /// <param name="worldDirection">Local direction.</param>
    public static Vector3 LocalToWorldDirection( this Transform transform, Vector3 localDirection )
    {
        return transform.TransformDirection( localDirection );
    }

    /// <summary>
    /// Get the ray in world space starting at this position and going along transform.forward
    /// </summary>
    /// <returns>The ray.</returns>
    /// <param name="transform">Transform.</param>
    public static Ray GetRay( this Transform transform )
    {
        return new Ray( transform.position, transform.forward );
    }
	#endregion
	
	#region Collections
    /// <summary>
    /// Tries to add a value to a collection.
    /// </summary>
    /// <returns><c>true</c>, if value didn't exist in collection so was added, <c>false</c> otherwise.</returns>
    /// <param name="itemToTryAdd">Item to try to add.</param>
	public static bool AddUnique<T>( this ICollection<T> collection, T itemToTryAdd )
	{
		if ( collection.Contains( itemToTryAdd ) )
			return false;
		
		collection.Add( itemToTryAdd );
        return true;
	}

    /// <summary>
    /// Tries to add an entry to a dictionary
    /// </summary>
    /// <returns><c>true</c>, if key not present in dictionary, so could be added, <c>false</c> otherwise.</returns>
    /// <param name="keyToAdd">Key to try to add.</param>
    /// <param name="valueToAdd">Value to try to add.</param>
    public static bool TryAdd<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, TKey keyToAdd, TValue valueToAdd )
    {
        if ( dictionary.ContainsKey( keyToAdd ) )
            return false;
        else
        {
            dictionary.Add( keyToAdd, valueToAdd );
            return true;
        }
    }

    /// <summary>
    /// Enumerates through a linked list (using foreach) removing null entries as it goes
    /// </summary>
    public static IEnumerable<T> EnumerateRemoveNull<T>( this LinkedList<T> linkedList )
    {
        var node = linkedList.First;
        while ( node != null )
        {
            var next = node.Next;
            if ( node.Value == null )
                linkedList.Remove( node );
            else
                yield return node.Value;
            
            node = next;
        }
    }
    
    /// <summary>
    /// Enumerates through a dictionary (using foreach) removing null values as it goes
    /// </summary>
    public static IEnumerable<KeyValuePair<K,V>> EnumerateRemoveNull<K,V>( this IDictionary<K,V> dictionary )
    {
        var toRemove = new HashSet<K>();
        foreach ( var kv in dictionary )
        {
            if ( kv.Value == null )
                toRemove.Add( kv.Key );
            else
                yield return kv;
        }

        foreach( var removedKey in toRemove )
            dictionary.Remove( removedKey );
    }

    
    /// <summary>
    /// Enumerates through a collection (using foreach) removing null values as it goes
    /// </summary>
    public static IEnumerable<T> EnumerateRemoveNull<T>( this ICollection<T> collection )
    {
        var toRemove = new HashSet<T>();
        foreach ( var t in collection )
        {
            if ( t == null )
                toRemove.Add( t );
            else
                yield return t;
        }

        foreach ( var removed in toRemove )
            collection.Remove( removed );
    }

    /// <summary>
    /// Get a random element from a list
    /// </summary>
    /// <returns>A random element.</returns>
    public static T RandomElement<T>( this IList<T> collection )
    {
        if ( collection.Count > 0 )
            return collection[UnityEngine.Random.Range( 0, collection.Count )];

        return default( T );
    }
	#endregion
	
	#region GameObject
    /// <summary>
    /// Like GetComponent, but returns a script component that implements a given interface
    /// </summary>
    /// <returns>The first found component with interface T.</returns>
    /// <typeparam name="T">The interface type to look for</typeparam>
	public static T GetComponentWithInterface<T>( this GameObject gameObject ) where T : class
	{
		foreach( Component c in gameObject.GetComponents<Component>() )
		{
			T t = c as T;
			if ( t != null )
				return t;
		}
		return null;
	}

    public static T GetComponentWithInterface<T>( this Component component ) where T : class
    {
        return component.gameObject.GetComponentWithInterface<T>();
    }
    
    /// <summary>
    /// Like GetComponents, but returns script components that implement a given interface
    /// </summary>
    /// <returns>An array of components with interface T.</returns>
    /// <typeparam name="T">The interface type to look for</typeparam>
	public static T[] GetComponentsWithInterface<T>( this GameObject gameObject ) where T : class
	{
		List<T> interfaceComponents = new List<T>();
		foreach( Component c in gameObject.GetComponents<Component>() )
		{
			T t = c as T;
			if ( t != null )
				interfaceComponents.Add( t );
		}
		return interfaceComponents.ToArray();
    }
    
    public static T[] GetComponentsWithInterface<T>( this Component component ) where T : class
    {
        return component.gameObject.GetComponentsWithInterface<T>();
    }

    /// <summary>
    /// Only returns the first component per child
    /// </summary>
    /// <returns>The components in children no crash.</returns>
    /// <param name="gameObject">Game object.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T[] GetComponentsInChildrenNoCrash<T>( this GameObject gameObject ) where T : Component
    {
        List<T> foundComponents = new List<T>();
        Queue<Transform> objectsToCheck = new Queue<Transform>();
        objectsToCheck.Enqueue( gameObject.transform );

        while ( objectsToCheck.Count > 0 )
        {
            var objectToCheck = objectsToCheck.Dequeue();
            var foundComponent = objectToCheck.GetComponent<T>();
            if ( foundComponent )
                foundComponents.Add( foundComponent );

            foreach ( Transform child in objectToCheck )
                objectsToCheck.Enqueue( child );
        }
        return foundComponents.ToArray();
    }
    
    public static T[] GetComponentsInChildrenNoCrash<T>( this Component component ) where T : Component
    {
        return component.gameObject.GetComponentsInChildrenNoCrash<T>();
    }
   	#endregion
}