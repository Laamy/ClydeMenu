namespace ClydeMenu.Engine.Rendering;

using System;
using UnityEngine;

public class WallCharmHighlight : ClydeBehaviour
{
    public Color color;
    public override void OnGUI()
    {
        if (this.IsAlive)
            RenderUtils.DrawAABB(ClientInstance.GetActiveColliderBounds(gameObject), color);
    }

    public static WallCharmHighlight Create(GameObject target, Color color)
    {
        try
        {
            var charm = target.CLYDE_GetOrAdd<WallCharmHighlight>();
            charm.color = color;
    
            return charm;
        }
        catch (Exception e)
        {
            Entry.Log($"Error creating WallCharmHighlight: {e}");
            Entry.Log($"Target: {target.name}, Color: {color}");
            Entry.Log($"Stack Trace: {Environment.StackTrace}");
            Entry.Log($"Target Type: {target.GetType()}");
        }
        return null;
    }
}
