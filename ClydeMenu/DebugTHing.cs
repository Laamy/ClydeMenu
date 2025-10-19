namespace ClydeMenu;

using System;
using System.Collections.Generic;

using UnityEngine;

enum UILayers
{
    Static, // menu background page content & everything drawn as disabled
    Enabled, // only contains texture to draw over other disabled state teexture for stuff thats enabled
}

public class DebugTHing
{
    static Dictionary<UILayers, Texture2D> Layers;

    static Color32 COLOUR_TRANS = new Color32(0, 0, 0, 0);

    public static void Load()
    {
        //foreach (UILayers i in Enum.GetValues(typeof(UILayers)))
        //{
        //    var layer = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false)
        //    {
        //        filterMode = FilterMode.Point,
        //        wrapMode = TextureWrapMode.Clamp
        //    };
        //
        //    if (!layer.Reinitialize(Screen.width, Screen.height))
        //        throw new Exception("nigger v2.59399.3425.345");// very much informative
        //
        //    layer.Clear(COLOUR_TRANS);
        //
        //    Layers[i] = layer;
        //}
    }

    static void DrawPage1()
    {
        //var layer = Layers[UILayers.Static];

        //layer.Clear(COLOUR_TRANS);

        //layer.DrawRect(new Vector2(100, 100), new Vector2(100, 100), new Color32(255 / 2, 255 / 2, 0, 255));
    }

    static void DrawPage4()
    {
    
    }

    public static void OnGUI()
    {
    }
}