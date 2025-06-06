/// [Noise Logger]
/// Display nearby sounds as subtitles in the bottom-right corner of the screen.
/// Each subtitle includes:
/// - A short label (e.g. "Footsteps", "Robe Screams")
/// - A horizontal arrow indicating direction relative to the player's forward vector:
///   - ← if sound is to the left
///   - → if sound is to the right
/// Only log sounds actually heard by the local player (within audible range and volume threshold).
/// Subtitles fade out after a short duration (e.g. 3–5 seconds).

// [Missed Room Alerts]
// For each room, track whether the local player has entered it this round. (A counter of sorts and markers)
// Unexplored rooms are outlined in orange (on the map or HUD).

// [Team Tracker]
// On hold — tracking the value of loot held(or destroyed) by teammates.
// Not sure about this one

// [Basic Macros]
// On hold - to automate stuff like tumble jumping higher then usual
// this could be seen as unfair

// [Visual Indicators]
/// Packets - Display ping packet loss and other misc things that are higher then normal
// Helper - subtle screen effects for if a player drops an item nearby (and loses value)
// Helper - arrows on the side of the screen to indicate a player is nearby (or a cart)
// Helper - this could also loop back around to subtitles for sounds ^
// On hold - drawing a trajectory line for thrown items
// Battery / Flashlight Monitor - Show remaining battery percentage and estimated time left (basic QoL)
// Teammate Tracker - Display alive teammates on dirt finder (You can find this by seeing the voice meeters in escape being active even if your not near them)
// Weather/Time Display - Show in-game time and weather conditions (future idea but i do want a timer to express respawn timers easier)
// Performance Monitor - FPS, frame time, and memory usage overlay
// Voice Activity Indicator - Visual cues showing who's speaking in voice chat (like discord)

// [Map Info]
// display map info on the screen (for example shows how many people are still alive if you have the upgrade for it)
/// Waypoints - placing waypoints on the map to mark important locations with colours & 4 letter tags
//    - could put it on top of the screen on a bar similar to Rust's compass

// [Utility Modules]
// Crosshair Customization - Different crosshair styles and colors
// FOV Adjuster - Fine-tune field of view settings
// Chat Enhancer - Chat history, timestamps, and filtering options
// Session Stats - Track your performance across multiple runs
// Ping System - Quick communication markers for teammates (clydemenu only users cuz duh)
// Quick Action Shortcuts - Customizable hotkeys for common actions
// Advanced Audio Filters - Customize which sounds appear in NoiseLogger
// Custom HUD Layouts - Rearrange information display positions
// Backup & Restore - Save/load different configuration profiles
// Screenshot Gallery - Organized storage for captured moments
// Session Replay - Record and review gameplay sessions (I really want to implement a replay mod)

// these are meant to be goofy things that most people wont care about (hopefully) but you can just go into the settings file and give yourself everything (its just for goals)
// also note It could be fun to put experimental features in shop and move them to release when im done
// [Cosmetic Items(100-500 gems)]
// Custom Crosshairs Pack - Unlock various crosshair designs (might scroll crosshair X for a bit)
// Theme Bundle - Additional UI themes beyond the current ones (I could pack red green blue and purple into buyable themes for 50 each/I'll add a custom theme editor so people dont have to buy them)
// Waypoint Icons Pack - Custom icons for different waypoint types (I was thinking about the rust waypoint icons in some kind of Rust Waypoints pack)
// Sound Pack - Custom notification sounds (2012 sounds stuff like that could be funny)
// Sound Alerts - Customizable audio cues for nearby teammates, etc. (ties into the noiselogger & other modules but as an extra setting for audio)
// Quick Notes - In-game notepad for jotting down important information (I find myself writing down how much we've destroyed so far)

// [Breadcrumbs]
// On hold - drawing breadcrumb trails where players and carts go
// might be to much

// [Keystrokes]
// Everyone loves a good old keystrokes module
// this gives me an excuse to add a new kind of draggable window that only shows a background & draggable functionality when in the clients main menu/hud
// probably gonna write a new renderutil window based on the other one i never ended up using
// its also probably best if i make noise subtitles & haul info one of these draggable windows

/// [Freelook]
/// Hold alt to freeze the camera direction and rotate it freely without effecting the movement vectors
// make a disableTimer for freelook so i can set the disable timer up by .1 when a peeper has touched u!

namespace ClydeMenu.Engine;

using System;
using System.Collections.Generic;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
class ClydeChangeAttribute : Attribute
{
    public string Description { get; }
    public uint Version { get; }
    public bool IsDebug { get; }

    public ClydeChangeAttribute(string description, uint version, bool isDebug = false)
    {
        Description = description;
        Version = version;
        IsDebug = isDebug;
    }
}

class ClydeChangeEntry
{
    public string Description { get; }
    public uint Version { get; }
    public bool IsDebug { get; }

    public ClydeChangeEntry(string description, uint version, bool isDebug)
    {
        Description = description;
        Version = version;
        IsDebug = isDebug;
    }
}

[ClydeChange("Updated for R.E.P.O v0.1.2.37_beta", Release_v1_2)]
[ClydeChange("Updated for R.E.P.O v0.1.2.38_beta", Release_v1_3)] // hello?? bro??
static class ClydeVersion
{
    public static readonly bool IsDebug = false;

    public const uint Release_v1_0 = 0x01000000;
    public const uint Release_v1_1 = 0x01010000;
    public const uint Release_v1_2 = 0x01020000;
    public const uint Release_v1_3 = 0x01030000;
    public const uint Release_v1_4 = 0x01040000;
    public const uint Release_v1_5 = 0x01050000;//latest

    public const uint Release_v1_6_1 = 0x01060100;

    public const uint Current = Release_v1_6_1;

    public static string ToVersionString(uint version)
    {
        var major = (byte)((version >> 24) & 0xFF);
        var minor = (byte)((version >> 16) & 0xFF);
        var build = (byte)((version >> 8) & 0xFF);
        var debug = (byte)(version & 0xFF);

        var result = $"v{major}.{minor}.{build}";
        if (debug != 0)
            result += $".{debug} Dev";

        return result;
    }

    private static readonly Dictionary<uint, List<ClydeChangeEntry>> changesByVersion = new();

    static ClydeVersion()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            Register(type.GetCustomAttributes<ClydeChangeAttribute>());
            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                Register(method.GetCustomAttributes<ClydeChangeAttribute>());
        }
    }

    private static void Register(IEnumerable<ClydeChangeAttribute> attributes)
    {
        foreach (var attr in attributes)
        {
            var entry = new ClydeChangeEntry(attr.Description, attr.Version, attr.IsDebug);
            if (!changesByVersion.TryGetValue(attr.Version, out var list))
                changesByVersion[attr.Version] = list = new();
            list.Add(entry);
        }
    }

    public static IReadOnlyList<ClydeChangeEntry> Get(uint version) =>
        changesByVersion.TryGetValue(version, out var list) ? list : Array.Empty<ClydeChangeEntry>();

    public static IEnumerable<uint> AllVersions => changesByVersion.Keys;
}
