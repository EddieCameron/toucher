/* TextureMapMaker.cs
 * Copyright Grasshopper 2013
 * ----------------------------
 *
 */

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TextureMapMaker : BehaviourBase 
{
    [MenuItem ("Grasshopper/MakeTextureAtlas")]
    static void MakeTextureAtlas ()
    {
        Object[] selectedTextureObjects = Selection.GetFiltered( typeof( Texture2D ), SelectionMode.Assets | SelectionMode.DeepAssets );
        Debug.Log( "Selected " + selectedTextureObjects.Length );
        Texture2D[] textures = new Texture2D[selectedTextureObjects.Length];
        for ( int i = 0; i < textures.Length; i++ )
            textures[i] = (Texture2D)selectedTextureObjects[i];
        BuildAtlas( textures );
    }

    [MenuItem ("Grasshopper/MakeTextureAtlas", true)]
    static bool ValidateMakeTextureAtlas() 
    {
        Object[] validSelected = Selection.GetFiltered( typeof( Texture2D ), SelectionMode.Assets | SelectionMode.DeepAssets );
        return validSelected.Length > 0;
    }

    static void BuildAtlas( Texture2D[] textures )
    {
        Debug.Log( textures.Length );

        Texture2D output = new Texture2D( 1024, 1024 );
        output.PackTextures( textures, 0, 1024 );

        using ( var fs = new FileStream( "Assets/texMap.png", FileMode.Create ) )
        {
            byte[] pngBytes = output.EncodeToPNG();
            fs.Write( pngBytes, 0, pngBytes.Length );
            fs.Close();
        }
    }
}