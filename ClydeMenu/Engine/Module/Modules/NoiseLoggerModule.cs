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

    /*

    (LOG) [Exception] NullReferenceException: Object reference not set to an instance of an object
    (STACKTRACE) [Exception] ClydeMenu.Engine.Commands.NoiseLoggerModule.OnRender () (at <b548ff4422ef4b41b7cb9adc8be37e3c>:0)
    ClydeMenu.Engine.ClientComponent.OnGUI () (at <b548ff4422ef4b41b7cb9adc8be37e3c>:0)
    ClydeMenu.Entry.OnGUI () (at <b548ff4422ef4b41b7cb9adc8be37e3c>:0)
    System.Reflection.RuntimeMethodInfo.Invoke (System.Object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) (at <4b234520e36749be9cf6b053d911690f>:0)
    Rethrow as TargetInvocationException: Exception has been thrown by the target of an invocation.
    System.Reflection.RuntimeMethodInfo.Invoke (System.Object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) (at <4b234520e36749be9cf6b053d911690f>:0)
    System.Reflection.MethodBase.Invoke (System.Object obj, System.Object[] parameters) (at <4b234520e36749be9cf6b053d911690f>:0)
    Hot_reload.Components.HotReloadBehaviour.OnGUI () (at <268968d3c2a04898a0d180aacdeb083d>:0)

    */
    const int padding = 6;
    public override void OnRender()
    {
        if (!MenuSettings.VISUAL_NOISELOGGER.Value)
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
