namespace ClydeMenu.Engine.Commands;

using ClydeMenu.Engine.Settings;

[ClydeChange("New Waypoint module To mark important spots in the map", ClydeVersion.Release_v1_0)]
[ClydeChange("Fixed debug waypoint showing up in release build", ClydeVersion.Release_v1_5)]
public class WaypointModule : BaseModule
{
    public WaypointModule() : base("Waypoint", "Waypoints for the map", "Visual") {}

    public override void Initialize()
    {
        IsEnabled = true;

        //Storage.WAYPOINTS_POINTS.Add(new Storage.WaypointInfo()
        //{
        //    Color = new Color(0.15f, 0.5f, 0.15f),
        //    Label = "Green",
        //    Position = new Vector3(0, 2, 0)
        //});

        //GameEvents.OnLevelChanged += OnLevelChanged;
    }

    //private void OnLevelChanged(Patches.ChangeLevelInfo info)
    //    => Storage.WAYPOINTS_POINTS.Clear();

    public override void OnRender()
    {
        if (!MenuSettings.VISUAL_MAPINFO.Value)
            return;

        foreach (var waypoint in Storage.WAYPOINTS_POINTS)
            RenderUtils.DrawWaypoint(waypoint);

        //RenderUtils.DrawWaypoint(new Vector3(0,0,2), "Green", new Color(0.15f, 0.5f, 0.15f));
        //RenderUtils.DrawWaypoint(new Vector3(0,0,0), "Red", new Color(0.5f, 0.15f, 0.15f));
        //RenderUtils.DrawWaypoint(new Vector3(0,0,-2), "Blue", new Color(0.15f, 0.15f, 0.5f));
    }
}
