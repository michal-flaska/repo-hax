using HarmonyLib;
using UnityEngine;

namespace cheat
{
    [HarmonyPatch(typeof(PhysGrabObjectImpactDetector), "BreakRPC")]
    internal static class Patch_BreakRPC
    {
        [HarmonyPrefix]
        private static bool Prefix() => false;
    }
}