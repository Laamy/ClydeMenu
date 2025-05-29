namespace ClydeMenu.Engine.Commands;

using ClydeMenu.Engine.Settings;

using UnityEngine;

[ClydeChange("New HaulInfo module that displays the current haul info (min-max & level)", ClydeVersion.Release_v1_0)]
public class HaulInfoModule : BaseModule
{
    public HaulInfoModule() : base("HualInfo", "Information about the scene (lvl & estimated possible haul)", "Visual") { }

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

    public override void OnRender()
    {
        if (!MenuSettings.VISUAL_MAPINFO.Value)
            return;

        if (RoundDirector.instance == null)
            return;

        var haulGoalMax = ClientInstance.FetchFieldValue<int, RoundDirector>("haulGoal", RoundDirector.instance);
        if (haulGoalMax == 0)
            return;
        float num = haulGoalMax / ClientInstance.FetchFieldValue<int, RoundDirector>("extractionPoints", RoundDirector.instance);

        var curLevel = RunManager.instance.levelsCompleted;
        num *= quotaMultiply[Mathf.Clamp(curLevel, 0, quotaMultiply.Length - 1)];

        RenderUtils.DrawString(new Vector2(10, 10), $"Min:{haulGoalMax} | Max:{(int)num}\r\nLvl:{curLevel + 1}", Color.white, 20);
    }
}
