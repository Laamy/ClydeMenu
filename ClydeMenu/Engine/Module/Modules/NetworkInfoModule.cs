namespace ClydeMenu.Engine.Commands;

using System.Collections.Generic;
using System;

using ClydeMenu.Engine.Settings;

using Photon.Pun;

using UnityEngine;
using ClydeMenu.Engine.Menu;

[ClydeChange("Added NetworkInfo module for ping & jitter alerts", ClydeVersion.Release_v1_3)]
public class NetworkInfoModule : BaseModule
{
    private List<Action> renderActions;
    public int x = 0;

    public NetworkInfoModule() : base("NetworkInfo", "Displays current network latency (ping)", "Visual") { }

    public override void Initialize()
    {
        IsEnabled = true;

        renderActions = new List<Action>
        {
            RenderPing,
            RenderPacketJitter
        };
    }

    public static class NetConfig
    {
        public const int PING_Warning = 149;
        public const int PING_Critical = 225;
        public const int JITTER_Warning = 50;
        public const int JITTER_Critical = 100;
        public const int JITTER_MAXSAMPLE = 100;
        public const int PACKETLOSS_Warning = 5;
        public const int PACKETLOSS_Critical = 10;
    }

    public Rect networkInfoBounds;
    public bool init = false;
    public int padding = 5;

    public override void OnRender()
    {
        if (!MenuSettings.VISUAL_NETNFO.Value)
            return;

        if (!init)
        {
            networkInfoBounds = new Rect(Screen.width - 120, 0, 120, 24);
            init = true;
        }

        using (RenderUtils.Window.Begin(MenuSceneComponent.IsMenuOpen(), ref networkInfoBounds, "NetworkInfo"))
        {
            if (!PhotonNetwork.IsConnected)
                return;

            x = 0;
            foreach (var action in renderActions)
                action();
        }
    }
    
    private List<int> rttSamples = new();
    private void RenderPacketJitter()
    {
        rttSamples.Add(PhotonNetwork.GetPing());
        if (rttSamples.Count > NetConfig.JITTER_MAXSAMPLE)
            rttSamples.RemoveAt(0);

        var count = rttSamples.Count;
        if (count < 2)
            return;

        var sumDiff = 0f;
        for (int i = 1; i < count; i++)
            sumDiff += Mathf.Abs(rttSamples[i] - rttSamples[i - 1]);
        var jitter = sumDiff / (count - 1);

        if (jitter < NetConfig.JITTER_Warning)
            return;

        var color = jitter > NetConfig.JITTER_Critical ? Color.red : Color.yellow;
        var text = $"JIT: {jitter:F1}ms";
        var size = RenderUtils.StringSize(text, 12);
        RenderUtils.DrawString(new Vector2(networkInfoBounds.width - size.x - x, padding + 20), text, color, 12);
        x += (int)size.x + padding;
    }

    private void RenderPing()
    {
        if (PhotonNetwork.GetPing() < NetConfig.PING_Warning)
            return;

        var ping = PhotonNetwork.GetPing();
        var color = ping > NetConfig.PING_Critical ? Color.red : Color.yellow;
        var text = $"PING: {ping}ms";
        var size = RenderUtils.StringSize(text, 12);
        RenderUtils.DrawString(new Vector2(networkInfoBounds.width - size.x - x, padding), text, color, 12);
        x += (int)size.x + padding;
    }
}
