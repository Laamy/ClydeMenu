namespace ClydeMenu.Engine;

using System;
using SingularityGroup.HotReload;
using UnityEngine;

internal class GameEvents
{
    public static event Action? OnLobbyJoin;
    public static event Action? OnLobbyJoinStart;

    public static event Action? OnLobbyLeft;
    public static event Action? OnBanned;
    public static event Action? OnKicked;

    public static void Start()
    {
        Console.WriteLine("ClydeMenu - GameEvents Init");

        Application.logMessageReceived += onLog;

        Console.WriteLine("ClydeMenu - GameEvents Hooked");
    }

    public static void Shutdown()
    {
        Application.logMessageReceived -= onLog;
        OnLobbyJoin = null;
        OnLobbyJoinStart = null;
        OnLobbyLeft = null;
        OnBanned = null;
        OnKicked = null;
        Console.WriteLine("GameEvents::Shutdown(void) -> Shutdown successful, resources cleaned up.");
    }

    private static void onLog(string logString, string stackTrace, LogType type)
    {
        if (type != LogType.Exception)
            Console.WriteLine($"(LOG) [{type}] {logString}");
        else
        {
            Console.WriteLine($"(LOG) [{type}] {logString}");
            Console.WriteLine($"(STACKTRACE) [{type}] {stackTrace}");
        }

        if (logString.Contains("Photon - Set AppId")) // good spot for namespoof & versionspoof(packet)
            OnLobbyJoinStart?.Invoke();

        if (logString.Contains("Steam: Lobby entered with ID"))
        {
            OnLobbyJoin?.Invoke();
            Storage.Network.CanSendPackets = true;
        }

        if (logString.Contains("Steam: Leaving lobby..."))
        {
            OnLobbyLeft?.Invoke();
            Storage.Network.CanSendPackets = false;
        }

        if (logString.Contains("Msg: \"Ban\""))
            OnBanned?.Invoke();

        if (logString.Contains("You were kicked by the server."))
            OnKicked?.Invoke();
    }
}
