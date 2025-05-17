    namespace ClydeMenu.Engine;

using System;
using System.Reflection;

using UnityEngine;
using Object = UnityEngine.Object;

using Photon.Pun;
using System.Media;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using static PlayerHealth;
using Unity.VisualScripting;
using System.CodeDom;

internal static class AssemblyReader
{
    public static object Fetch(string typeName)
    {
        var playerlist = SemiFunc.PlayerGetList();
        var steamid = SemiFunc.PlayerGetSteamID(playerlist[0]);

        var type = Type.GetType(typeName);
        if (type != null)
            return Object.FindObjectOfType(type);
        Console.WriteLine($"{typeName} instance not found");
        return null;
    }
}

internal class ClientInstance
{
    public static PlayerController PlayerController
    {
        get
        {
            var instance = AssemblyReader.Fetch("PlayerController, Assembly-CSharp");
            if (instance == null)
                Console.WriteLine("Unable to find PlayerController instance.");

            return (PlayerController)instance;
        }
    }

    public static void SetHealth(int health, int maxHealth)
        => PlayerController.playerAvatarScript.playerHealth.UpdateHealthRPC(health, maxHealth, true);

    // ???????? why is this so fucking buggy bruh
    public static List<EnemyParent> GetEnemyList()
    {
        if (EnemyDirector.instance == null)
            return [];

        var enemies = EnemyDirector.instance.enemiesSpawned;
        if (enemies == null || enemies.Count == 0)
            return [];

        return enemies;
    }

    public static List<GameObject> GetExtractionPoints()
    {
        List<GameObject> ePoints = [];
        try
        {
            if (RoundDirector.instance == null)
                return [];

            ePoints = FetchFieldValue<List<GameObject>, RoundDirector>("extractionPointList", RoundDirector.instance);
            if (ePoints == null || ePoints.Count == 0)
                return [];
        }
        catch (Exception)
        {
            //Console.WriteLine($"Error fetching extraction points: {ex}");
            return [];
        }

        return ePoints;
    }

    private static List<ValuableObject> valuableObjects = [];
    private static Stopwatch timer;
    public static List<ValuableObject> GetValuableList() // this is gonna piss me off lowkey..
    {
        if (timer == null)
            timer = Stopwatch.StartNew();
        
        if (timer.ElapsedMilliseconds >= 1000)
        {
            valuableObjects = Object.FindObjectsOfType<ValuableObject>(true).ToList();
            timer = Stopwatch.StartNew();
        }
        
        return valuableObjects;
    }

    public static GameObject GetLocalPlayer()
    {
        var players = SemiFunc.PlayerGetList();
        if (players == null || players.Count == 0)
            return null;

        return players[0].gameObject;
    }

    public static PlayerAvatar GetLocalAvatar()
    {
        var players = SemiFunc.PlayerGetList();
        if (players == null || players.Count == 0)
            return null;

        return players[0];
    }

    public enum FilterType
    {
        None,
        Dead,
        Alive,
        Master,
        NameContains,
        ActorNumber,
        AllButLocalHost
    }

    public class AvatarFilter
    {
        private static readonly Dictionary<FilterType, Func<object[], PlayerAvatar, bool>> Filters = new()
        {
            [FilterType.None] = (_, _) => true,
            [FilterType.Dead] = (_, p) => FetchFieldValue<int, PlayerHealth>("health", p.playerHealth) <= 0,
            [FilterType.Alive] = (_, p) => FetchFieldValue<int, PlayerHealth>("health", p.playerHealth) > 0,
            [FilterType.Master] = (_, p) => p.photonView.OwnerActorNr == PhotonNetwork.MasterClient.ActorNumber,
            [FilterType.NameContains] = (args, p) => SemiFunc.PlayerGetName(p).Contains((string)args[0]),
            [FilterType.ActorNumber] = (args, p) => p.photonView.OwnerActorNr == (int)args[0],
            [FilterType.AllButLocalHost] = (_, p) =>
                p.photonView.OwnerActorNr != PhotonNetwork.MasterClient.ActorNumber &&
                p.photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber
        };

        public static List<PlayerAvatar> Apply(FilterType type, object[] args)
        {
            var fn = Filters[type];
            var result = new List<PlayerAvatar>();
            foreach (var p in SemiFunc.PlayerGetList())
                if (fn(args, p))
                    result.Add(p);
            return result;
        }
    }

