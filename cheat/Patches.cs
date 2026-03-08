using HarmonyLib;
using System.Reflection;
using UnityEngine;

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
}