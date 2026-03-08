using HarmonyLib;
using UnityEngine;

namespace cheat
{
    public class Loader
    {
        private static GameObject _cheatObject;
        private static Harmony _harmony;

        public static void Load()
        {
            _harmony = new Harmony("repo-hax");
            _harmony.PatchAll(typeof(Loader).Assembly);

            _cheatObject = new GameObject("REPOCheat");
            _cheatObject.AddComponent<CheatBehaviour>();
            UnityEngine.Object.DontDestroyOnLoad(_cheatObject);
        }

        public static void Unload()
        {
            _harmony?.UnpatchAll("repo-hax");
            UnityEngine.Object.Destroy(_cheatObject);
        }
    }
}