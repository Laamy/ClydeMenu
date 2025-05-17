namespace ClydeMenu.Engine;

using ClydeMenu.Engine.Menu;

using UnityEngine;

using Photon.Pun;

using HarmonyLib;
using System.Diagnostics;
using Unity.VisualScripting;
using System;

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
    //[HarmonyPatch(typeof(MenuButton), "Update")]
    //public static class Patches_MenuButtonUpdate
    //{
    //    public static bool Prefix()
    //        => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    //}
    //
    //[HarmonyPatch(typeof(MenuElementServer), "Update")]
    //public static class Patches_MenuElementServerUpdate
    //{
    //    public static bool Prefix()
    //        => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    //}
    //
    //[HarmonyPatch(typeof(MenuElementSaveFile), "Update")]
    //public static class Patches_MenuElementSaveFileUpdate
    //{
    //    public static bool Prefix()
    //        => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    //}
    //
    //[HarmonyPatch(typeof(MenuElementRegion), "Update")]
    //public static class Patches_MenuElementRegionUpdate
    //{
    //    public static bool Prefix()
    //        => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    //}
    //
    //[HarmonyPatch(typeof(MenuSlider), "Update")]
    //public static class Patches_MenuSliderUpdate
    //{
    //    public static bool Prefix()
    //        => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    //}
    //
    //[HarmonyPatch(typeof(MenuScrollBox), "Update")]
    //public static class Patches_MenuScrollBoxUpdate
    //{
    //    public static bool Prefix()
    //        => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    //}
    //
    //[HarmonyPatch(typeof(MenuManager), "Update")]
    //public static class Patches_MenuManagerUpdate
    //{
    //    public static bool Prefix()
    //        => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    //}
    //
    //[HarmonyPatch(typeof(MenuButtonArrow), "Update")]
    //public static class Patches_MenuButtonArrowUpdate
    //{
    //    public static bool Prefix()
    //        => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    //}
    //
    //[HarmonyPatch(typeof(MenuElementHover), "Update")]
    //public static class Patches_MenuElementHoverUpdate
    //{
    //    public static bool Prefix()
    //        => !MenuSceneComponent.Instance.HasMenuByType<MainMenu>();
    //}

    private static Stopwatch _stopwatch = null;
    private static int oldPing = 0;
    [HarmonyPatch(typeof(PlayerAvatar), "OnPhotonSerializeView")]
    public static class Patches_OnPhotonSerializeView
    {
        public static bool Prefix(PlayerAvatar __instance, PhotonStream stream, PhotonMessageInfo info)
        {
            if (!Storage.CHEAT_PLAYER_AccountSpoofer)
                return true; // only pingspoof if accountspoof is active

            // macros !! (closest thing to it)
            T Get<T>(string v) => ClientInstance.FetchFieldValue<T, PlayerAvatar>(v, __instance);
    
            if (!__instance.photonView.IsMine)
                return true;
    
            if (stream.IsWriting)
            {
                stream.SendNext(Get<bool>("isCrouching"));
                stream.SendNext(Get<bool>("isSprinting"));
                stream.SendNext(Get<bool>("isCrawling"));
                stream.SendNext(Get<bool>("isSliding"));
                stream.SendNext(Get<bool>("isMoving"));
                stream.SendNext(Get<bool>("isGrounded"));
                stream.SendNext(Get<bool>("Interact"));
                stream.SendNext(Get<Vector3>("InputDirection"));
                stream.SendNext(PlayerController.instance.VelocityRelative);
                stream.SendNext(Get<Vector3>("rbVelocityRaw"));
                stream.SendNext(PlayerController.instance.transform.position);
                stream.SendNext(PlayerController.instance.transform.rotation);
                stream.SendNext(Get<Vector3>("localCameraPosition"));
                stream.SendNext(Get<Quaternion>("localCameraRotation"));
                stream.SendNext(PlayerController.instance.CollisionGrounded.physRiding);
                stream.SendNext(PlayerController.instance.CollisionGrounded.physRidingID);
                stream.SendNext(PlayerController.instance.CollisionGrounded.physRidingPosition);
                stream.SendNext(__instance.flashlightLightAim.clientAimPoint);
                Console.WriteLine(__instance.flashlightLightAim.clientAimPoint);

                if (_stopwatch == null || _stopwatch.ElapsedMilliseconds > 1000)
                {
                    _stopwatch = Stopwatch.StartNew();
                    oldPing = new System.Random().Next(15,25); // randomize ping to avoid detection
                    Console.WriteLine($"Ping: {oldPing}");
                }
                stream.SendNext(oldPing);
                return true;
            }
    
            return true;
        }
    }
}
