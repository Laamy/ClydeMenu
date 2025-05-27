namespace ClydeMenu.Engine;

using UnityEngine;

using HarmonyLib;

using ClydeMenu.Engine.Settings;
using ClydeMenu.Engine.Menu;
using System.Collections.Generic;
using System;

internal static class Patches
{
    public static class Patches_MenuSceneCrap
    {
        [HarmonyPatch(typeof(PhysGrabber), "RayCheck")]
        public class Patches_RayCheck
        {
            public static bool Prefix(bool _grab)
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }

        [HarmonyPatch(typeof(SpectateCamera), "PlayerSwitch")]
        public static class Patches_PlayerSwitch
        {
            public static bool Prefix(bool _next)
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }

        // bro...
        [HarmonyPatch(typeof(MenuButton), "Update")]
        public static class Patches_MenuButtonUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }

        [HarmonyPatch(typeof(MenuElementServer), "Update")]
        public static class Patches_MenuElementServerUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }

        [HarmonyPatch(typeof(MenuElementSaveFile), "Update")]
        public static class Patches_MenuElementSaveFileUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }

        [HarmonyPatch(typeof(MenuElementRegion), "Update")]
        public static class Patches_MenuElementRegionUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }

        [HarmonyPatch(typeof(MenuSlider), "Update")]
        public static class Patches_MenuSliderUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }

        [HarmonyPatch(typeof(MenuScrollBox), "Update")]
        public static class Patches_MenuScrollBoxUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }

        [HarmonyPatch(typeof(MenuManager), "Update")]
        public static class Patches_MenuManagerUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }

        [HarmonyPatch(typeof(MenuButtonArrow), "Update")]
        public static class Patches_MenuButtonArrowUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }

        [HarmonyPatch(typeof(MenuElementHover), "Update")]
        public static class Patches_MenuElementHoverUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
        }

        // block the chat keybind stuff
        [HarmonyPatch(typeof(ChatManager), "StateInactive")]
        public static class Patches_StateInactive
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
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

    // public static bool IsMasterClientOrSingleplayer()

    public static bool OverrideMaster = false;
    [HarmonyPatch(typeof(SemiFunc), "IsMasterClientOrSingleplayer")]
    public static class Patches_IsMasterClientOrSingleplayer
    {
        public static bool Prefix(ref bool __result)
        {
            if (!OverrideMaster)
                return true;

            __result = true;
            return false;
        }
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
                playerPing = new System.Random().Next(20, 22);//20,22
                playerPingTimer = 6f;
            }

            ClientInstance.SetFieldValue("playerPing", __instance, playerPing);
        }
    }

    public class AudioInfo
    {
        public string label;
        public Vector3 position;
        public float time;
        public float maxDistance = 20f;

        // animation crap
        public float fadeInTimer = 0;
        public bool isFadingIn = true;
    }

    public static List<AudioInfo> audioStack = new();
    [HarmonyPatch(typeof(AudioSource), "Play", [])]
    public static class Patches_AudioSrcLogger
    {
        public static bool Prefix(AudioSource __instance)
        {
            if (__instance.spatialBlend <= 0)
                return true;

            var localPlayer = ClientInstance.GetLocalPlayer();

            if (localPlayer == null)
                return true;

            var obj = __instance.gameObject;
            var pos = obj.transform.position;
            if (Vector3.Distance(pos, localPlayer.transform.position) > __instance.maxDistance)
                return true;

            //Entry.Log(__instance.gameObject.name);
            var info = new AudioInfo
            {
                label = SerilizeString(obj.name),
                position = pos,
                time = Time.time,
                maxDistance = __instance.maxDistance,
            };
            lock (audioStack)
            {
                var oldClip = audioStack.Find(x => x.label == info.label);
                if (oldClip != null && oldClip.time < Time.time - 1)
                    audioStack.Remove(oldClip);

                if (oldClip == null)
                    audioStack.Add(info);
            }
            return true;
        }

        private static string SerilizeString(string name)
        {
            var i = name.Length - 1;
            while (i >= 0 && char.IsDigit(name[i])) i--;
            return name.Substring(0, i + 1);
        }
    }

    [HarmonyPatch(typeof(RunManager), "ChangeLevel")]
    public static class Patches_ChangeLevel
    {
        public static bool Prefix(RunManager __instance)
        {
            Storage.WAYPOINTS_POINTS.Clear();

            return true;
        }
    }
}
