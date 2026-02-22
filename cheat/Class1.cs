using System;
using System.Reflection;
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

    public class CheatBehaviour : MonoBehaviour
    {
        private bool _menuOpen = false;
        private bool _godMode = false;

        private PlayerHealth _localPlayerHealth;

        private static FieldInfo _healthField = typeof(PlayerHealth).GetField("health", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        private static FieldInfo _isLocalField = typeof(PlayerAvatar).GetField("isLocal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Insert))
                _menuOpen = !_menuOpen;

            if (_localPlayerHealth == null)
                _localPlayerHealth = FindLocalPlayer();

            if (_godMode && _localPlayerHealth != null)
            {
                _localPlayerHealth.InvincibleSet(9999f);
                _healthField?.SetValue(_localPlayerHealth, 100);
            }
        }

        void OnGUI()
        {
            if (!_menuOpen) return;

            UnityEngine.GUI.Box(new Rect(20, 20, 200, 100), "REPO Cheat");
            _godMode = UnityEngine.GUI.Toggle(new Rect(30, 50, 180, 25), _godMode, "God Mode");
        }

        private PlayerHealth FindLocalPlayer()
        {
            PlayerHealth[] players = UnityEngine.Object.FindObjectsOfType<PlayerHealth>();
            foreach (var p in players)
            {
                PlayerAvatar avatar = p.GetComponent<PlayerAvatar>();
                if (avatar != null && (bool)(_isLocalField?.GetValue(avatar) ?? false))
                    return p;
            }
            return null;
        }
    }
}