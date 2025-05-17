namespace ClydeMenu.Engine;

using ClydeMenu.Engine.Menu;

using UnityEngine;

using Photon.Pun;

using HarmonyLib;
using System.Diagnostics;
using Unity.VisualScripting;
using System;
using ClydeMenu.Engine.Settings;

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

    // bro...
    [HarmonyPatch(typeof(MenuButton), "Update")]
    public static class Patches_MenuButtonUpdate
    {
        public static bool Prefix()
            => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    }
    
    [HarmonyPatch(typeof(MenuElementServer), "Update")]
    public static class Patches_MenuElementServerUpdate
    {
        public static bool Prefix()
            => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    }
    
    [HarmonyPatch(typeof(MenuElementSaveFile), "Update")]
    public static class Patches_MenuElementSaveFileUpdate
    {
        public static bool Prefix()
            => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    }
    
    [HarmonyPatch(typeof(MenuElementRegion), "Update")]
    public static class Patches_MenuElementRegionUpdate
    {
        public static bool Prefix()
            => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    }
    
    [HarmonyPatch(typeof(MenuSlider), "Update")]
    public static class Patches_MenuSliderUpdate
    {
        public static bool Prefix()
            => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    }
    
    [HarmonyPatch(typeof(MenuScrollBox), "Update")]
    public static class Patches_MenuScrollBoxUpdate
    {
        public static bool Prefix()
            => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    }
    
    [HarmonyPatch(typeof(MenuManager), "Update")]
    public static class Patches_MenuManagerUpdate
    {
        public static bool Prefix()
            => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    }
    
    [HarmonyPatch(typeof(MenuButtonArrow), "Update")]
    public static class Patches_MenuButtonArrowUpdate
    {
        public static bool Prefix()
            => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    }
    
    [HarmonyPatch(typeof(MenuElementHover), "Update")]
    public static class Patches_MenuElementHoverUpdate
    {
        public static bool Prefix()
            => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    }

    [HarmonyPatch(typeof(PlayerAvatar), "FixedUpdate")]
    public static class Patches_PingSpoof
    {
        private static float playerPingTimer;
        private static int playerPing = 0;
        public static void Postfix(PlayerAvatar __instance)
        {
            if (!__instance.photonView.IsMine)
                return;

            if (!MenuSettings.AccountSpoofer.Value && !MenuSettings.PingSpoofer.Value)
                return;

            playerPingTimer -= Time.deltaTime;
            if (playerPingTimer <= 0f)
            {
                playerPing = new System.Random().Next(15, 25);
                playerPingTimer = 6f;
            }

            ClientInstance.SetFieldValue("playerPing", __instance, playerPing);
        }
    }
}
