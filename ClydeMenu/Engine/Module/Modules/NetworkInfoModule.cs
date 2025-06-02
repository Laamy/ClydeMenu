namespace ClydeMenu.Engine.Commands;

using System.Collections.Generic;
using System;

using ClydeMenu.Engine.Settings;

using Photon.Pun;

using UnityEngine;

[ClydeChange("Added NetworkInfo module for ping & jitter alerts", ClydeVersion.Release_v1_3)]
public class NetworkInfoModule : BaseModule
{
    private List<Action> renderActions;

    public NetworkInfoModule() : base("NetworkInfo", "Displays current network latency (ping)", "Visual") { }

    public override void Initialize()
    {
        IsEnabled = true;

        renderActions = new List<Action>
        {
            RenderPing,
            RenderPacketJitter,
            RenderPacketLoss
        };
    }

    public static class NetConfig
    {
        // ping wwwarnings
        public const int PING_Warning = 149;
        public const int PING_Critical = 225;

        // jitter warnings
        public const int JITTER_Warning = 50;
        public const int JITTER_Critical = 100;
        public const int JITTER_MAXSAMPLE = 100;

        // packet loss warnings
        public const int PACKETLOSS_Warning = 5;
        public const int PACKETLOSS_Critical = 10;
    }

    public override void OnRender()
    {
        if (!MenuSettings.VISUAL_NETNFO.Value)
            return;

        if (!PhotonNetwork.IsConnected)
            return;

        x = 0; // reset
        foreach (var action in renderActions)
            action();
    }

    public int x = 0;
    public const int padding = 5;

    private void RenderPacketLoss()
    {
        // PacketLossByCrcCheck
        //var packetLossCount = PhotonNetwork.PacketLossByCrcCheck; // photon netwwwork doesnt provide anythhing for actual packet loss bruhh
        //if (packetLossCount < NetConfig.PACKETLOSS_Warning)
        //    return;

        //Console.WriteLine($"Packet Loss: {packetLossCount}%"); // debug log

        //var color = packetLossCount > NetConfig.PACKETLOSS_Critical ? Color.red : Color.yellow;
        //var text = $"PL: {packetLossCount}%";
        //var size = RenderUtils.StringSize(text, 12);
        //RenderUtils.DrawString(new Vector2(Screen.width - size.x - 10, 50), text, color, 12);
        //x += (int)size.x + padding;
    }

    private List<int> rttSamples = new();
    private void RenderPacketJitter()
    {
        rttSamples.Add(PhotonNetwork.GetPing());
        if (rttSamples.Count > NetConfig.JITTER_MAXSAMPLE)
            rttSamples.RemoveAt(0);

        var count = rttSamples.Count;
        if (count < 2)
            return; // lack of samples

        var sumDiff = 0f;
        for (int i = 1; i < count; i++)
            sumDiff += Mathf.Abs(rttSamples[i] - rttSamples[i - 1]);
        var jitter = sumDiff / (count - 1);

        //Console.WriteLine($"Jitter: {jitter:F1}ms"); // debug log

        if (jitter < NetConfig.JITTER_Warning)
            return;

        var color = jitter > NetConfig.JITTER_Critical ? Color.red : Color.yellow;
        var text = $"JIT: {jitter:F1}ms";
        var size = RenderUtils.StringSize(text, 12);
        RenderUtils.DrawString(new Vector2(Screen.width - size.x - x, 30), text, color, 12);
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
        RenderUtils.DrawString(new Vector2(Screen.width - size.x - x, 10), text, color, 12);
        x += (int)size.x + padding;
    }
}
