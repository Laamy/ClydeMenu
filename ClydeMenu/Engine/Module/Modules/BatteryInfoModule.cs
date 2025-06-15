namespace ClydeMenu.Engine.Commands;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Settings;

using UnityEngine;
using UnityEngine.Video;

//[ClydeChange("Fixed divided by zero error when in disposal level", ClydeVersion.Release_v1_7_1)]
public class BatteryInfoModule : BaseModule
{
    public BatteryInfoModule() : base("BatteryInf", "Information about the scene (lvl & estimated possible haul)", "Visual") { }

    public override void Initialize()
    {
        IsEnabled = true;
    }

    public List<(float life, long lastSampleTime)> batterySamples = new();
    public Rect infoBounds = Rect.zero;
    public bool init = false;
    public int padding = 5;

    public float averageDrainRate = 0;
    public float averageTimer = 0;

    public float HowLongLeft()
    {
        if (batterySamples.Count < 2)
            return -1; // infinity

        var first = batterySamples.First();
        var last = batterySamples.Last();

        if (first.lastSampleTime == last.lastSampleTime)
            return -1; // infinity (not possible hopefully)

        var drainsASecond = first.life - last.life;
        drainsASecond *= 100;
        drainsASecond = Mathf.Max(drainsASecond, 1);

        var held = ClientInstance.GetHeldObject();
        if (held != null)
        {
            var grabObj = held.rb.gameObject;
            var grabBattery = grabObj.GetComponent<ItemBattery>();
            if (grabBattery != null)
            {
                var life = grabBattery.batteryLife;

                return ((life * 100) / drainsASecond) / 100; // estimated time left in seconds
            }
        }

        return -1;
    }

    public void SortSamples()
    {
        if (watch == null || watch.ElapsedMilliseconds > 1000)
        {
            if (batterySamples.Count > 50)
                batterySamples.RemoveAt(0);

            if (batterySamples.Count > 0 && batterySamples[0].lastSampleTime + 1000 < watch.ElapsedMilliseconds)
                batterySamples.RemoveAt(0);

            var held = ClientInstance.GetHeldObject();
            if (held != null)
            {
                var grabObj = held.rb.gameObject;
                var grabBattery = grabObj.GetComponent<ItemBattery>();
                if (grabBattery != null)
                {
                    float life = grabBattery.batteryLife;
                    batterySamples.Add((life, watch.ElapsedMilliseconds));
                }
            }
            else batterySamples.Clear(); // might aswell
        }
    }

    Stopwatch watch = null;
    public override void OnRender()
    {
        //if (!MenuSettings.VISUAL_MAPINFO.Value)
        //    return;

        if (watch == null)
            watch = Stopwatch.StartNew();

        SortSamples();

        if (!init)
        {
            init = true;
            var tmpStr = $"Min:99999 | Max:999999";
            var strMsr = RenderUtils.StringSize(tmpStr, 20);
            infoBounds = new Rect(0, (strMsr.y * 2) + (padding * 5), strMsr.x + (padding * 2), (strMsr.y * 2) + (padding * 5));
        }

        using (RenderUtils.Window.Begin(MenuSceneComponent.IsMenuOpen(), ref infoBounds, "BatteryInfo"))
        {
            var life = "N/A";
            var secondsLeft = HowLongLeft();
            if (secondsLeft != -1)
            {
                life = $"{secondsLeft:0.00} seconds left";
            }

            // held.rb.gameObject.name
            RenderUtils.DrawString(new Vector2(padding, padding), $"{life}", Color.white);
        }
    }
}
