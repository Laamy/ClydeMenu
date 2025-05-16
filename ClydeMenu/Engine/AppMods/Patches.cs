namespace ClydeMenu.Engine;

using ClydeMenu.Engine.Menu;

using HarmonyLib;

internal static class Patches
{
    [HarmonyPatch(typeof(PhysGrabber), "RayCheck")]
    public class Patches_RayCheck
    {
        public static bool Prefix(bool _grab)
            => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    }

    [HarmonyPatch(typeof(SpectateCamera), "PlayerSwitch")]
    public static class Patches_PlayerSwitch
    {
        public static bool Prefix(bool _next)
            => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    }

    // no clue how to patch unity input so its an issue for later me :)
}
