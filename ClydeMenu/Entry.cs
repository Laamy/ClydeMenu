namespace ClydeMenu;

using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using Object = UnityEngine.Object;

using ClydeMenu.Engine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class Entry
{
    private static readonly List<GameObject> loadedComps = [];

    public static void Load()
    {
        Kernel32.AllocConsole();
        {
            var stdHandle = Kernel32.GetStdHandle(-11);
            var safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
            var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            var encoding = System.Text.Encoding.UTF8;
            var standardOutput = new StreamWriter(fileStream, encoding) { AutoFlush = true };
            Console.SetOut(standardOutput);
            Console.Clear();
        }

        Console.WriteLine("ClydeMenu injected successfully");

        Application.logMessageReceived += onLog;

        try
        {
            InitModule<ClientComponent>("ClydeMenu");
            InitModule<CmdBarComponent>("CmdBar");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading comps: {e}");
            return;
        }
    }

    private static void onLog(string logString, string stackTrace, LogType type)
    {
        Console.WriteLine($"(LOG) [{type}] {logString}");

        // namespoofer
        if (logString.Contains("Joining specific open server:"))
        {
            NameSpoofer.randomName(); // :)
        }

        // versionsspoofer
        if (logString.Contains("Photon - Set Version"))
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "v0.1.2";
            NameSpoofer.randomName(); // :)
        }

        // massccrasher
        if (logString.Contains("Steam: Lobby entered with ID"))
        {
            var options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.All;
            for (byte i = 0; i < 0xFF; ++i)
            {
                if (i==199)
                    continue;
                PhotonNetwork.RaiseEvent(i, null, options, SendOptions.SendReliable);
            }

            //foreach (var plyr in SemiFunc.PlayerGetList())
            //{
            //    if (plyr.photonView.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber
            //        || plyr.photonView.OwnerActorNr == PhotonNetwork.MasterClient.ActorNumber)
            //        continue;
            //    var plyrActorId = plyr.photonView.OwnerActorNr;
            //
            //    var options = new RaiseEventOptions();
            //    options.TargetActors = new[] { plyrActorId };
            //    PhotonNetwork.RaiseEvent(199, null, options, SendOptions.SendReliable);
            //    Console.WriteLine($"Kicked player {plyr.name} from the server.");
            //}

            //var options = new RaiseEventOptions();
            //options.TargetActors = new[] { PhotonNetwork.LocalPlayer.ActorNumber };
            //PhotonNetwork.RaiseEvent(199, null, options, SendOptions.SendReliable);
        }
    }

    public static void InitModule<T>(string modName) where T : MonoBehaviour
    {
        var LoadObj = new GameObject(modName);
        var clientModule = LoadObj.AddComponent<T>();
        loadedComps.Add(LoadObj);
        Object.DontDestroyOnLoad(LoadObj);
    }

    public static void Unload()
    {
        Console.Clear();
        Kernel32.FreeConsole();

        foreach (var obj in loadedComps)
            Object.DestroyImmediate(obj);

        //CleanStaticReferences();
        GC.Collect();
    }
}
