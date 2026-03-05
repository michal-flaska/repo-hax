using UnityEngine;

namespace cheat
{
    public class Loader
    {
        private static GameObject _cheatObject;

        public static void Load()
        {
            _cheatObject = new GameObject("REPOCheat");
            _cheatObject.AddComponent<CheatBehaviour>();
            UnityEngine.Object.DontDestroyOnLoad(_cheatObject);
        }

        public static void Unload()
        {
            UnityEngine.Object.Destroy(_cheatObject);
        }
    }
}