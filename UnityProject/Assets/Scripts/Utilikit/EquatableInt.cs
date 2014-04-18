/* EquatableInt.cs
 * ----------------------------
 * Copyright Eddie Cameron & Grasshopper NYC 2013 (MIT licenced)
 * ----------------------------
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntComparer : IEqualityComparer<int> 
{
	public bool Equals (int a, int b )
	{
		return a == b;
	}

	public int GetHashCode( int a )
	{
		return a;
	}
}
