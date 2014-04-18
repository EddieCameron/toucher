/* ColourSpot.cs
 * Copyright Grasshopper 2013
 * ----------------------------
 *
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColourSpot : BehaviourBase 
{
    public Material[] colourMats;

    FadeRenderer spotRenderer;

    protected override void Awake()
    {
        base.Awake();

        spotRenderer = GetComponentInChildren<FadeRenderer>();
        spotRenderer.renderer.material = colourMats.RandomElement();
    }

    protected override void Start()
    {
        base.Start();

        spotRenderer.Hide();
        spotRenderer.FadeIn();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void Remove()
    {
        spotRenderer.FadeOut();
        Destroy( gameObject, 2f );
    }
}