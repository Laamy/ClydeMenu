namespace ClydeMenu.Engine.Utils;

using UnityEngine;

using System.Collections.Generic;

// might add some fancy stuff later
internal class ComplexColour
{
    public Color Colour { get; set; }

    public ComplexColour(Color colour) => Colour = colour;

    //rgb
    public static ComplexColour FromRGBa(float r, float g, float b, float alpha)
        => new(new Color(r / 255, g / 255, b / 255, alpha / 255));

    public override bool Equals(object obj)=> obj is ComplexColour cc && Colour == cc.Colour;
    public override int GetHashCode() => Colour.GetHashCode();
}

internal class TextureUtils
{
    private static Dictionary<Color, Texture2D> textureCache = [];

    public static Texture2D CreateSolid(ComplexColour colour)
    {
        if (!textureCache.TryGetValue(colour.Colour, out _))
        {
            Texture2D texture = new(1, 1);
            texture.SetPixel(0, 0, colour.Colour);
            texture.Apply();
            textureCache[colour.Colour] = texture;
        }

        return textureCache[colour.Colour];
    }

    public static Texture2D CreateSolid(Color colour)
        => CreateSolid(new ComplexColour(colour));

    public static Texture2D CreateSolid(float r, float g, float b, float alpha)
        => CreateSolid(ComplexColour.FromRGBa(r,g,b,alpha));
}
