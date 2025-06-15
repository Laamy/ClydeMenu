namespace ClydeMenu.Engine.Commands;

using System;
using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Settings;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[ClydeChange("New HaulInfo module that displays the current haul info (min-max & level)", ClydeVersion.Release_v1_0)]
[ClydeChange("Fixed divided by zero error when in disposal level", ClydeVersion.Release_v1_5)]
public class HaulInfoModule : BaseModule
{
    public HaulInfoModule() : base("HaulInfo", "Information about the scene (lvl & estimated possible haul)", "Visual") { }

    public override void Initialize()
    {
        IsEnabled = true;
    }

    public float[] quotaMultiply = [
        3.57142851821014f,
        4.76190457268368f,
        4.74639982908994f,
        7.07604922227163f,
        7.00606421609719f,
        9.20059248925003f,
        8.99406905420288f,
        8.70545410147724f,
        8.39388691548115f,
        8.21523722771264f,
        8.1699877074361f
    ];

    public Rect haulBounds = Rect.zero;
    public bool init = false;
    public int padding = 5;

    [ClydeChange("Expanded the size of HaulInfo to match higher then 100k min hauls", ClydeVersion.Release_v1_7_1)]
    public override void OnRender()
    {
        if (!MenuSettings.VISUAL_MAPINFO.Value)
            return;

        if (!init)
        {
            init = true;
            var tmpStr = $"Min:999999 | Max:999999";
            var strMsr = RenderUtils.StringSize(tmpStr, 20);
            haulBounds = new Rect(0, 0, strMsr.x + (padding * 2), (strMsr.y * 2) + (padding * 5));
        }

        using (RenderUtils.Window.Begin(MenuSceneComponent.IsMenuOpen(), ref haulBounds, "HaulInfo"))
        {
            if (RoundDirector.instance == null)
                return;

            var haulGoalMax = ClientInstance.FetchFieldValue<int, RoundDirector>("haulGoal", RoundDirector.instance);
            var extractionPoints = ClientInstance.FetchFieldValue<int, RoundDirector>("extractionPoints", RoundDirector.instance);
            if (haulGoalMax == 0 || extractionPoints == 0)
                return;
            float num = haulGoalMax / extractionPoints;

            var curLevel = RunManager.instance.levelsCompleted;
            num *= quotaMultiply[Mathf.Clamp(curLevel, 0, quotaMultiply.Length - 1)];

            RenderUtils.DrawString(new Vector2(padding, padding), $"Min:{haulGoalMax} | Max:{(int)num}\r\nLvl:{curLevel + 1}", Color.white, 20);
        }
    }
}
