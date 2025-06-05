namespace ClydeMenu.Engine;

using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Clocks
{
    private static Dictionary<string, float> clocks = new();

    public static void Reset(string v)
    {
        lock (clocks)
            clocks[v] = 0.1f;
    }

    public static bool IsAlive(string v)
    {
        lock (clocks)
        {
            if (!clocks.ContainsKey(v))
                clocks[v] = 0;

            return clocks[v] > 0;
        }
    }

    internal static void Tick()
    {
        lock (clocks)
        {
            var keys = new List<string>(clocks.Keys);
            foreach (var key in keys)
                clocks[key] = Mathf.Clamp(clocks[key] - Time.deltaTime, 0, float.MaxValue);
        }
    }
}