/* Flasher.cs
 * Copyright Eddie Cameron & Grasshopper NYC 2013 (MIT licenced)
 * ----------------------------
 * Flash any renderer a given colour, for a time
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flasher : BehaviourBase
{
    public Color flashColour;
    public float timePerFlash = 0.2f;

    Material origMat; // to make sure mat doesn't change
    Material flashMat;
    Color origColour;
    bool flashing;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void Flash( float forTime )
    {
        if ( flashing )
        {
            StopCoroutine( "_Flash" );
        }
        else
        {
            flashing = true;
            origMat = renderer.material;
            renderer.sharedMaterial = flashMat = new Material( origMat );
            origColour = origMat.color;
        }
        StartCoroutine( "_Flash", forTime );
    }
   
    IEnumerator _Flash( float forTime )
    {
        float flashTime = 0f;
        bool flashOn = false;
        while ( flashTime < forTime )
        {
            flashOn = !flashOn;
            flashMat.color = flashOn ? flashColour : origColour;
            yield return new WaitForSeconds( timePerFlash );
            flashTime += timePerFlash;
            if ( renderer.sharedMaterial != flashMat )
            {
                DebugExtras.Log( "Mat changed" );
                flashing = false;
                yield break;
            }
        }

        flashing = false;
        renderer.material = origMat;
    }
}