namespace ClydeMenu.Engine.Commands;

using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Settings;

using UnityEngine;

//[ClydeChange("Fixed divided by zero error when in disposal level", ClydeVersion.Release_v1_7_1)]
public class BatteryInfoModule : BaseModule
{
    public BatteryInfoModule() : base("BatteryInf", "Information about the scene (lvl & estimated possible haul)", "Visual") { }

    public override void Initialize()
    {
        IsEnabled = true;
    }

    public Rect infoBounds = Rect.zero;
    public bool init = false;
    public int padding = 5;

    public override void OnRender()
    {
        //if (!MenuSettings.VISUAL_MAPINFO.Value)
        //    return;

        if (!init)
        {
            init = true;
            var tmpStr = $"Min:99999 | Max:999999";
            var strMsr = RenderUtils.StringSize(tmpStr, 20);
            infoBounds = new Rect(0, (strMsr.y * 2) + (padding * 5), strMsr.x + (padding * 2), (strMsr.y * 2) + (padding * 5));
        }

        using (RenderUtils.Window.Begin(MenuSceneComponent.IsMenuOpen(), ref infoBounds, "BatteryInfo"))
        {
            var lp = ClientInstance.GetLocalAvatar();
            var held = ClientInstance.GetHeldObject();

            if (held == null)
                return;

            var grabObj = held.rb.gameObject;
            var grabBattery = grabObj.GetComponent<ItemBattery>();
            if (grabBattery == null)
                return;

            // held.rb.gameObject.name
            RenderUtils.DrawString(new Vector2(padding, padding), $"{grabBattery.batteryLife}", Color.white);
        }
    }
}
