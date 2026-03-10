using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace cheat
{
    [HarmonyPatch(typeof(PhysGrabObjectImpactDetector), "BreakRPC")]
    internal static class Patch_BreakRPC
    {
        [HarmonyPrefix]
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(PlayerHealthGrab), "Update")]
    internal static class Patch_HealthGrab
    {
        static readonly FieldInfo TimerField =
            typeof(PlayerHealthGrab).GetField("grabbingTimer", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo ColliderActiveField =
            typeof(PlayerHealthGrab).GetField("colliderActive", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo StaticGrabField =
            typeof(PlayerHealthGrab).GetField("staticGrabObject", BindingFlags.Instance | BindingFlags.NonPublic);

        [HarmonyPrefix]
        private static bool Prefix(PlayerHealthGrab __instance)
        {
            bool colliderActive = (bool)(ColliderActiveField?.GetValue(__instance) ?? false);
            if (!colliderActive) return true;

            var sgo = StaticGrabField?.GetValue(__instance) as StaticGrabObject;
            if (sgo == null || sgo.playerGrabbing.Count == 0) return true;

            float timer = (float)(TimerField?.GetValue(__instance) ?? 0f);
            timer += Time.deltaTime;
            TimerField?.SetValue(__instance, timer);

            if (timer >= 1f)
            {
                foreach (var grabber in sgo.playerGrabbing)
                {
                    var target = __instance.playerAvatar;
                    int hp = (int)(CheatBehaviour.HealthField?.GetValue(target.playerHealth) ?? 0);
                    int maxHp = (int)(typeof(PlayerHealth).GetField("maxHealth", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(target.playerHealth) ?? 100);
                    if (hp != maxHp)
                        target.playerHealth.HealOther(100, true); // how much to heal
                }
                TimerField?.SetValue(__instance, 0f);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(SemiFunc), "IsMasterClient")]
    internal static class Patch_GameManager_IsMasterClient
    {
        [HarmonyPrefix]
        private static bool Prefix(ref bool __result)
        {
            if (!CheatState.SpoofHost) return true;
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(SemiFunc), "IsMasterClientOrSingleplayer")]
    internal static class Patch_GameManager_IsMasterClientOrSingleplayer
    {
        [HarmonyPrefix]
        private static bool Prefix(ref bool __result)
        {
            if (!CheatState.SpoofHost) return true;
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(FlashlightController), "Update")]
    internal static class Patch_Flashlight
    {
        [HarmonyPostfix]
        private static void Postfix(FlashlightController __instance)
        {
            var c = CheatBehaviour.Instance;
            bool isLocal = (bool)(CheatBehaviour.AvatarIsLocalField?.GetValue(__instance.PlayerAvatar) ?? false);
            if (c == null || !isLocal) return;

            if (c.FlashlightCustomColor)
                __instance.spotlight.color = c.FlashlightColor;

            if (c.FlashlightIntensity != 3f)
                __instance.spotlight.intensity = c.FlashlightIntensity;
        }
    }

    [HarmonyPatch(typeof(PlayerController), "FixedUpdate")]
    internal static class Patch_Spinbot
    {
        private static float _spinY = 0f;

        [HarmonyPostfix]
        private static void Postfix()
        {
            var c = CheatBehaviour.Instance;
            if (c == null || !c.Spinbot) return;

            var avatar = PlayerAvatar.instance;
            if (avatar == null) return;

            _spinY += c.SpinSpeed * Time.fixedDeltaTime;
            if (_spinY >= 360f) _spinY -= 360f;

            CheatBehaviour.ClientRotationField?.SetValue(avatar, Quaternion.Euler(0f, _spinY, 0f));
        }
    }

    //[HarmonyPatch(typeof(NetworkManager), "OnEventReceivedCustom")]
    //internal static class Patch_AntiKick
    //{
    //    [HarmonyPrefix]
    //    private static bool Prefix(EventData photonEvent)
    //    {
    //        // dnspy shitnotes
    //        //  199 = server kick
    //        //  123 = host kick
    //        //  124 = host ban
    //        if (photonEvent.Code == 199 || photonEvent.Code == 123 || photonEvent.Code == 124)
    //            return false;
    //        return true;
    //    }
    //}

    [HarmonyPatch(typeof(NetworkManager), "TriggerLeavePhotonRoomForced")]
    internal static class Patch_AntiKick
    {
        [HarmonyPrefix]
        private static bool Prefix() => false; // just block it entirely
    }
}