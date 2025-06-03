namespace ClydeMenu.Engine.Commands;

using System.Diagnostics;

using ClydeMenu.Engine.Settings;

[ClydeChange("New RainbowPlayer module that does what you think it does", ClydeVersion.Release_v1_4)]
public class RainbowPlayerModule : BaseModule
{
    public RainbowPlayerModule() : base("RainbowPlayer", "", "Visual") { }

    public override void Initialize()
    {
        IsEnabled = true;
    }

    Stopwatch watch = null;
    int curColor = 0;
    public override void OnRender()
    {
        if (!MenuSettings.VISUAL_RAINBOW.Value)
            return;

        if (watch == null || watch.ElapsedMilliseconds > (1000 / 5))
        {
            ClientInstance.GetLocalAvatar().PlayerAvatarSetColor(curColor++);
            watch = Stopwatch.StartNew();
        }

        if (curColor == 35)
            curColor = -1;
    }
}
