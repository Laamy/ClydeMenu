namespace ClydeMenu;

using UnityEngine;
using Object = UnityEngine.Object;

using ClydeMenu.Engine;
using System;
using System.IO;

public class Entry
{
    private static GameObject LoadObj;
    private static ClientModule1 clientModule;

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

        LoadObj = new GameObject("ClydeMenu");
        clientModule = LoadObj.AddComponent<ClientModule1>();
        Object.DontDestroyOnLoad(LoadObj);
    }

    public static void Unload()
    {
        Console.Clear();
        Kernel32.FreeConsole();

        Object.DestroyImmediate(LoadObj);
        Object.DestroyImmediate(clientModule);

        //CleanStaticReferences();
        GC.Collect();
    }
}
