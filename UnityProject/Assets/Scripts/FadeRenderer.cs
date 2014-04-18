/* FadeRenderer.cs
 * Copyright Grasshopper 2013
 * ----------------------------
 *
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeRenderer : BehaviourBase
{
    public float fadeTime = 0.4f;

    public float currentAlpha { get; private set; }

	bool fadingIn, fadingOut, pulsing;
    float maxAlpha;

    public Material material { get { return renderer.material; } }

    protected override void Awake()
    {
        base.Awake();

        maxAlpha = renderer.material.color.a;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void SetColour( Color color )
    {
        renderer.material.color = color;
        maxAlpha = renderer.material.color.a;
    }

    public void Show()
    {
        SetAlpha( maxAlpha );
    }

    public void Hide()
    {
        SetAlpha( 0 );
    }

    public void FadeIn()
    {
        StopCoroutine( "_FadeOut" );

        if ( !fadingIn || !renderer.enabled )
        {
            fadingIn = true;
            fadingOut = false;
			pulsing = false;

            StartCoroutine( "_FadeIn" );
        }
    }

    public void FadeOut()
    {
        StopCoroutine( "_FadeIn" );

        if ( !fadingOut || renderer.enabled)
        {
            fadingOut = true;
            fadingIn = false;
			pulsing = false;

            StartCoroutine( "_FadeOut" );
        }
    }

	public Coroutine Pulse( float pulseTime )
	{
		return StartCoroutine( _Pulse( pulseTime ) );
	}


	IEnumerator _Pulse( float pulseTime )
	{
		if ( fadingIn )
			StopCoroutine( "_FadeIn" );
		else if ( fadingOut )
			StopCoroutine( "_FadeOut" );

		pulsing = true;
		float startAlpha = currentAlpha;
		yield return StartCoroutine( LerpValue( 0f, Mathf.PI, pulseTime, delegate( float t ) {
			if ( pulsing )
				SetAlpha( startAlpha + Mathf.Sin( t ) * ( maxAlpha - startAlpha ) );
		} ) );
	}

    void SetAlpha( float a )
    {
        currentAlpha = a;
        Color newColour = renderer.material.color;
        newColour.a = a;
        renderer.material.color = newColour;
    }

    IEnumerator _FadeIn()
    {
        renderer.enabled = true;
        float startAlpha = renderer.material.color.a;
        float fadeInTime = ( 1f - startAlpha / maxAlpha ) * fadeTime;

        float amount = 0f;
        while ( amount < 1f )
        {
            yield return 0;
            amount += Time.deltaTime / fadeInTime;
            SetAlpha( Mathf.Lerp( startAlpha, maxAlpha, amount ) );
        }
    }

    IEnumerator _FadeOut()
    {
        float startAlpha = renderer.material.color.a;
        float fadeOutTime = startAlpha / maxAlpha * fadeTime;

        float amount = 0f;
        while ( amount < 1f )
        {
            yield return 0;
            amount += Time.deltaTime / fadeOutTime;
            SetAlpha( Mathf.Lerp( startAlpha, 0f, amount ) );
        }

        renderer.enabled = false;
    }
}