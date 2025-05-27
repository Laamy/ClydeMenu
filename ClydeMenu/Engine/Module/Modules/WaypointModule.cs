namespace ClydeMenu.Engine.Commands;

using ClydeMenu.Engine.Settings;
using UnityEngine;
using Random = System.Random;

public class WaypointModule : BaseModule
{
    public WaypointModule() : base("Waypoint", "Waypoints for the map", "Visual") {}

    public override void Initialize()
    {
        IsEnabled = true;
    }

    public override void OnRender()
    {
        if (!MenuSettings.VISUAL_MAPINFO.Value)
            return;

        foreach (var waypoint in Storage.WAYPOINTS_POINTS)
            RenderUtils.DrawWaypoint(waypoint.Position, waypoint.Label, waypoint.Color);

        //RenderUtils.DrawWaypoint(new Vector3(0,0,2), "Green", new Color(0.15f, 0.5f, 0.15f));
        //RenderUtils.DrawWaypoint(new Vector3(0,0,0), "Red", new Color(0.5f, 0.15f, 0.15f));
        //RenderUtils.DrawWaypoint(new Vector3(0,0,-2), "Blue", new Color(0.15f, 0.15f, 0.5f));
    }
}
