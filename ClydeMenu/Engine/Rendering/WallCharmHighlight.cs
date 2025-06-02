namespace ClydeMenu.Engine.Rendering;

using System;
using UnityEngine;

public class WallCharmHighlight : ClydeMonoBehaviour
{
    //public Color color;
    //private void OnGUI()
    //{
    //    if (this.IsAlive)
    //        RenderUtils.DrawAABB(ClientInstance.GetActiveColliderBounds(base.gameObject), color);
    //}
    //
    //public static WallCharmHighlight Create(GameObject target, Color color)
    //{
    //    try
    //    {
    //        if (target.TryGetComponent<WallCharmHighlight>(out var existing))
    //            return existing;
    //
    //        var charm = target.AddComponent<WallCharmHighlight>();
    //        charm.color = color;
    //
    //        return charm;
    //    }
    //    catch (Exception e)
    //    {
    //        Entry.Log($"Error creating WallCharmHighlight: {e}");
    //        // full info ect here
    //        Entry.Log($"Target: {target.name}, Color: {color}");
    //        Entry.Log($"Stack Trace: {Environment.StackTrace}");
    //        Entry.Log($"Target Type: {target.GetType()}");
    //        return null;
    //    }
    //    return null;
    //}
}
