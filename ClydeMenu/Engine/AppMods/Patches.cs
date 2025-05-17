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

    //private static Stopwatch _stopwatch = null;
    //private static int oldPing = 0;
    //[HarmonyPatch(typeof(PlayerAvatar), "OnPhotonSerializeView")]
    //public static class Patches_OnPhotonSerializeView
    //{
    //    public static bool Prefix(PlayerAvatar __instance, PhotonStream stream, PhotonMessageInfo info)
    //    {
    //        if (!MenuSettings.AccountSpoofer.Value)
    //            return true; // only pingspoof if accountspoof is active
    //
    //        // macros !! (closest thing to it)
    //        T Get<T>(string v) => ClientInstance.FetchFieldValue<T, PlayerAvatar>(v, __instance);
    //
    //        if (!__instance.photonView.IsMine)
    //            return true;
    //
    //        if (stream.IsWriting)
    //        {
    //            stream.SendNext(Get<bool>("isCrouching"));
    //            Console.WriteLine($"Crouching: {Get<bool>("isCrouching")}");
    //            stream.SendNext(Get<bool>("isSprinting"));
    //            Console.WriteLine($"Sprinting: {Get<bool>("isSprinting")}");
    //            stream.SendNext(Get<bool>("isCrawling"));
    //            Console.WriteLine($"Crawling: {Get<bool>("isCrawling")}");
    //            stream.SendNext(Get<bool>("isSliding"));
    //            Console.WriteLine($"Sliding: {Get<bool>("isSliding")}");
    //            stream.SendNext(Get<bool>("isMoving"));
    //            Console.WriteLine($"Moving: {Get<bool>("isMoving")}");
    //            stream.SendNext(Get<bool>("isGrounded"));
    //            Console.WriteLine($"Grounded: {Get<bool>("isGrounded")}");
    //            stream.SendNext(Get<bool>("Interact"));
    //            Console.WriteLine($"Interact: {Get<bool>("Interact")}");
    //            stream.SendNext(PlayerController.instance.InputDirection);
    //            Console.WriteLine($"InputDirection: {PlayerController.instance.InputDirection}");
    //            stream.SendNext(PlayerController.instance.VelocityRelative);
    //            Console.WriteLine($"VelocityRelative: {PlayerController.instance.VelocityRelative}");
    //            stream.SendNext(PlayerController.instance.rb.velocity);
    //            Console.WriteLine($"rb.velocity: {PlayerController.instance.rb.velocity}");
    //            stream.SendNext(PlayerController.instance.transform.position);
    //            Console.WriteLine($"transform.position: {PlayerController.instance.transform.position}");
    //            stream.SendNext(PlayerController.instance.transform.rotation);
    //            Console.WriteLine($"transform.rotation: {PlayerController.instance.transform.rotation}");
    //            stream.SendNext(PlayerController.instance.cameraGameObject.transform.position);
    //            Console.WriteLine($"cameraGameObject.transform.position: {PlayerController.instance.cameraGameObject.transform.position}");
    //            stream.SendNext(PlayerController.instance.cameraGameObject.transform.rotation);
    //            Console.WriteLine($"cameraGameObject.transform.rotation: {PlayerController.instance.cameraGameObject.transform.rotation}");
    //            stream.SendNext(PlayerController.instance.CollisionGrounded.physRiding);
    //            Console.WriteLine($"CollisionGrounded.physRiding: {PlayerController.instance.CollisionGrounded.physRiding}");
    //            stream.SendNext(PlayerController.instance.CollisionGrounded.physRidingID);
    //            Console.WriteLine($"CollisionGrounded.physRidingID: {PlayerController.instance.CollisionGrounded.physRidingID}");
    //            stream.SendNext(PlayerController.instance.CollisionGrounded.physRidingPosition);
    //            Console.WriteLine($"CollisionGrounded.physRidingPosition: {PlayerController.instance.CollisionGrounded.physRidingPosition}");
    //            stream.SendNext(__instance.flashlightLightAim.clientAimPoint);
    //            Console.WriteLine($"flashlightLightAim.clientAimPoint: {__instance.flashlightLightAim.clientAimPoint}");
    //
    //            if (_stopwatch == null || _stopwatch.ElapsedMilliseconds > 1000)
    //            {
    //                _stopwatch = Stopwatch.StartNew();
    //                oldPing = new System.Random().Next(15,25); // randomize ping to avoid detection
    //                Console.WriteLine($"Ping: {oldPing}");
    //            }
    //            stream.SendNext(oldPing);
    //            return true;
    //        }
    //
    //        return false;
    //    }
    //}
}
