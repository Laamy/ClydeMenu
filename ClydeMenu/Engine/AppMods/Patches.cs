namespace ClydeMenu.Engine;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.EnterpriseServices.Internal;
using ClydeMenu.Engine.Menu;
using ClydeMenu.Engine.Rendering;
using ClydeMenu.Engine.Settings;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

internal static class Patches
{
    public static class Patches_MenuSceneCrap
    {
        [HarmonyPatch(typeof(PhysGrabber), "RayCheck")]
        public class Patches_RayCheck
        {
            public static bool Prefix(bool _grab)
            {
                if (isInFreelook)
                    return false;
                return MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
            }
        }

        [HarmonyPatch(typeof(SpectateCamera), "PlayerSwitch")]
        public static class Patches_PlayerSwitch
        {
            public static bool Prefix(bool _next)
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
        }

        // bro...
        [HarmonyPatch(typeof(MenuButton), "Update")]
        public static class Patches_MenuButtonUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
        }

        [HarmonyPatch(typeof(MenuElementServer), "Update")]
        public static class Patches_MenuElementServerUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
        }

        [HarmonyPatch(typeof(MenuElementSaveFile), "Update")]
        public static class Patches_MenuElementSaveFileUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
        }

        [HarmonyPatch(typeof(MenuElementRegion), "Update")]
        public static class Patches_MenuElementRegionUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
        }

        [HarmonyPatch(typeof(MenuSlider), "Update")]
        public static class Patches_MenuSliderUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
        }

        [HarmonyPatch(typeof(MenuScrollBox), "Update")]
        public static class Patches_MenuScrollBoxUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
        }

        [HarmonyPatch(typeof(MenuManager), "Update")]
        public static class Patches_MenuManagerUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
        }

        [HarmonyPatch(typeof(MenuButtonArrow), "Update")]
        public static class Patches_MenuButtonArrowUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
        }

        [HarmonyPatch(typeof(MenuElementHover), "Update")]
        public static class Patches_MenuElementHoverUpdate
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
        }

        // block the chat keybind stuff
        [HarmonyPatch(typeof(ChatManager), "StateInactive")]
        public static class Patches_StateInactive
        {
            public static bool Prefix()
                => MenuSceneComponent.Instance == null || !MenuSceneComponent.Instance.IsFocused();
        }

        // BRO THIS IS DRIIVINGH ME INSANE WTF DID IB REAK
        // annoying cursor crap
        [HarmonyPatch(typeof(CursorManager), "Update")]
        public static class Patches_CursorManager
        {
            public static bool Prefix()
            {
                if (MenuSceneComponent.Instance == null || MenuSceneComponent.Instance.IsFocused())
                {
                    RenderUtils.SetCursorState(true);
                    return false;
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(CursorManager), "Unlock")]
        public static class Patches_CursorManagerUnlock
        {
            public static bool Prefix(float _time)
            {
                if (MenuSceneComponent.Instance == null || MenuSceneComponent.Instance.IsFocused())
                    return false;
                return true;
            }
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

    [ClydeChange("New PingSpoof(er) for privacy in lobbies", ClydeVersion.Release_v1_0)]
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

            if (Clocks.IsAlive("MainMenuUpdate"))
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

    public class ChangeLevelInfo
    {
        public RunManager __Instance;
        public bool completedLevel;
        public bool levelFailed;
        public RunManager.ChangeLevelType changeLevelType;
        public ChangeLevelInfo(RunManager instance, bool completed, bool failed, RunManager.ChangeLevelType changeType)
        {
            __Instance = instance;
            completedLevel = completed;
            levelFailed = failed;
            changeLevelType = changeType;
        }
    }

    [HarmonyPatch(typeof(RunManager), "ChangeLevel")]
    public static class Patches_ChangeLevel
    {
        public static bool Prefix(RunManager __instance, bool _completedLevel, bool _levelFailed, RunManager.ChangeLevelType _changeLevelType = RunManager.ChangeLevelType.Normal)
        {
            Storage.WAYPOINTS_POINTS.Clear();

            GameEvents.TriggerLevelChanged(new ChangeLevelInfo(__instance, _completedLevel, _levelFailed, _changeLevelType));

            return true;
        }
    }

    //[HarmonyPatch(typeof(Gizmos), "DrawLine")]
    //public static class Patches_DrawLine
    //{
    //    public static bool Prefix(Gizmos __instance, Vector3 from, Vector3 to)
    //    {
    //        RenderUtils.TranslateDrawLine(from, to, Color.yellow);
    //
    //        return true;
    //    }
    //}

    [HarmonyPatch(typeof(RunManager), "LeaveToMainMenu")]
    public static class Patches_LeaveToMainMenu
    {
        public static bool Prefix(RunManager __instance)
        {
            Storage.WAYPOINTS_POINTS.Clear();

            GameEvents.TriggerLevelChanged(new ChangeLevelInfo(__instance, true, false, RunManager.ChangeLevelType.MainMenu));

            return true;
        }
    }

    // debugging features!!!
    //[HarmonyPatch(typeof(EnemyDirector), "PickEnemies")]
    //public static class Patches_PickEnemies
    //{
    //    public static bool Prefix(EnemyDirector __instance, List<EnemySetup> _enemiesList)
    //    {
    //        var enemyList = ClientInstance.FetchFieldValue<List<EnemySetup>, EnemyDirector>("enemyList", __instance);
    //        foreach (var enemy in _enemiesList)
    //        {
    //            // private List<EnemySetup> enemyList = new List<EnemySetup>();
    //            if (enemy.name.Contains("Ceiling Eye"))
    //            {
    //                for (var i = 0; i < 5; ++i)
    //                    enemyList.Add(enemy);
    //            }
    //        }
    //        return false;
    //    }
    //}

    // all of this just for freelook
    [HarmonyPatch(typeof(SpiralOnScreen))]
    public static class Patches_SpiralFade
    {
        [HarmonyPatch("FadeIn")]
        [HarmonyPrefix]
        public static bool Prefix_FadeIn()
        {
            overrideFreelook = true;
            return true;
        }

        [HarmonyPatch("FadeOut")]
        [HarmonyPrefix]
        public static bool Prefix_FadeOut()
        {
            overrideFreelook = false;
            return true;
        }
    }

    public static bool isInFreelook = false;
    public static bool overrideFreelook = false;
    public static bool _1TickDisable = false;
    [ClydeChange("Added Freelook mode when ALT is held (Toggle in clickgui)", ClydeVersion.Release_v1_3)]
    [HarmonyPatch(typeof(CameraAim), "Update")]
    public static class Patches_CameraAim
    {
        static bool oldHeld = false;
        static Quaternion originalRotation;
        static float lerpProgress = 0f;
        static float yaw = 0f;
        static float pitch = 0f;
        static bool initialized = false;
        static bool lockInput = false;

        public static bool Prefix(CameraAim __instance)
        {
            if (lockInput)
            {
                lerpProgress += Time.deltaTime * 5f;
                Quaternion target = Quaternion.Euler(originalRotation.eulerAngles.x, originalRotation.eulerAngles.y, 0f);
                Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, target, lerpProgress);
                if (lerpProgress >= 1f)
                {
                    lockInput = false;
                }
                return false;
            }

            var held = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

            var targetObj = ClientInstance.FetchFieldValue<GameObject, CameraAim>("AimTargetObject", __instance);
            if (targetObj != null && __instance.AimTargetActive && targetObj.GetComponent<EnemyCeilingEye>() == true)
            {
                held = false;//FSDJHJSFDHSDFHFSDNH
            }

            if (
                SemiFunc.MenuLevel() ||
                ClientInstance.FetchFieldValue<float, InputManager>("disableAimingTimer", InputManager.instance) > 0 ||
                ClientInstance.FetchFieldValue<bool, CameraAim>("overrideAimStop", CameraAim.Instance) ||
                overrideFreelook ||
                !MenuSettings.VISUAL_FreeLook.Value ||
                ClientInstance.IsHolding<GumballValuable>() ||
                ClientInstance.IsHolding<ValuableBoombox>() ||
                ClientInstance.IsHolding<MusicBoxTrap>()
            )
            {
                held = false;
            }

            if (held)
            {
                if (!initialized)
                {
                    Vector3 euler = Camera.main.transform.eulerAngles;
                    originalRotation = Quaternion.Euler(euler.x, euler.y, 0f);
                    yaw = euler.y;
                    pitch = euler.x > 180f ? euler.x - 360f : euler.x;
                    initialized = true;
                }

                var aimSens = ClientInstance.FetchFieldValue<float, GameplayManager>("aimSensitivity", GameplayManager.instance);
                var aimSensDefault = ClientInstance.FetchFieldValue<int, DataDirector>("aimSensitivityDefault", DataDirector.instance);
                var defaultSetAmt = ClientInstance.FetchFieldValue<float, CameraAim>("defaultSettingAmount", __instance);

                // i give up
                var sens = Mathf.Lerp(aimSens / 100f, aimSensDefault / 100f, defaultSetAmt);
                sens *= 4;

                var mouseX = Input.GetAxis("Mouse X") * sens;
                var mouseY = Input.GetAxis("Mouse Y") * sens;

                yaw += mouseX;
                pitch -= mouseY;

                pitch = Mathf.Clamp(pitch, -60f, 60f);
                var yawDelta = Mathf.DeltaAngle(originalRotation.eulerAngles.y, yaw);
                yaw = Mathf.Clamp(yawDelta, -90f, 90f) + originalRotation.eulerAngles.y;

                Camera.main.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

                isInFreelook = true;
                oldHeld = true;
                return false;
            }

            if (oldHeld)
            {
                isInFreelook = false;
                lerpProgress = 0f;
                oldHeld = false;
                initialized = false;
                lockInput = true;
            }

            return true;
        }
    }

    public static bool isDebugWorld = false;
    [HarmonyPatch(typeof(PlayerController), "Start")]
    public class Patches_PlayerStart
    {
        public static void Postfix(PlayerController __instance)
        {
            if (isDebugWorld)
            {
                isDebugWorld = false;

                DebugWorld.OnDebugStart(__instance);
            }
        }
    }

    private static readonly List<int> completed = new();
    [HarmonyPatch(typeof(ExtractionPoint), "StateComplete")]
    public class Patches_StateComplete
    {
        public static void Postfix(ExtractionPoint __instance)
        {
            var isShop = ClientInstance.FetchFieldValue<bool, ExtractionPoint>("isShop", __instance);

            if (!isShop)// && extractionPointsCompleted == extractionPoints)
            {
                var extractionHaul = ClientInstance.FetchFieldValue<int, ExtractionPoint>("extractionHaul", __instance);

                if (extractionHaul <= 1000)
                    return;

                extractionHaul = Mathf.CeilToInt(extractionHaul / 1000);

                if (!completed.Contains(__instance.GetInstanceID()))
                {
                    completed.Add(__instance.GetInstanceID());
                    MenuSettings.Currency.Value += (uint)extractionHaul;
                    Entry.Log($"Extraction point {__instance.GetInstanceID()} completed, user now has {MenuSettings.Currency.Value}.");
                }
            }
        }
    }

    [HarmonyPatch(typeof(MenuPageMain), "Update")]
    public class Patches_MainMenuUpdate
    {
        public static bool Prefix(MenuPageMain __instance)
        {
            Clocks.Reset("MainMenuUpdate");

            return true;
        }
    }

    [HarmonyPatch(typeof(MenuPageMain), "Start")]
    public class Patches_MainMenuStart
    {
        public static void Postfix(MenuPageMain __instance)
        {
            if (MenuSettings.FastIntro.Value)
            {
                ClientInstance.SetFieldValue("waitTimer", __instance, 3*MenuSettings.FastIntroSpeed.Value);
            }

            MainMenuController.Prepare(__instance);
        }
    }

    // might add options to groom UraniumScript's functionality and other crap
}
