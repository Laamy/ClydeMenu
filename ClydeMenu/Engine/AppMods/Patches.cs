namespace ClydeMenu.Engine;

using ClydeMenu.Engine.Menu;

using UnityEngine;

using Photon.Pun;

using HarmonyLib;
using System.Diagnostics;
using Unity.VisualScripting;
using System;
using ClydeMenu.Engine.Settings;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Reflection;

internal static class Patches
{
    public static class Patches_MenuSceneCrap
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

        // block the chat keybind stuff
        [HarmonyPatch(typeof(ChatManager), "StateInactive")]
        public static class Patches_StateInactive
        {
            public static bool Prefix()
                => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }
    }

    //[HarmonyPatch(typeof(PhotonNetwork), "ExecuteRpc")]
    //public static class Patches_ExecuteRpc
    //{
    //    public static bool Prefix(Hashtable rpcData, Player sender)
    //    {
    //        if (sender.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
    //        {
    //            Entry.Log($"[RPC] spoofed from localplayer to master");
    //            MethodInfo method = typeof(PhotonNetwork).GetMethod("ExecuteRpc", BindingFlags.NonPublic | BindingFlags.Static, null, [typeof(Hashtable), typeof(Player)], null);
    //            method?.Invoke(null, [rpcData, PhotonNetwork.MasterClient]);
    //            return true;
    //        }
    //
    //        return false;
    //    }
    //}

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
                playerPing = -new System.Random().Next(15, 25);
                playerPingTimer = 6f;
            }

            ClientInstance.SetFieldValue("playerPing", __instance, playerPing);
        }
    }
}
