using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Photon.Pun;

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

    [HarmonyPatch(typeof(PlayerAvatar), "OnPhotonSerializeView")]
internal static class Patch_Spinbot
{
    private static float _spinY = 0f;
    private static readonly FieldInfo ClientRotationField =
        typeof(PlayerAvatar).GetField("clientRotation", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

    [HarmonyPrefix]
    private static void Prefix(PlayerAvatar __instance, PhotonStream stream)
    {
        if (!stream.IsWriting) return;
        if (!CheatBehaviour.Instance?.Spinbot ?? true) return;

        _spinY += CheatBehaviour.Instance.SpinSpeed * Time.deltaTime;
        if (_spinY > 360f) _spinY -= 360f;

        // Temporarily replace PlayerController rotation so the stream picks it up
        var pc = PlayerController.instance;
        if (pc != null)
        {
            float originalY = pc.transform.eulerAngles.y;
            pc.transform.rotation = Quaternion.Euler(0f, _spinY, 0f);
        }
    }

    [HarmonyPostfix]
    private static void Postfix(PhotonStream stream)
    {
        if (!stream.IsWriting) return;
        if (!CheatBehaviour.Instance?.Spinbot ?? true) return;

        // Restore real rotation — CameraAim will reset it next frame anyway
        var pc = PlayerController.instance;
        var cam = CameraAim.Instance;
        if (pc != null && cam != null)
        {
            float realY = cam.transform.localRotation.eulerAngles.y;
            pc.transform.rotation = Quaternion.Euler(0f, realY, 0f);
        }
    }
}
}