    internal static PhotonView GetPhotonView<T>(T instance)
    {
        var type = typeof(T);
        var field = FetchField<T>("photonView");
        if (field == null)
            throw new MissingFieldException(type.FullName, "photonView");

        var view = (PhotonView)field.GetValue(instance);
        if (view == null)
            throw new NullReferenceException("photonView is null");

        return view;
    }

    internal static FieldInfo FetchField<T>(string v)
        => typeof(T).GetField(v, BindingFlags.NonPublic | BindingFlags.Instance);

    internal static T FetchFieldValue<T, B>(string v, B cls)
    {
        var field = FetchField<B>(v);
        if (field == null)
            throw new Exception($"FieldInfo for {v} doesnt exist");
        return (T)field.GetValue(cls);
    }

    internal static void SetFieldValue<T, B>(string v, B cls, T newValue)
    {
        var field = FetchField<B>(v);
        field.SetValue(cls, newValue);
    }

    internal static MethodInfo FetchMethod<T>(string v)
        => typeof(T).GetMethod(v, BindingFlags.NonPublic | BindingFlags.Instance);

    internal static Bounds GetActiveColliderBounds(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>(true);
        List<Collider> activeColliders = new List<Collider>();

        foreach (Collider col in colliders)
        {
            if (col.enabled && col.gameObject.activeInHierarchy)
                activeColliders.Add(col);
        }

        if (activeColliders.Count == 0)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    if (renderers[i].enabled && renderers[i].gameObject.activeInHierarchy)
                        bounds.Encapsulate(renderers[i].bounds);
                }
                return bounds;
            }
            return new Bounds(obj.transform.position, Vector3.one * 0.5f);
        }

        Bounds resultBounds = activeColliders[0].bounds;
        for (int i = 1; i < activeColliders.Count; i++)
        {
            resultBounds.Encapsulate(activeColliders[i].bounds);
        }

        resultBounds.Expand(0.1f);
        return resultBounds;
    }

    // new MasterAndOwnerOnlyRPC & MasterOnlyRPC func groomed these
    //internal static void SpoofMsg(PlayerAvatar avatar, string msg) => avatar.photonView.RPC("ChatMessageSendRPC", RpcTarget.All, msg, false);
    //internal static void RevivePlayer(PlayerAvatar avatar)
    //{
    //    if (FetchFieldValue<int, PlayerHealth>("health", avatar.playerHealth) > 0)
    //        avatar.Revive(true);
    //}
    //internal static void KillPlayer(PlayerAvatar avatar)
    //{
    //    avatar.playerHealth.Death();
    //    avatar.PlayerDeath(-1);
    //}

    // rpc funcs without security checks
    // exported

    // PlayerAvatar::HealedOtherRPC(void) -> void
    // PlayerAvatar::ResetPhysPusher(void) -> void
    // PlayerAvatar::UpdateMyPlayerVoiceChat(int photonViewID) -> void
    // PlayerAvatar::OverrideAnimationSpeedActivateRPC(bool active, float _speedMulti, float _in, float _out, float _time = 0.1f) -> void
    // PlayerAvatar::OverridePupilSizeActivateRPC(bool active, float _multiplier, int _prio, float springSpeedIn, float dampIn, float springSpeedOut, float dampOut, float _time = 0.1f) -> void

    // NetworkManager::PlayerSpawnedRPC(void) -> void
    // NetworkManager::AllPlayerSpawnedRPC(void) -> void
    // NetworkManager::AllPlayerSpawnedRPC(void) -> void

    // ClownTrap::TouchNoseRPC(void) -> void
    // ClownTrap::HeadSpinDoneRPC(void) -> void

    // AmbienceBreaker::PlaySoundRPC(Vector3 _position, int _preset, int _breaker) -> void

    // BlenderValuable::SetStateRPC(BlenderValuable.States state) -> void

    // Cauldron::EndCookRPC(void) -> void
    // Cauldron::ExplosionRPC(void) -> void
    // Cauldron::CookStartRPC(void) -> void

    // ChargingStation::StopChargeRPC(void) -> void
    // ChargingStation::StartChargeRPC(void) -> void
    // ChargingStation::UpdateChargeBarRPC(int segmentPassed) -> void

    // ChompBookTrap::TrapStopRPC(void) -> void

    // EnemyHealth::HurtRPC(int _damage, Vector3 _hurtDirection) -> void
    // EnemyHealth::DeathRPC(Vector3 _deathDirection) -> void
    // EnemyHealth::DeathImpulseRPC(void) -> void

    // EnemyJump::JumpingSetRPC(bool _jumping) -> void
    // EnemyJump::JumpingDelaySetRPC(bool _jumpingDelay) -> void
    // EnemyJump::LandDelaySetRPC(bool _landDelay) -> void

    // EnemyOnScreen::OnScreenPlayerUpdateRPC(int playerID, bool onScreen, bool culled) -> void

    // EnemyRigidBody::GrabReleaseRPC(void) -> void
    // EnemyRigidBody::GrabbedSetRPC(bool _grabbed) -> void

    // ExtractionPoint::ButtonDenyRPC(void) -> void

    // FanTrap::SetStateRPC(FanTrap.States state) -> void

    // FlameThrowerValuable::GrabTriggerRPC(void) -> void
    // FlameThrowerValuable::ReleaseTriggerRPC(void) -> void
    // FlameThrowerValuable::SetStateRPC(FlameThrowerValuable.States state) -> void

    // FlatScreenTV::BrokenOrNotRPC(bool _broken) -> void
    // FlatScreenTV::actionTimeRPC(void) -> void

    // GumballValuable::PlayerStateChangedRPC(bool _state, int _playerID) -> void

    // IceSawValuable::SetStateRPC(IceSawValuable.States state) -> void

    // ItemAttribute::GetValueRPC(int _value) -> void
    // ItemAttribute::DisableUIRPC(bool _disable) -> void

    // BatteryItem::BatteryToggleRPC(bool toggle) -> void
    // BatteryItem::BatteryChargeStartRPC(bool toggle) -> void
    // BatteryItem::BatteryFullPercentChangeRPC(int batteryLifeInt, bool charge) -> void

    // ItemCartCannon::ShootBulletRPC(Vector3 _end, bool _hit) -> void

    // ItemCartCannonMain::StateSetRPC(int state) -> void

    // ItemDrone::TeleportEffectRPC(Vector3 startPosition, Vector3 endPosition) -> void
    // ItemDrone::ButtonToggleRPC(bool activated) -> void
    // ItemDrone::MagnetActiveToggleRPC(bool activated) -> void
    // ItemDrone::NewRayHitPointRPC(Vector3 newAttachPoint, int photonViewId, int colliderID) -> void
    // ItemDrone::SetTumbleTargetRPC(int _photonViewID) -> void

    // ItemEquippable::RPC_RequestEquip(int spotIndex, int physGrabberPhotonViewID) -> void
    // ItemEquippable::RPC_UpdateItemState(int state, int spotIndex, int ownerId) -> void
    // ItemEquippable::RPC_StartUnequip(int requestingPlayerId) -> void
    // ItemEquippable::RPC_CompleteUnequip(int physGrabberPhotonViewID, Vector3 teleportPos) -> void
    // ItemEquippable::RPC_ForceUnequip(Vector3 dropPosition, int physGrabberPhotonViewID) -> void

    // ItemGrenade::TickStartRPC(void) -> void
    // ItemGrenade::TickEndRPC(void) -> void

    // ItemGun::ShootRPC(void) -> void
    // ItemGun::ShootBulletRPC(Vector3 _endPosition, bool _hit) -> void

    // ItemHealthPack::UsedRPC(void) -> void
    // ItemHealthPack::RejectRPC(void) -> void

    // ItemMelee::EnemyOrPVPSwingHitRPC(bool _playerHit) -> void
    // ItemMelee::SwingHitRPC(bool durabilityLoss) -> void
    // ItemMelee::MeleeBreakRPC(void) -> void
    // ItemMelee::MeleeFixRPC(void) -> void

    // ItemMeleeInflatableHammer::ExplosionRPC(void) -> void

    // ItemMine::TriggerRPC(void) -> void
    // ItemMine::SetStateRPC(ItemMine.States state) -> void
}
