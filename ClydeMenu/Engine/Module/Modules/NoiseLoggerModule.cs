namespace ClydeMenu.Engine.Commands;

using UnityEngine;

using ClydeMenu.Engine.Settings;

[ClydeChange("New NoiseLogger module that allows you to use minecraft subtitles in R.E.P.O", ClydeVersion.Release_v1_0)]
public class NoiseLoggerModule : BaseModule
{
    public NoiseLoggerModule() : base("NoiseLogger", "Minecraft subtitles ported to REPO", "Visual") { }

    public override void Initialize()
    {
        IsEnabled = true;
    }

    const int padding = 6;
    public override void OnRender()
    {
        if (!MenuSettings.VISUAL_NOISELOGGER.Value)
            return;

        if (Camera.main == null)
            return;

        lock (Patches.audioStack)
        {
            var y = 20 + padding;
            foreach (var audio in Patches.audioStack)
            {
                if (audio == null)
                    continue;
                if (audio.time < Time.time - 5f)
                    continue;

                if (audio.isFadingIn)
                {
                    audio.fadeInTimer = Mathf.Clamp(audio.fadeInTimer + Time.deltaTime, 0, 1);
                    if (audio.fadeInTimer >= 1f)
                        audio.isFadingIn = false;
                }
                else
                {
                    if (audio.time < Time.time - 4.5f)
                        audio.fadeInTimer = Mathf.Clamp(audio.fadeInTimer - Time.deltaTime, 0, 1);
                }

                var prefix = "";
                var audioPosition = Camera.main.WorldToScreenPoint(audio.position);

                if (audioPosition.x < 0)
                    prefix = "[◀] ";

                else if (audioPosition.x > Screen.width)
                    prefix = "[▶] ";

                var drawLabel = $"{prefix}{audio.label}";

                var measure = RenderUtils.StringSize(drawLabel, 16);
                RenderUtils.DrawRect(new Vector2(
                    Screen.width - measure.x - padding * 2,
                    Screen.height - y - padding
                ), new Vector2(measure.x + padding * 2, 20 + padding), new Color(0.1f, 0.1f, 0.1f, audio.fadeInTimer));
                RenderUtils.DrawString(new Vector2(
                    Screen.width - measure.x - padding,
                    Screen.height - y
                ), drawLabel, new Color(1, 1, 1, audio.fadeInTimer), 16);
                y += 20 + padding;
            }
        }
    }
}
